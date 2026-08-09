"use client"

import { useState } from "react"
import { useParams, useRouter } from "next/navigation"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { ArrowLeft } from "lucide-react"
import { getSubmissionsForAssignment, gradeSubmission, updateSubmissionStatus, TeacherSubmission } from "@/lib/api/teacher"
import { Pagination } from "@/components/ui/Pagination"

const STATUS_OPTIONS = ["Submitted", "Late", "UnderReview", "Graded", "ResubmissionRequested"]

export default function AssignmentSubmissionsPage() {
  const params = useParams<{ id: string }>()
  const router = useRouter()
  const [page, setPage] = useState(1)
  const [gradingId, setGradingId] = useState<string | null>(null)
  const [marks, setMarks] = useState("")
  const [feedback, setFeedback] = useState("")
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ["teacher-submissions", params.id, page],
    queryFn: () => getSubmissionsForAssignment(params.id, page),
  })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["teacher-submissions", params.id] })

  const gradeMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: { marks: number; feedback?: string } }) => gradeSubmission(id, payload),
    onSuccess: () => { toast.success("Submission graded."); invalidate(); setGradingId(null) },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to grade submission."),
  })

  const statusMutation = useMutation({
    mutationFn: ({ id, status }: { id: string; status: string }) => updateSubmissionStatus(id, { status }),
    onSuccess: () => { toast.success("Status updated."); invalidate() },
    onError: () => toast.error("Failed to update status."),
  })

  const openGrading = (s: TeacherSubmission) => {
    setGradingId(s.id)
    setMarks(s.marks !== null ? String(s.marks) : "")
    setFeedback(s.feedback ?? "")
  }

  const handleGradeSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!gradingId) return
    gradeMutation.mutate({ id: gradingId, payload: { marks: Number(marks), feedback: feedback || undefined } })
  }

  return (
    <div>
      <button onClick={() => router.back()} className="flex items-center gap-2 text-sm text-muted-foreground mb-4 hover:text-foreground">
        <ArrowLeft size={16} /> Back to assignments
      </button>

      <h1 className="text-2xl font-semibold mb-6">
        Submissions {data?.items[0]?.assignmentTitle ? `— ${data.items[0].assignmentTitle}` : ""}
      </h1>

      <div className="space-y-4">
        {isLoading && <p className="text-muted-foreground text-sm">Loading...</p>}
        {!isLoading && data?.items.length === 0 && <p className="text-muted-foreground text-sm">No submissions yet.</p>}

        {data?.items.map((s) => (
          <div key={s.id} className="bg-card border rounded-xl p-4">
            <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2">
              <div>
                <p className="font-medium">{s.studentName}</p>
                <p className="text-xs text-muted-foreground">{s.studentEmail}</p>
                <p className="text-xs text-muted-foreground mt-1">Submitted {new Date(s.submittedAt).toLocaleString()}</p>
              </div>
              <div className="flex items-center gap-2">
                <select
                  value={s.status}
                  onChange={(e) => statusMutation.mutate({ id: s.id, status: e.target.value })}
                  className="border rounded-lg px-2 py-1 text-xs bg-card"
                >
                  {STATUS_OPTIONS.map((opt) => <option key={opt} value={opt}>{opt}</option>)}
                </select>
              </div>
            </div>

            <p className="text-sm mt-3 whitespace-pre-wrap">{s.answerText}</p>
            {s.attachmentUrl && (
              <a href={s.attachmentUrl} target="_blank" rel="noreferrer" className="text-sm text-accent hover:underline mt-2 inline-block">
                View attachment
              </a>
            )}

            {s.status === "Graded" && (
              <div className="mt-3 p-3 rounded-lg bg-green-50 text-sm">
                <p className="font-medium">Marks: {s.marks}</p>
                {s.feedback && <p className="text-muted-foreground mt-1">{s.feedback}</p>}
              </div>
            )}

            {gradingId === s.id ? (
              <form onSubmit={handleGradeSubmit} className="mt-3 space-y-2 border-t pt-3">
                <div className="flex gap-2">
                  <input
                    type="number"
                    required
                    min={0}
                    placeholder="Marks"
                    value={marks}
                    onChange={(e) => setMarks(e.target.value)}
                    className="border rounded-lg px-3 py-2 text-sm bg-card w-24"
                  />
                  <input
                    placeholder="Feedback (optional)"
                    value={feedback}
                    onChange={(e) => setFeedback(e.target.value)}
                    className="border rounded-lg px-3 py-2 text-sm bg-card flex-1"
                  />
                </div>
                <div className="flex gap-2">
                  <button type="submit" disabled={gradeMutation.isPending} className="px-3 py-1.5 rounded-lg bg-foreground text-background text-xs font-medium disabled:opacity-50">
                    Save grade
                  </button>
                  <button type="button" onClick={() => setGradingId(null)} className="px-3 py-1.5 rounded-lg border text-xs font-medium">
                    Cancel
                  </button>
                </div>
              </form>
            ) : (
              <button onClick={() => openGrading(s)} className="mt-3 text-sm font-medium text-accent hover:underline">
                {s.status === "Graded" ? "Update grade" : "Grade submission"}
              </button>
            )}
          </div>
        ))}
      </div>

      {data && <Pagination page={data.page} totalPages={data.totalPages} hasPrevious={data.hasPrevious} hasNext={data.hasNext} onPageChange={setPage} />}
    </div>
  )
}
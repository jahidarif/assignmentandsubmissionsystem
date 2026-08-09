"use client"

import { useState, useEffect } from "react"
import { useParams, useRouter } from "next/navigation"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { ArrowLeft } from "lucide-react"
import { getAssignmentById, submitAssignment, updateSubmission, getMySubmissions } from "@/lib/api/student"

export default function StudentAssignmentDetailPage() {
  const params = useParams<{ id: string }>()
  const router = useRouter()
  const queryClient = useQueryClient()

  const [answerText, setAnswerText] = useState("")
  const [attachmentUrl, setAttachmentUrl] = useState("")

  const { data: assignment, isLoading } = useQuery({
    queryKey: ["student-assignment", params.id],
    queryFn: () => getAssignmentById(params.id),
  })

  const { data: submissionsPage } = useQuery({
    queryKey: ["student-submissions-check", params.id],
    queryFn: () => getMySubmissions(1),
    enabled: !!assignment?.hasSubmitted,
  })

  const existingSubmission = submissionsPage?.items.find((s) => s.assignmentId === params.id)

  useEffect(() => {
    if (existingSubmission) {
      setAnswerText(existingSubmission.answerText)
      setAttachmentUrl(existingSubmission.attachmentUrl ?? "")
    }
  }, [existingSubmission])

  const submitMutation = useMutation({
    mutationFn: () => submitAssignment(params.id, { answerText, attachmentUrl: attachmentUrl || undefined }),
    onSuccess: () => {
      toast.success("Submission sent.")
      queryClient.invalidateQueries({ queryKey: ["student-assignment", params.id] })
      queryClient.invalidateQueries({ queryKey: ["student-assignments"] })
      router.push("/dashboard/student/submissions")
    },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to submit."),
  })

  const updateMutation = useMutation({
    mutationFn: () => updateSubmission(existingSubmission!.id, { answerText, attachmentUrl: attachmentUrl || undefined }),
    onSuccess: () => {
      toast.success("Submission updated.")
      queryClient.invalidateQueries({ queryKey: ["student-assignment", params.id] })
      router.push("/dashboard/student/submissions")
    },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to update."),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (existingSubmission) {
      updateMutation.mutate()
    } else {
      submitMutation.mutate()
    }
  }

  if (isLoading) return <p className="text-muted-foreground text-sm">Loading...</p>
  if (!assignment) return <p className="text-muted-foreground text-sm">Assignment not found.</p>

  return (
    <div className="max-w-2xl">
      <button onClick={() => router.back()} className="flex items-center gap-2 text-sm text-muted-foreground mb-4 hover:text-foreground">
        <ArrowLeft size={16} /> Back
      </button>

      <div className="bg-card border rounded-xl p-5 mb-6">
        <h1 className="text-xl font-semibold">{assignment.title}</h1>
        <p className="text-sm text-muted-foreground mt-1">
          {assignment.classCourseName} — {assignment.subjectName} · {assignment.teacherName}
        </p>
        <p className="text-sm mt-4 whitespace-pre-wrap">{assignment.description}</p>
        <div className="flex gap-4 mt-4 text-sm text-muted-foreground">
          <span>Deadline: {new Date(assignment.deadline).toLocaleString()}</span>
          <span>Max marks: {assignment.maxMarks}</span>
        </div>
      </div>

      {assignment.isPastDeadline && !assignment.hasSubmitted && (
        <p className="text-sm text-red-600">The deadline for this assignment has passed. You can no longer submit.</p>
      )}

      {(!assignment.isPastDeadline || existingSubmission?.canUpdate) && (!assignment.hasSubmitted || existingSubmission?.canUpdate) && (
        <form onSubmit={handleSubmit} className="bg-card border rounded-xl p-5 space-y-3">
          <h2 className="font-medium">{existingSubmission ? "Update your submission" : "Submit your answer"}</h2>
          <div>
            <label className="text-sm font-medium">Answer</label>
            <textarea
              required
              rows={6}
              value={answerText}
              onChange={(e) => setAnswerText(e.target.value)}
              className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card"
            />
          </div>
          <div>
            <label className="text-sm font-medium">Attachment URL (optional)</label>
            <input
              value={attachmentUrl}
              onChange={(e) => setAttachmentUrl(e.target.value)}
              className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card"
              placeholder="https://..."
            />
          </div>
          <button
            type="submit"
            disabled={submitMutation.isPending || updateMutation.isPending}
            className="px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium disabled:opacity-50"
          >
            {existingSubmission ? "Save changes" : "Submit"}
          </button>
        </form>
      )}

      {assignment.hasSubmitted && existingSubmission && !existingSubmission.canUpdate && (
        <p className="text-sm text-muted-foreground">
          Deadline has passed — your submission is locked. View it under "My Submissions".
        </p>
      )}
    </div>
  )
}
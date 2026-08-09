"use client"

import { useState } from "react"
import { useQuery } from "@tanstack/react-query"
import Link from "next/link"
import { getMySubmissions } from "@/lib/api/student"
import { Pagination } from "@/components/ui/Pagination"

const STATUS_COLORS: Record<string, string> = {
  Submitted: "bg-blue-100 text-blue-700",
  Late: "bg-orange-100 text-orange-700",
  UnderReview: "bg-yellow-100 text-yellow-700",
  Graded: "bg-green-100 text-green-700",
  ResubmissionRequested: "bg-red-100 text-red-700",
}

export default function StudentSubmissionsPage() {
  const [page, setPage] = useState(1)
  const { data, isLoading } = useQuery({ queryKey: ["student-my-submissions", page], queryFn: () => getMySubmissions(page) })

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-6">My Submissions</h1>

      <div className="space-y-3">
        {isLoading && <p className="text-muted-foreground text-sm">Loading...</p>}
        {!isLoading && data?.items.length === 0 && <p className="text-muted-foreground text-sm">No submissions yet.</p>}

        {data?.items.map((s) => (
          <div key={s.id} className="bg-card border rounded-xl p-4">
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2">
              <div>
                <p className="font-medium">{s.assignmentTitle}</p>
                <p className="text-xs text-muted-foreground mt-1">Submitted {new Date(s.submittedAt).toLocaleString()}</p>
              </div>
              <span className={`px-2 py-1 rounded-full text-xs font-medium ${STATUS_COLORS[s.status] ?? "bg-gray-100 text-gray-700"}`}>
                {s.status}
              </span>
            </div>

            {s.status === "Graded" ? (
              <div className="mt-3 p-3 rounded-lg bg-green-50 text-sm">
                <p className="font-medium">Marks: {s.marks} / {s.assignmentMaxMarks}</p>
                {s.feedback && <p className="text-muted-foreground mt-1">{s.feedback}</p>}
              </div>
            ) : (
              <p className="text-xs text-muted-foreground mt-3">Not graded yet.</p>
            )}

            {s.canUpdate && (
              <Link href={`/dashboard/student/assignments/${s.assignmentId}`} className="text-sm text-accent hover:underline mt-3 inline-block">
                Update submission
              </Link>
            )}
          </div>
        ))}
      </div>

      {data && <Pagination page={data.page} totalPages={data.totalPages} hasPrevious={data.hasPrevious} hasNext={data.hasNext} onPageChange={setPage} />}
    </div>
  )
}
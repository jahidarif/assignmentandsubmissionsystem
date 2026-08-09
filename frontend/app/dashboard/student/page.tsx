"use client"

import { useState } from "react"
import { useQuery } from "@tanstack/react-query"
import Link from "next/link"
import { getAssignments } from "@/lib/api/student"
import { Pagination } from "@/components/ui/Pagination"

export default function StudentAssignmentsPage() {
  const [page, setPage] = useState(1)
  const { data, isLoading } = useQuery({ queryKey: ["student-assignments", page], queryFn: () => getAssignments(page) })

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-6">Assignments</h1>

      <div className="space-y-3">
        {isLoading && <p className="text-muted-foreground text-sm">Loading...</p>}
        {!isLoading && data?.items.length === 0 && <p className="text-muted-foreground text-sm">No assignments yet.</p>}

        {data?.items.map((a) => (
          <Link
            key={a.id}
            href={`/dashboard/student/assignments/${a.id}`}
            className="block bg-card border rounded-xl p-4 hover:border-foreground transition-colors"
          >
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2">
              <div>
                <p className="font-medium">{a.title}</p>
                <p className="text-xs text-muted-foreground mt-1">
                  {a.classCourseName} — {a.subjectName} · {a.teacherName}
                </p>
              </div>
              <div className="flex items-center gap-2">
                {a.hasSubmitted && (
                  <span className="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-700">Submitted</span>
                )}
                {!a.hasSubmitted && a.isPastDeadline && (
                  <span className="px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-700">Missed</span>
                )}
                {!a.hasSubmitted && !a.isPastDeadline && (
                  <span className="px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-700">Pending</span>
                )}
              </div>
            </div>
            <p className="text-xs text-muted-foreground mt-2">
              Due {new Date(a.deadline).toLocaleString()} · Max marks {a.maxMarks}
            </p>
          </Link>
        ))}
      </div>

      {data && <Pagination page={data.page} totalPages={data.totalPages} hasPrevious={data.hasPrevious} hasNext={data.hasNext} onPageChange={setPage} />}
    </div>
  )
}
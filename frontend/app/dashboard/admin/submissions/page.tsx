"use client"

import { useState } from "react"
import { useQuery } from "@tanstack/react-query"
import { getAllSubmissions } from "@/lib/api/admin"
import { Pagination } from "@/components/ui/Pagination"

export default function AdminSubmissionsPage() {
  const [page, setPage] = useState(1)
  const { data, isLoading } = useQuery({ queryKey: ["admin-submissions", page], queryFn: () => getAllSubmissions(page) })

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-6">All Submissions</h1>
      <div className="bg-card rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Assignment</th>
              <th className="px-4 py-3 font-medium">Student</th>
              <th className="px-4 py-3 font-medium">Submitted</th>
              <th className="px-4 py-3 font-medium">Status</th>
              <th className="px-4 py-3 font-medium">Marks</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && <tr><td colSpan={5} className="px-4 py-8 text-center text-muted-foreground">Loading...</td></tr>}
            {!isLoading && data?.items.length === 0 && <tr><td colSpan={5} className="px-4 py-8 text-center text-muted-foreground">No submissions yet.</td></tr>}
            {data?.items.map((s) => (
              <tr key={s.id} className="border-b last:border-0">
                <td className="px-4 py-3">{s.assignmentTitle}</td>
                <td className="px-4 py-3">{s.studentName}</td>
                <td className="px-4 py-3">{new Date(s.submittedAt).toLocaleDateString()}</td>
                <td className="px-4 py-3">{s.status}</td>
                <td className="px-4 py-3">{s.marks ?? "—"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {data && <Pagination page={data.page} totalPages={data.totalPages} hasPrevious={data.hasPrevious} hasNext={data.hasNext} onPageChange={setPage} />}
    </div>
  )
}
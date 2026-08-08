"use client"

import { useState } from "react"
import { useQuery } from "@tanstack/react-query"
import { getAllAssignments } from "@/lib/api/admin"
import { Pagination } from "@/components/ui/Pagination"

export default function AdminAssignmentsPage() {
  const [page, setPage] = useState(1)
  const { data, isLoading } = useQuery({ queryKey: ["admin-assignments", page], queryFn: () => getAllAssignments(page) })

  return (
    <div>
      <h1 className="text-2xl font-semibold mb-6">All Assignments</h1>
      <div className="bg-card rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Title</th>
              <th className="px-4 py-3 font-medium">Class</th>
              <th className="px-4 py-3 font-medium">Subject</th>
              <th className="px-4 py-3 font-medium">Teacher</th>
              <th className="px-4 py-3 font-medium">Deadline</th>
              <th className="px-4 py-3 font-medium">Status</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && <tr><td colSpan={6} className="px-4 py-8 text-center text-muted-foreground">Loading...</td></tr>}
            {!isLoading && data?.items.length === 0 && <tr><td colSpan={6} className="px-4 py-8 text-center text-muted-foreground">No assignments yet.</td></tr>}
            {data?.items.map((a) => (
              <tr key={a.id} className="border-b last:border-0">
                <td className="px-4 py-3">{a.title}</td>
                <td className="px-4 py-3">{a.classCourseName}</td>
                <td className="px-4 py-3">{a.subjectName}</td>
                <td className="px-4 py-3">{a.teacherName}</td>
                <td className="px-4 py-3">{new Date(a.deadline).toLocaleDateString()}</td>
                <td className="px-4 py-3">{a.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {data && <Pagination page={data.page} totalPages={data.totalPages} hasPrevious={data.hasPrevious} hasNext={data.hasNext} onPageChange={setPage} />}
    </div>
  )
}
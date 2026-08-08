"use client"

import { useState } from "react"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { getUsers, deactivateUser, reactivateUser } from "@/lib/api/admin"
import { Pagination } from "@/components/ui/Pagination"

export default function UsersPage() {
  const [page, setPage] = useState(1)
  const [roleFilter, setRoleFilter] = useState("")
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ["admin-users", page, roleFilter],
    queryFn: () => getUsers(page, roleFilter || undefined),
  })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["admin-users"] })

  const deactivateMutation = useMutation({
    mutationFn: deactivateUser,
    onSuccess: () => { toast.success("User deactivated."); invalidate() },
    onError: () => toast.error("Failed to deactivate user."),
  })

  const reactivateMutation = useMutation({
    mutationFn: reactivateUser,
    onSuccess: () => { toast.success("User reactivated."); invalidate() },
    onError: () => toast.error("Failed to reactivate user."),
  })

  return (
    <div>
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-6">
        <h1 className="text-2xl font-semibold">Users</h1>
        <select
          value={roleFilter}
          onChange={(e) => { setRoleFilter(e.target.value); setPage(1) }}
          className="border rounded-lg px-3 py-2 text-sm bg-card"
        >
          <option value="">All roles</option>
          <option value="Admin">Admin</option>
          <option value="Teacher">Teacher</option>
          <option value="Student">Student</option>
        </select>
      </div>

      <div className="bg-card rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Name</th>
              <th className="px-4 py-3 font-medium">Email</th>
              <th className="px-4 py-3 font-medium">Role</th>
              <th className="px-4 py-3 font-medium">Status</th>
              <th className="px-4 py-3 font-medium text-right">Action</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && <tr><td colSpan={5} className="px-4 py-8 text-center text-muted-foreground">Loading...</td></tr>}
            {!isLoading && data?.items.length === 0 && <tr><td colSpan={5} className="px-4 py-8 text-center text-muted-foreground">No users found.</td></tr>}
            {data?.items.map((u) => (
              <tr key={u.id} className="border-b last:border-0">
                <td className="px-4 py-3 whitespace-nowrap">{u.fullName}</td>
                <td className="px-4 py-3 whitespace-nowrap">{u.email}</td>
                <td className="px-4 py-3">{u.role}</td>
                <td className="px-4 py-3">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${u.isActive ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"}`}>
                    {u.isActive ? "Active" : "Deactivated"}
                  </span>
                </td>
                <td className="px-4 py-3 text-right">
                  {u.isActive ? (
                    <button onClick={() => deactivateMutation.mutate(u.id)} className="text-sm font-medium text-red-600 hover:underline">Deactivate</button>
                  ) : (
                    <button onClick={() => reactivateMutation.mutate(u.id)} className="text-sm font-medium text-green-600 hover:underline">Reactivate</button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {data && <Pagination page={data.page} totalPages={data.totalPages} hasPrevious={data.hasPrevious} hasNext={data.hasNext} onPageChange={setPage} />}
    </div>
  )
}
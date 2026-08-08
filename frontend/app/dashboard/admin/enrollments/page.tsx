"use client"

import { useState } from "react"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Plus, Trash2 } from "lucide-react"
import { getEnrollments, createEnrollment, deleteEnrollment, getStudentsLookup, getClassCoursesLookup } from "@/lib/api/admin"
import { Pagination } from "@/components/ui/Pagination"

export default function EnrollmentsPage() {
  const [page, setPage] = useState(1)
  const [formOpen, setFormOpen] = useState(false)
  const [studentId, setStudentId] = useState("")
  const [classCourseId, setClassCourseId] = useState("")
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({ queryKey: ["admin-enrollments", page], queryFn: () => getEnrollments(page) })
  const { data: students } = useQuery({ queryKey: ["lookup-students"], queryFn: getStudentsLookup })
  const { data: classCourses } = useQuery({ queryKey: ["lookup-class-courses"], queryFn: getClassCoursesLookup })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["admin-enrollments"] })

  const createMutation = useMutation({
    mutationFn: createEnrollment,
    onSuccess: () => { toast.success("Student enrolled."); invalidate(); closeForm() },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to enroll student."),
  })

  const deleteMutation = useMutation({
    mutationFn: deleteEnrollment,
    onSuccess: () => { toast.success("Unenrolled."); invalidate() },
    onError: () => toast.error("Failed to unenroll."),
  })

  const closeForm = () => { setFormOpen(false); setStudentId(""); setClassCourseId("") }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createMutation.mutate({ studentId, classCourseId })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Enrollments</h1>
        <button onClick={() => setFormOpen(true)} className="flex items-center gap-2 px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium">
          <Plus size={16} /> Enroll student
        </button>
      </div>

      {formOpen && (
        <form onSubmit={handleSubmit} className="bg-card border rounded-xl p-4 mb-6 space-y-3 max-w-md">
          <div>
            <label className="text-sm font-medium">Student</label>
            <select required value={studentId} onChange={(e) => setStudentId(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
              <option value="">Select a student</option>
              {students?.map((s) => <option key={s.id} value={s.id}>{s.fullName}</option>)}
            </select>
          </div>
          <div>
            <label className="text-sm font-medium">Class</label>
            <select required value={classCourseId} onChange={(e) => setClassCourseId(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
              <option value="">Select a class</option>
              {classCourses?.map((c) => <option key={c.id} value={c.id}>{c.name}{c.section ? ` - ${c.section}` : ""}</option>)}
            </select>
          </div>
          <div className="flex gap-2">
            <button type="submit" disabled={createMutation.isPending} className="px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium disabled:opacity-50">Enroll</button>
            <button type="button" onClick={closeForm} className="px-4 py-2 rounded-lg border text-sm font-medium">Cancel</button>
          </div>
        </form>
      )}

      <div className="bg-card rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Student</th>
              <th className="px-4 py-3 font-medium">Class</th>
              <th className="px-4 py-3 font-medium">Enrolled</th>
              <th className="px-4 py-3 font-medium text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && <tr><td colSpan={4} className="px-4 py-8 text-center text-muted-foreground">Loading...</td></tr>}
            {!isLoading && data?.items.length === 0 && <tr><td colSpan={4} className="px-4 py-8 text-center text-muted-foreground">No enrollments yet.</td></tr>}
            {data?.items.map((en) => (
              <tr key={en.id} className="border-b last:border-0">
                <td className="px-4 py-3">{en.studentName}</td>
                <td className="px-4 py-3">{en.classCourseName}</td>
                <td className="px-4 py-3">{new Date(en.enrolledAt).toLocaleDateString()}</td>
                <td className="px-4 py-3 text-right">
                  <button onClick={() => deleteMutation.mutate(en.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
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
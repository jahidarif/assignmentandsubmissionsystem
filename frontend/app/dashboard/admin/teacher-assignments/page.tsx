"use client"

import { useState } from "react"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Plus, Trash2 } from "lucide-react"
import { getTeacherAssignments, createTeacherAssignment, deleteTeacherAssignment, getTeachersLookup, getClassSubjectsLookup } from "@/lib/api/admin"
import { Pagination } from "@/components/ui/Pagination"

export default function TeacherAssignmentsPage() {
  const [page, setPage] = useState(1)
  const [formOpen, setFormOpen] = useState(false)
  const [teacherId, setTeacherId] = useState("")
  const [classSubjectId, setClassSubjectId] = useState("")
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({ queryKey: ["admin-teacher-assignments", page], queryFn: () => getTeacherAssignments(page) })
  const { data: teachers } = useQuery({ queryKey: ["lookup-teachers"], queryFn: getTeachersLookup })
  const { data: classSubjects } = useQuery({ queryKey: ["lookup-class-subjects"], queryFn: getClassSubjectsLookup })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["admin-teacher-assignments"] })

  const createMutation = useMutation({
    mutationFn: createTeacherAssignment,
    onSuccess: () => { toast.success("Teacher assigned."); invalidate(); closeForm() },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to assign teacher."),
  })

  const deleteMutation = useMutation({
    mutationFn: deleteTeacherAssignment,
    onSuccess: () => { toast.success("Unassigned."); invalidate() },
    onError: () => toast.error("Failed to unassign."),
  })

  const closeForm = () => { setFormOpen(false); setTeacherId(""); setClassSubjectId("") }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createMutation.mutate({ teacherId, classSubjectId })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Teacher Assignments</h1>
        <button onClick={() => setFormOpen(true)} className="flex items-center gap-2 px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium">
          <Plus size={16} /> Assign teacher
        </button>
      </div>

      {formOpen && (
        <form onSubmit={handleSubmit} className="bg-card border rounded-xl p-4 mb-6 space-y-3 max-w-md">
          <div>
            <label className="text-sm font-medium">Teacher</label>
            <select required value={teacherId} onChange={(e) => setTeacherId(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
              <option value="">Select a teacher</option>
              {teachers?.map((t) => <option key={t.id} value={t.id}>{t.fullName}</option>)}
            </select>
          </div>
          <div>
            <label className="text-sm font-medium">Class + Subject</label>
            <select required value={classSubjectId} onChange={(e) => setClassSubjectId(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
              <option value="">Select a class-subject</option>
              {classSubjects?.map((cs) => <option key={cs.id} value={cs.id}>{cs.classCourseName} — {cs.subjectName}</option>)}
            </select>
          </div>
          <div className="flex gap-2">
            <button type="submit" disabled={createMutation.isPending} className="px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium disabled:opacity-50">Assign</button>
            <button type="button" onClick={closeForm} className="px-4 py-2 rounded-lg border text-sm font-medium">Cancel</button>
          </div>
        </form>
      )}

      <div className="bg-card rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Teacher</th>
              <th className="px-4 py-3 font-medium">Class</th>
              <th className="px-4 py-3 font-medium">Subject</th>
              <th className="px-4 py-3 font-medium text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && <tr><td colSpan={4} className="px-4 py-8 text-center text-muted-foreground">Loading...</td></tr>}
            {!isLoading && data?.items.length === 0 && <tr><td colSpan={4} className="px-4 py-8 text-center text-muted-foreground">No assignments yet.</td></tr>}
            {data?.items.map((ta) => (
              <tr key={ta.id} className="border-b last:border-0">
                <td className="px-4 py-3">{ta.teacherName}</td>
                <td className="px-4 py-3">{ta.classCourseName}</td>
                <td className="px-4 py-3">{ta.subjectName}</td>
                <td className="px-4 py-3 text-right">
                  <button onClick={() => deleteMutation.mutate(ta.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
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
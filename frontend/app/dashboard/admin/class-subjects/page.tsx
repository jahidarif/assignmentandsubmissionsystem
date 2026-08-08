"use client"

import { useState } from "react"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Plus, Trash2 } from "lucide-react"
import { getClassSubjects, createClassSubject, deleteClassSubject, getClassCoursesLookup, getSubjectsLookup } from "@/lib/api/admin"
import { Pagination } from "@/components/ui/Pagination"

export default function ClassSubjectsPage() {
  const [page, setPage] = useState(1)
  const [formOpen, setFormOpen] = useState(false)
  const [classCourseId, setClassCourseId] = useState("")
  const [subjectId, setSubjectId] = useState("")
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({ queryKey: ["admin-class-subjects", page], queryFn: () => getClassSubjects(page) })
  const { data: classCourses } = useQuery({ queryKey: ["lookup-class-courses"], queryFn: getClassCoursesLookup })
  const { data: subjects } = useQuery({ queryKey: ["lookup-subjects"], queryFn: getSubjectsLookup })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["admin-class-subjects"] })

  const createMutation = useMutation({
    mutationFn: createClassSubject,
    onSuccess: () => { toast.success("Linked."); invalidate(); closeForm() },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to create link."),
  })

  const deleteMutation = useMutation({
    mutationFn: deleteClassSubject,
    onSuccess: () => { toast.success("Removed."); invalidate() },
    onError: () => toast.error("Can't remove — assignments still reference this."),
  })

  const closeForm = () => { setFormOpen(false); setClassCourseId(""); setSubjectId("") }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createMutation.mutate({ classCourseId, subjectId })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Class Subjects</h1>
        <button onClick={() => setFormOpen(true)} className="flex items-center gap-2 px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium">
          <Plus size={16} /> Link subject
        </button>
      </div>

      {formOpen && (
        <form onSubmit={handleSubmit} className="bg-card border rounded-xl p-4 mb-6 space-y-3 max-w-md">
          <div>
            <label className="text-sm font-medium">Class</label>
            <select required value={classCourseId} onChange={(e) => setClassCourseId(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
              <option value="">Select a class</option>
              {classCourses?.map((c) => <option key={c.id} value={c.id}>{c.name}{c.section ? ` - ${c.section}` : ""}</option>)}
            </select>
          </div>
          <div>
            <label className="text-sm font-medium">Subject</label>
            <select required value={subjectId} onChange={(e) => setSubjectId(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
              <option value="">Select a subject</option>
              {subjects?.map((s) => <option key={s.id} value={s.id}>{s.name} ({s.code})</option>)}
            </select>
          </div>
          <div className="flex gap-2">
            <button type="submit" disabled={createMutation.isPending} className="px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium disabled:opacity-50">Link</button>
            <button type="button" onClick={closeForm} className="px-4 py-2 rounded-lg border text-sm font-medium">Cancel</button>
          </div>
        </form>
      )}

      <div className="bg-card rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Class</th>
              <th className="px-4 py-3 font-medium">Subject</th>
              <th className="px-4 py-3 font-medium text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && <tr><td colSpan={3} className="px-4 py-8 text-center text-muted-foreground">Loading...</td></tr>}
            {!isLoading && data?.items.length === 0 && <tr><td colSpan={3} className="px-4 py-8 text-center text-muted-foreground">No links yet.</td></tr>}
            {data?.items.map((cs) => (
              <tr key={cs.id} className="border-b last:border-0">
                <td className="px-4 py-3">{cs.classCourseName}</td>
                <td className="px-4 py-3">{cs.subjectName} ({cs.subjectCode})</td>
                <td className="px-4 py-3 text-right">
                  <button onClick={() => deleteMutation.mutate(cs.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
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
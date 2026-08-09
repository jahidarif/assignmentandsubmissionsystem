"use client"

import { useState } from "react"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import Link from "next/link"
import { Plus, Pencil, Trash2, Send, Eye } from "lucide-react"
import {
  getAssignments,
  createAssignment,
  updateAssignment,
  deleteAssignment,
  publishAssignment,
  getClassSubjectsLookup,
  TeacherAssignment,
} from "@/lib/api/teacher"
import { Pagination } from "@/components/ui/Pagination"

export default function TeacherAssignmentsPage() {
  const [page, setPage] = useState(1)
  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<TeacherAssignment | null>(null)
  const [title, setTitle] = useState("")
  const [description, setDescription] = useState("")
  const [deadline, setDeadline] = useState("")
  const [maxMarks, setMaxMarks] = useState("")
  const [classSubjectId, setClassSubjectId] = useState("")
  const [status, setStatus] = useState<"Draft" | "Published">("Draft")

  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({ queryKey: ["teacher-assignments", page], queryFn: () => getAssignments(page) })
  const { data: classSubjects } = useQuery({ queryKey: ["teacher-class-subjects"], queryFn: getClassSubjectsLookup })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["teacher-assignments"] })

  const createMutation = useMutation({
    mutationFn: createAssignment,
    onSuccess: () => { toast.success("Assignment created."); invalidate(); closeForm() },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to create assignment."),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: any }) => updateAssignment(id, payload),
    onSuccess: () => { toast.success("Assignment updated."); invalidate(); closeForm() },
    onError: (e: any) => toast.error(e.response?.data?.message ?? "Failed to update assignment."),
  })

  const deleteMutation = useMutation({
    mutationFn: deleteAssignment,
    onSuccess: () => { toast.success("Assignment deleted."); invalidate() },
    onError: () => toast.error("Can't delete — submissions already exist for this assignment."),
  })

  const publishMutation = useMutation({
    mutationFn: publishAssignment,
    onSuccess: () => { toast.success("Assignment published."); invalidate() },
    onError: () => toast.error("Failed to publish assignment."),
  })

  const openCreate = () => {
    setEditing(null)
    setTitle(""); setDescription(""); setDeadline(""); setMaxMarks(""); setClassSubjectId(""); setStatus("Draft")
    setFormOpen(true)
  }

  const openEdit = (a: TeacherAssignment) => {
    setEditing(a)
    setTitle(a.title)
    setDescription(a.description)
    setDeadline(a.deadline.slice(0, 16))
    setMaxMarks(String(a.maxMarks))
    setFormOpen(true)
  }

  const closeForm = () => { setFormOpen(false); setEditing(null) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editing) {
      updateMutation.mutate({
        id: editing.id,
        payload: { title, description, deadline: new Date(deadline).toISOString(), maxMarks: Number(maxMarks) },
      })
    } else {
      createMutation.mutate({
        title,
        description,
        deadline: new Date(deadline).toISOString(),
        maxMarks: Number(maxMarks),
        classSubjectId,
        status,
      })
    }
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Assignments</h1>
        <button onClick={openCreate} className="flex items-center gap-2 px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium">
          <Plus size={16} /> New assignment
        </button>
      </div>

      {formOpen && (
        <form onSubmit={handleSubmit} className="bg-card border rounded-xl p-4 mb-6 space-y-3 max-w-lg">
          <div>
            <label className="text-sm font-medium">Title</label>
            <input required value={title} onChange={(e) => setTitle(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card" />
          </div>
          <div>
            <label className="text-sm font-medium">Description</label>
            <textarea required value={description} onChange={(e) => setDescription(e.target.value)} rows={3} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card" />
          </div>
          {!editing && (
            <div>
              <label className="text-sm font-medium">Class + Subject</label>
              <select required value={classSubjectId} onChange={(e) => setClassSubjectId(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
                <option value="">Select a class-subject</option>
                {classSubjects?.map((cs) => <option key={cs.id} value={cs.id}>{cs.classCourseName} — {cs.subjectName}</option>)}
              </select>
            </div>
          )}
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-sm font-medium">Deadline</label>
              <input required type="datetime-local" value={deadline} onChange={(e) => setDeadline(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card" />
            </div>
            <div>
              <label className="text-sm font-medium">Max Marks</label>
              <input required type="number" min={1} value={maxMarks} onChange={(e) => setMaxMarks(e.target.value)} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card" />
            </div>
          </div>
          {!editing && (
            <div>
              <label className="text-sm font-medium">Save as</label>
              <select value={status} onChange={(e) => setStatus(e.target.value as "Draft" | "Published")} className="w-full mt-1 border rounded-lg px-3 py-2 text-sm bg-card">
                <option value="Draft">Draft</option>
                <option value="Published">Published</option>
              </select>
            </div>
          )}
          <div className="flex gap-2">
            <button type="submit" disabled={createMutation.isPending || updateMutation.isPending} className="px-4 py-2 rounded-lg bg-foreground text-background text-sm font-medium disabled:opacity-50">
              {editing ? "Save changes" : "Create"}
            </button>
            <button type="button" onClick={closeForm} className="px-4 py-2 rounded-lg border text-sm font-medium">Cancel</button>
          </div>
        </form>
      )}

      <div className="bg-card rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Title</th>
              <th className="px-4 py-3 font-medium">Class</th>
              <th className="px-4 py-3 font-medium">Subject</th>
              <th className="px-4 py-3 font-medium">Deadline</th>
              <th className="px-4 py-3 font-medium">Status</th>
              <th className="px-4 py-3 font-medium text-right">Actions</th>
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
                <td className="px-4 py-3">{new Date(a.deadline).toLocaleString()}</td>
                <td className="px-4 py-3">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${a.status === "Published" ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-700"}`}>
                    {a.status}
                  </span>
                </td>
                <td className="px-4 py-3 text-right">
                  <div className="flex justify-end gap-3">
                    <Link href={`/dashboard/teacher/assignments/${a.id}/submissions`} className="text-gray-600 hover:text-black" title="View submissions">
                      <Eye size={16} />
                    </Link>
                    {a.status === "Draft" && (
                      <button onClick={() => publishMutation.mutate(a.id)} className="text-blue-600 hover:text-blue-800" title="Publish">
                        <Send size={16} />
                      </button>
                    )}
                    <button onClick={() => openEdit(a)} className="text-gray-600 hover:text-black" title="Edit">
                      <Pencil size={16} />
                    </button>
                    <button onClick={() => deleteMutation.mutate(a.id)} className="text-red-600 hover:text-red-800" title="Delete">
                      <Trash2 size={16} />
                    </button>
                  </div>
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
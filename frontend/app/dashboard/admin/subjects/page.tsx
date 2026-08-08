"use client"

import { useState } from "react"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Plus, Pencil, Trash2 } from "lucide-react"
import { getSubjects, createSubject, updateSubject, deleteSubject, Subject } from "@/lib/api/admin"
import { Pagination } from "@/components/ui/Pagination"

export default function SubjectsPage() {
  const [page, setPage] = useState(1)
  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<Subject | null>(null)
  const [name, setName] = useState("")
  const [code, setCode] = useState("")
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ["admin-subjects", page],
    queryFn: () => getSubjects(page),
  })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["admin-subjects"] })

  const createMutation = useMutation({
    mutationFn: createSubject,
    onSuccess: () => {
      toast.success("Subject created.")
      invalidate()
      closeForm()
    },
    onError: () => toast.error("Failed to create subject."),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: { name: string; code: string } }) =>
      updateSubject(id, payload),
    onSuccess: () => {
      toast.success("Subject updated.")
      invalidate()
      closeForm()
    },
    onError: () => toast.error("Failed to update subject."),
  })

  const deleteMutation = useMutation({
    mutationFn: deleteSubject,
    onSuccess: () => {
      toast.success("Subject deleted.")
      invalidate()
    },
    onError: () => toast.error("Can't delete — this subject is still linked to a class."),
  })

  const openCreate = () => {
    setEditing(null)
    setName("")
    setCode("")
    setFormOpen(true)
  }

  const openEdit = (s: Subject) => {
    setEditing(s)
    setName(s.name)
    setCode(s.code)
    setFormOpen(true)
  }

  const closeForm = () => {
    setFormOpen(false)
    setEditing(null)
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const payload = { name, code }
    if (editing) {
      updateMutation.mutate({ id: editing.id, payload })
    } else {
      createMutation.mutate(payload)
    }
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Subjects</h1>
        <button
          onClick={openCreate}
          className="flex items-center gap-2 px-4 py-2 rounded-lg bg-black text-white text-sm font-medium hover:bg-gray-800"
        >
          <Plus size={16} />
          Add subject
        </button>
      </div>

      {formOpen && (
        <form onSubmit={handleSubmit} className="bg-white border rounded-xl p-4 mb-6 space-y-3 max-w-md">
          <div>
            <label className="text-sm font-medium">Name</label>
            <input
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              className="w-full mt-1 border rounded-lg px-3 py-2 text-sm"
              placeholder="Mathematics"
            />
          </div>
          <div>
            <label className="text-sm font-medium">Code</label>
            <input
              value={code}
              onChange={(e) => setCode(e.target.value)}
              required
              className="w-full mt-1 border rounded-lg px-3 py-2 text-sm"
              placeholder="MATH101"
            />
          </div>
          <div className="flex gap-2">
            <button
              type="submit"
              disabled={createMutation.isPending || updateMutation.isPending}
              className="px-4 py-2 rounded-lg bg-black text-white text-sm font-medium disabled:opacity-50"
            >
              {editing ? "Save changes" : "Create"}
            </button>
            <button
              type="button"
              onClick={closeForm}
              className="px-4 py-2 rounded-lg border text-sm font-medium"
            >
              Cancel
            </button>
          </div>
        </form>
      )}

      <div className="bg-white rounded-xl border overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b text-left text-muted-foreground">
              <th className="px-4 py-3 font-medium">Name</th>
              <th className="px-4 py-3 font-medium">Code</th>
              <th className="px-4 py-3 font-medium text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && (
              <tr>
                <td colSpan={3} className="px-4 py-8 text-center text-muted-foreground">
                  Loading...
                </td>
              </tr>
            )}
            {!isLoading && data?.items.length === 0 && (
              <tr>
                <td colSpan={3} className="px-4 py-8 text-center text-muted-foreground">
                  No subjects yet.
                </td>
              </tr>
            )}
            {data?.items.map((s) => (
              <tr key={s.id} className="border-b last:border-0">
                <td className="px-4 py-3">{s.name}</td>
                <td className="px-4 py-3">{s.code}</td>
                <td className="px-4 py-3 text-right">
                  <div className="flex justify-end gap-3">
                    <button onClick={() => openEdit(s)} className="text-gray-600 hover:text-black">
                      <Pencil size={16} />
                    </button>
                    <button
                      onClick={() => deleteMutation.mutate(s.id)}
                      className="text-red-600 hover:text-red-800"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {data && (
        <Pagination
          page={data.page}
          totalPages={data.totalPages}
          hasPrevious={data.hasPrevious}
          hasNext={data.hasNext}
          onPageChange={setPage}
        />
      )}
    </div>
  )
}
import { apiClient } from "./api-client"

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

// --- Users ---

export interface AdminUser {
  id: string
  fullName: string
  email: string
  role: "Admin" | "Teacher" | "Student"
  isActive: boolean
  createdAt: string
}

export const getUsers = async (page: number, role?: string, isActive?: boolean) => {
  const params = new URLSearchParams({ page: String(page) })
  if (role) params.set("role", role)
  if (isActive !== undefined) params.set("isActive", String(isActive))
  const { data } = await apiClient.get<PagedResult<AdminUser>>(`/admin/users?${params}`)
  return data
}
export const deactivateUser = (id: string) => apiClient.patch(`/admin/users/${id}/deactivate`)
export const reactivateUser = (id: string) => apiClient.patch(`/admin/users/${id}/reactivate`)
export const getTeachersLookup = async () => (await apiClient.get<AdminUser[]>("/admin/users/lookup/teachers")).data
export const getStudentsLookup = async () => (await apiClient.get<AdminUser[]>("/admin/users/lookup/students")).data

// --- Class Courses ---

export interface ClassCourse { id: string; name: string; section: string | null }

export const getClassCourses = async (page: number) =>
  (await apiClient.get<PagedResult<ClassCourse>>(`/admin/class-courses?page=${page}`)).data
export const createClassCourse = async (payload: { name: string; section?: string }) =>
  (await apiClient.post<ClassCourse>("/admin/class-courses", payload)).data
export const updateClassCourse = async (id: string, payload: { name: string; section?: string }) =>
  (await apiClient.put<ClassCourse>(`/admin/class-courses/${id}`, payload)).data
export const deleteClassCourse = (id: string) => apiClient.delete(`/admin/class-courses/${id}`)
export const getClassCoursesLookup = async () => (await apiClient.get<ClassCourse[]>("/admin/class-courses/lookup")).data

// --- Subjects ---

export interface Subject { id: string; name: string; code: string }

export const getSubjects = async (page: number) =>
  (await apiClient.get<PagedResult<Subject>>(`/admin/subjects?page=${page}`)).data
export const createSubject = async (payload: { name: string; code: string }) =>
  (await apiClient.post<Subject>("/admin/subjects", payload)).data
export const updateSubject = async (id: string, payload: { name: string; code: string }) =>
  (await apiClient.put<Subject>(`/admin/subjects/${id}`, payload)).data
export const deleteSubject = (id: string) => apiClient.delete(`/admin/subjects/${id}`)
export const getSubjectsLookup = async () => (await apiClient.get<Subject[]>("/admin/subjects/lookup")).data

// --- Class-Subjects ---

export interface ClassSubject {
  id: string; classCourseId: string; classCourseName: string
  subjectId: string; subjectName: string; subjectCode: string
}

export const getClassSubjects = async (page: number, classCourseId?: string) => {
  const params = new URLSearchParams({ page: String(page) })
  if (classCourseId) params.set("classCourseId", classCourseId)
  return (await apiClient.get<PagedResult<ClassSubject>>(`/admin/class-subjects?${params}`)).data
}
export const createClassSubject = async (payload: { classCourseId: string; subjectId: string }) =>
  (await apiClient.post<ClassSubject>("/admin/class-subjects", payload)).data
export const deleteClassSubject = (id: string) => apiClient.delete(`/admin/class-subjects/${id}`)
export const getClassSubjectsLookup = async () => (await apiClient.get<ClassSubject[]>("/admin/class-subjects/lookup")).data

// --- Teacher Assignments ---

export interface TeacherAssignment {
  id: string; teacherId: string; teacherName: string; teacherEmail: string
  classSubjectId: string; classCourseName: string; subjectName: string
}

export const getTeacherAssignments = async (page: number, teacherId?: string) => {
  const params = new URLSearchParams({ page: String(page) })
  if (teacherId) params.set("teacherId", teacherId)
  return (await apiClient.get<PagedResult<TeacherAssignment>>(`/admin/teacher-assignments?${params}`)).data
}
export const createTeacherAssignment = async (payload: { teacherId: string; classSubjectId: string }) =>
  (await apiClient.post<TeacherAssignment>("/admin/teacher-assignments", payload)).data
export const deleteTeacherAssignment = (id: string) => apiClient.delete(`/admin/teacher-assignments/${id}`)

// --- Enrollments ---

export interface Enrollment {
  id: string; studentId: string; studentName: string; studentEmail: string
  classCourseId: string; classCourseName: string; enrolledAt: string
}

export const getEnrollments = async (page: number, classCourseId?: string) => {
  const params = new URLSearchParams({ page: String(page) })
  if (classCourseId) params.set("classCourseId", classCourseId)
  return (await apiClient.get<PagedResult<Enrollment>>(`/admin/enrollments?${params}`)).data
}
export const createEnrollment = async (payload: { studentId: string; classCourseId: string }) =>
  (await apiClient.post<Enrollment>("/admin/enrollments", payload)).data
export const deleteEnrollment = (id: string) => apiClient.delete(`/admin/enrollments/${id}`)

// --- Read-only: Assignments & Submissions ---

export interface AdminAssignment {
  id: string; title: string; classCourseName: string; subjectName: string
  teacherName: string; deadline: string; status: string
}
export interface AdminSubmission {
  id: string; assignmentTitle: string; studentName: string
  submittedAt: string; status: string; marks: number | null
}

export const getAllAssignments = async (page: number) =>
  (await apiClient.get<PagedResult<AdminAssignment>>(`/admin/assignments?page=${page}`)).data
export const getAllSubmissions = async (page: number) =>
  (await apiClient.get<PagedResult<AdminSubmission>>(`/admin/submissions?page=${page}`)).data
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

export interface TeacherAssignment {
  id: string
  title: string
  description: string
  deadline: string
  maxMarks: number
  status: "Draft" | "Published" | "Closed"
  classSubjectId: string
  classCourseName: string
  subjectName: string
}

export interface TeacherClassSubject {
  id: string
  classCourseName: string
  subjectName: string
}

export interface CreateAssignmentPayload {
  title: string
  description: string
  deadline: string
  maxMarks: number
  classSubjectId: string
  status: "Draft" | "Published"
}

export interface UpdateAssignmentPayload {
  title: string
  description: string
  deadline: string
  maxMarks: number
}

export const getAssignments = async (page: number) =>
  (await apiClient.get<PagedResult<TeacherAssignment>>(`/teacher/assignments?page=${page}`)).data

export const getAssignmentById = async (id: string) =>
  (await apiClient.get<TeacherAssignment>(`/teacher/assignments/${id}`)).data

export const createAssignment = async (payload: CreateAssignmentPayload) =>
  (await apiClient.post<TeacherAssignment>("/teacher/assignments", payload)).data

export const updateAssignment = async (id: string, payload: UpdateAssignmentPayload) =>
  (await apiClient.put<TeacherAssignment>(`/teacher/assignments/${id}`, payload)).data

export const deleteAssignment = (id: string) => apiClient.delete(`/teacher/assignments/${id}`)

export const publishAssignment = async (id: string) =>
  (await apiClient.patch<TeacherAssignment>(`/teacher/assignments/${id}/publish`)).data

export const getClassSubjectsLookup = async () =>
  (await apiClient.get<TeacherClassSubject[]>("/teacher/class-subjects")).data

export interface TeacherSubmission {
  id: string
  assignmentId: string
  assignmentTitle: string
  studentId: string
  studentName: string
  studentEmail: string
  answerText: string
  attachmentUrl: string | null
  submittedAt: string
  status: "Submitted" | "Late" | "UnderReview" | "Graded" | "ResubmissionRequested"
  marks: number | null
  feedback: string | null
  gradedAt: string | null
}

export const getSubmissionsForAssignment = async (assignmentId: string, page: number) =>
  (await apiClient.get<PagedResult<TeacherSubmission>>(`/teacher/assignments/${assignmentId}/submissions?page=${page}`)).data

export const gradeSubmission = async (id: string, payload: { marks: number; feedback?: string }) =>
  (await apiClient.patch<TeacherSubmission>(`/teacher/submissions/${id}/grade`, payload)).data

export const updateSubmissionStatus = async (id: string, payload: { status: string }) =>
  (await apiClient.patch<TeacherSubmission>(`/teacher/submissions/${id}/status`, payload)).data
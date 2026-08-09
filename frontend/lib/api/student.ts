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

export interface StudentAssignment {
  id: string
  title: string
  description: string
  deadline: string
  maxMarks: number
  classCourseName: string
  subjectName: string
  teacherName: string
  hasSubmitted: boolean
  isPastDeadline: boolean
}

export interface StudentSubmission {
  id: string
  assignmentId: string
  assignmentTitle: string
  assignmentDeadline: string
  assignmentMaxMarks: number
  answerText: string
  attachmentUrl: string | null
  submittedAt: string
  status: string
  marks: number | null
  feedback: string | null
  gradedAt: string | null
  canUpdate: boolean
}

export const getAssignments = async (page: number) =>
  (await apiClient.get<PagedResult<StudentAssignment>>(`/student/assignments?page=${page}`)).data

export const getAssignmentById = async (id: string) =>
  (await apiClient.get<StudentAssignment>(`/student/assignments/${id}`)).data

export const getMySubmissions = async (page: number) =>
  (await apiClient.get<PagedResult<StudentSubmission>>(`/student/submissions?page=${page}`)).data

export const getSubmissionById = async (id: string) =>
  (await apiClient.get<StudentSubmission>(`/student/submissions/${id}`)).data

export const submitAssignment = async (assignmentId: string, payload: { answerText: string; attachmentUrl?: string }) =>
  (await apiClient.post<StudentSubmission>(`/student/assignments/${assignmentId}/submit`, payload)).data

export const updateSubmission = async (id: string, payload: { answerText: string; attachmentUrl?: string }) =>
  (await apiClient.put<StudentSubmission>(`/student/submissions/${id}`, payload)).data
import { apiClient } from "./api-client"

export interface CurrentUser {
  userId: string
  fullName: string
  email: string
  role: "Admin" | "Teacher" | "Student"
}

export async function getCurrentUser(): Promise<CurrentUser> {
  const { data } = await apiClient.get<CurrentUser>("/auth/me")
  return data
}
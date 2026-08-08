import { apiClient } from "./api-client"
import { backendClient } from "./backend-client"

export type UserRole = "Teacher" | "Student"

export interface RegisterPayload {
  fullName: string
  email: string
  password: string
  role: UserRole
}

export interface RegisterResponse {
  userId: string
  fullName: string
  email: string
  role: string
}

export async function registerUser(payload: RegisterPayload): Promise<RegisterResponse> {
  const { data } = await backendClient.post<RegisterResponse>("/api/auth/register", payload)
  return data
}

export interface LoginPayload {
  email: string
  password: string
}

export interface LoginResponse {
  userId: string
  fullName: string
  email: string
  role: string
}

export async function loginUser(payload: LoginPayload): Promise<LoginResponse> {
  const { data } = await apiClient.post<LoginResponse>("/auth/login", payload)
  return data
}

export async function logoutUser(): Promise<void> {
  await apiClient.post("/auth/logout")
}
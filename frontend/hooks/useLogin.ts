import { useMutation } from "@tanstack/react-query"
import { AxiosError } from "axios"
import { loginUser, LoginPayload, LoginResponse } from "@/lib/api/auth"

export interface ApiErrorResponse {
  title?: string
  status?: number
  message?: string
  errors?: Record<string, string[]>
}

export function useLogin() {
  return useMutation<LoginResponse, AxiosError<ApiErrorResponse>, LoginPayload>({
    mutationFn: loginUser,
  })
}
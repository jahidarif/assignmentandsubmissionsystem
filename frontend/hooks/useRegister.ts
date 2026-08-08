import { useMutation } from "@tanstack/react-query"
import { AxiosError } from "axios"
import { registerUser, RegisterPayload, RegisterResponse } from "@/lib/api/auth"

// Matches the JSON shape your ExceptionHandlingMiddleware returns
// for ValidationException (400) and ConflictException (409).
export interface ApiErrorResponse {
  title?: string
  status?: number
  message?: string
  errors?: Record<string, string[]>
}

export function useRegister() {
  return useMutation<RegisterResponse, AxiosError<ApiErrorResponse>, RegisterPayload>({
    mutationFn: registerUser,
  })
}
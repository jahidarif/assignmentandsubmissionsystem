"use client"

import { useEffect } from "react"
import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { AxiosError } from "axios"
import { RegisterForm } from "@/components/auth/RegisterForm"
import { useRegister } from "@/hooks/useRegister"
import { useCurrentUser } from "@/hooks/useCurrentUser"
import type { ApiErrorResponse } from "@/hooks/useRegister"

const ROLE_REDIRECTS: Record<string, string> = {
  Admin: "/dashboard/admin",
  Teacher: "/dashboard/teacher",
  Student: "/dashboard/student",
}

export default function SignUpPage() {
  const router = useRouter()
  const { mutateAsync } = useRegister()
  const { data: currentUser, isLoading: checkingSession } = useCurrentUser()

  useEffect(() => {
    if (!checkingSession && currentUser) {
      router.replace(ROLE_REDIRECTS[currentUser.role] ?? "/dashboard")
    }
  }, [checkingSession, currentUser, router])

  const handleRegister = async (data: { fullName: string; email: string; password: string; role: "Teacher" | "Student" }) => {
    try {
      await mutateAsync(data)
      toast.success("Account created successfully! Please sign in.")
      router.push("/auth/sign-in?registered=true")
    } catch (err) {
      const axiosError = err as AxiosError<ApiErrorResponse>
      if (axiosError.response?.status === 409) { toast.error(axiosError.response.data?.message ?? "An account with this email already exists."); return }
      if (axiosError.response?.status === 400 && axiosError.response.data?.errors) {
        const firstError = Object.values(axiosError.response.data.errors)[0]?.[0]
        toast.error(firstError ?? "Please check your details and try again.")
        return
      }
      if (!axiosError.response) { toast.error("Could not reach the server. Is the backend running?"); return }
      toast.error("Something went wrong. Please try again.")
    }
  }

  if (checkingSession || currentUser) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <p className="text-muted-foreground text-sm">Loading...</p>
      </div>
    )
  }

  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <div className="w-full max-w-md">
        <RegisterForm onSubmit={handleRegister} />
      </div>
    </div>
  )
}
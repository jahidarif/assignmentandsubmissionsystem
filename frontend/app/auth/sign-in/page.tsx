"use client"

import { Suspense, useEffect } from "react"
import { useRouter, useSearchParams } from "next/navigation"
import { toast } from "sonner"
import { AxiosError } from "axios"
import { SignInForm } from "@/components/auth/SignInForm"
import { useLogin } from "@/hooks/useLogin"
import { useCurrentUser } from "@/hooks/useCurrentUser"
import type { ApiErrorResponse } from "@/hooks/useLogin"

const ROLE_REDIRECTS: Record<string, string> = {
  Admin: "/dashboard/admin",
  Teacher: "/dashboard/teacher",
  Student: "/dashboard/student",
}

function SignInContent() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const { mutateAsync } = useLogin()
  const { data: currentUser, isLoading: checkingSession } = useCurrentUser()

  // Already logged in — skip the form, go straight to the right dashboard.
  useEffect(() => {
    if (!checkingSession && currentUser) {
      router.replace(ROLE_REDIRECTS[currentUser.role] ?? "/dashboard")
    }
  }, [checkingSession, currentUser, router])

  useEffect(() => {
    if (searchParams.get("registered") === "true") toast.success("Account created! Please sign in.")
    const authError = searchParams.get("authError")
    if (authError) toast.error(authError)
  }, [searchParams])

  const handleLogin = async (data: { email: string; password: string }) => {
    try {
      const result = await mutateAsync(data)
      toast.success(`Welcome back, ${result.fullName}!`)
      router.push(ROLE_REDIRECTS[result.role] ?? "/dashboard")
    } catch (err) {
      const axiosError = err as AxiosError<ApiErrorResponse>
      if (axiosError.response?.status === 401) { toast.error("Invalid email or password."); return }
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
        <SignInForm onSubmit={handleLogin} />
      </div>
    </div>
  )
}

export default function SignInPage() {
  return (
    <Suspense fallback={null}>
      <SignInContent />
    </Suspense>
  )
}
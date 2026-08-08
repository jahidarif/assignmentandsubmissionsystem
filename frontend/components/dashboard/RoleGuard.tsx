"use client"

import { useEffect } from "react"
import { useRouter } from "next/navigation"
import { useCurrentUser } from "@/hooks/useCurrentUser"

const ROLE_HOME: Record<string, string> = {
  Admin: "/dashboard/admin",
  Teacher: "/dashboard/teacher",
  Student: "/dashboard/student",
}

export function RoleGuard({
  allowedRole,
  children,
}: {
  allowedRole: "Admin" | "Teacher" | "Student"
  children: React.ReactNode
}) {
  const router = useRouter()
  const { data: user, isLoading, isError } = useCurrentUser()

  useEffect(() => {
    if (isLoading) return

    if (isError || !user) {
      router.replace("/auth/sign-in")
      return
    }

    if (user.role !== allowedRole) {
      router.replace(ROLE_HOME[user.role] ?? "/auth/sign-in")
    }
  }, [isLoading, isError, user, allowedRole, router])

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <p className="text-muted-foreground text-sm">Checking your session...</p>
      </div>
    )
  }

  if (isError || !user || user.role !== allowedRole) {
    return null
  }

  return <>{children}</>
}
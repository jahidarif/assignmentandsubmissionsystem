"use client"

import { useRouter } from "next/navigation"
import { Menu, LogOut } from "lucide-react"
import { apiClient } from "@/lib/api/api-client"
import { useCurrentUser } from "@/hooks/useCurrentUser"

export function TopBar({ onMenuClick }: { onMenuClick?: () => void }) {
  const router = useRouter()
  const { data: user } = useCurrentUser()

  const handleLogout = async () => {
    await apiClient.post("/auth/logout")
    router.push("/auth/sign-in")
  }

  return (
    <header className="flex items-center justify-between px-4 md:px-6 py-3 border-b bg-card">
      <div className="flex items-center gap-3">
        {onMenuClick && (
          <button onClick={onMenuClick} className="md:hidden">
            <Menu size={22} />
          </button>
        )}
        <p className="font-medium text-sm md:text-base">
          Hello, <span className="font-semibold">{user?.fullName ?? "..."}</span>
        </p>
      </div>
      <button
        onClick={handleLogout}
        className="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm font-medium text-red-600 hover:bg-red-50 transition-colors"
      >
        <LogOut size={16} />
        <span className="hidden sm:inline">Log out</span>
      </button>
    </header>
  )
}
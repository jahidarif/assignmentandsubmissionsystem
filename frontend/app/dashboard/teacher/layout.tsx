"use client"
import { RoleGuard } from "@/components/dashboard/RoleGuard"
import { DashboardLayout } from "@/components/dashboard/DashboardLayout"
import { ClipboardList } from "lucide-react"

const TEACHER_NAV_ITEMS = [
  { href: "/dashboard/teacher", label: "Assignments", icon: ClipboardList },
]

export default function TeacherLayout({ children }: { children: React.ReactNode }) {
  return (
    <RoleGuard allowedRole="Teacher">
      <DashboardLayout navItems={TEACHER_NAV_ITEMS} panelTitle="Teacher Panel">
        {children}
      </DashboardLayout>
    </RoleGuard>
  )
}
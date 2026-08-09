"use client"

import { RoleGuard } from "@/components/dashboard/RoleGuard"
import { DashboardLayout } from "@/components/dashboard/DashboardLayout"
import { ClipboardList, FileCheck2 } from "lucide-react"

const STUDENT_NAV_ITEMS = [
  { href: "/dashboard/student", label: "Assignments", icon: ClipboardList },
  { href: "/dashboard/student/submissions", label: "My Submissions", icon: FileCheck2 },
]

export default function StudentLayout({ children }: { children: React.ReactNode }) {
  return (
    <RoleGuard allowedRole="Student">
      <DashboardLayout navItems={STUDENT_NAV_ITEMS} panelTitle="Student Panel">
        {children}
      </DashboardLayout>
    </RoleGuard>
  )
}
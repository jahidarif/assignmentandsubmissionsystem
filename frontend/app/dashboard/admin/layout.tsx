"use client"

import { RoleGuard } from "@/components/dashboard/RoleGuard"
import { DashboardLayout } from "@/components/dashboard/DashboardLayout"
import { Users, GraduationCap, BookOpen, Link2, UserCog, ClipboardList, FileCheck2 } from "lucide-react"

const ADMIN_NAV_ITEMS = [
  { href: "/dashboard/admin", label: "Users", icon: Users },
  { href: "/dashboard/admin/class-courses", label: "Classes", icon: GraduationCap },
  { href: "/dashboard/admin/subjects", label: "Subjects", icon: BookOpen },
  { href: "/dashboard/admin/class-subjects", label: "Class Subjects", icon: Link2 },
  { href: "/dashboard/admin/teacher-assignments", label: "Teacher Assignments", icon: UserCog },
  { href: "/dashboard/admin/enrollments", label: "Enrollments", icon: ClipboardList },
  { href: "/dashboard/admin/assignments", label: "Assignments", icon: ClipboardList },
  { href: "/dashboard/admin/submissions", label: "Submissions", icon: FileCheck2 },
]

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  return (
    <RoleGuard allowedRole="Admin">
      <DashboardLayout navItems={ADMIN_NAV_ITEMS} panelTitle="Admin Panel">
        {children}
      </DashboardLayout>
    </RoleGuard>
  )
}
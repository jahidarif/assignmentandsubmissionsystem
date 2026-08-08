"use client"

import { useState } from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { X, Users, BookOpen, GraduationCap, Link2, UserCog, ClipboardList, FileCheck2 } from "lucide-react"
import { TopBar } from "./TopBar"

const NAV_ITEMS = [
  { href: "/dashboard/admin", label: "Users", icon: Users },
  { href: "/dashboard/admin/class-courses", label: "Classes", icon: GraduationCap },
  { href: "/dashboard/admin/subjects", label: "Subjects", icon: BookOpen },
  { href: "/dashboard/admin/class-subjects", label: "Class Subjects", icon: Link2 },
  { href: "/dashboard/admin/teacher-assignments", label: "Teacher Assignments", icon: UserCog },
  { href: "/dashboard/admin/enrollments", label: "Enrollments", icon: ClipboardList },
  { href: "/dashboard/admin/assignments", label: "Assignments", icon: ClipboardList },
  { href: "/dashboard/admin/submissions", label: "Submissions", icon: FileCheck2 },
]

export function DashboardLayout({ children }: { children: React.ReactNode }) {
  const [drawerOpen, setDrawerOpen] = useState(false)
  const pathname = usePathname()

  const NavLinks = () => (
    <nav className="space-y-1">
      {NAV_ITEMS.map(({ href, label, icon: Icon }) => {
        const active = pathname === href
        return (
          <Link
            key={href}
            href={href}
            onClick={() => setDrawerOpen(false)}
            className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
              active ? "bg-foreground text-background" : "text-foreground/70 hover:bg-muted"
            }`}
          >
            <Icon size={18} />
            {label}
          </Link>
        )
      })}
    </nav>
  )

  return (
    <div className="min-h-screen flex bg-background text-foreground">
      <aside className="hidden md:flex md:flex-col md:w-64 border-r bg-card px-4 py-6">
        <p className="font-semibold text-lg mb-8 px-2">Admin Panel</p>
        <NavLinks />
      </aside>

      {drawerOpen && (
        <div className="fixed inset-0 z-40 md:hidden">
          <div className="absolute inset-0 bg-black/40" onClick={() => setDrawerOpen(false)} />
          <aside className="absolute left-0 top-0 bottom-0 w-64 bg-card px-4 py-6 flex flex-col">
            <div className="flex items-center justify-between mb-8 px-2">
              <p className="font-semibold text-lg">Admin Panel</p>
              <button onClick={() => setDrawerOpen(false)}>
                <X size={20} />
              </button>
            </div>
            <NavLinks />
          </aside>
        </div>
      )}

      <div className="flex-1 flex flex-col min-w-0">
        <TopBar onMenuClick={() => setDrawerOpen(true)} />
        <main className="flex-1 p-4 md:p-8 overflow-x-hidden bg-background">{children}</main>
      </div>
    </div>
  )
}
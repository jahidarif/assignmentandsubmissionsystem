"use client"

import { useState } from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { X } from "lucide-react"
import { TopBar } from "./TopBar"
import type { LucideIcon } from "lucide-react"

export interface NavItem {
  href: string
  label: string
  icon: LucideIcon
}

interface DashboardLayoutProps {
  children: React.ReactNode
  navItems: NavItem[]
  panelTitle: string
}

export function DashboardLayout({ children, navItems, panelTitle }: DashboardLayoutProps) {
  const [drawerOpen, setDrawerOpen] = useState(false)
  const pathname = usePathname()

  const NavLinks = () => (
    <nav className="space-y-1">
      {navItems.map(({ href, label, icon: Icon }) => {
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
        <p className="font-semibold text-lg mb-8 px-2">{panelTitle}</p>
        <NavLinks />
      </aside>

      {drawerOpen && (
        <div className="fixed inset-0 z-40 md:hidden">
          <div className="absolute inset-0 bg-black/40" onClick={() => setDrawerOpen(false)} />
          <aside className="absolute left-0 top-0 bottom-0 w-64 bg-card px-4 py-6 flex flex-col">
            <div className="flex items-center justify-between mb-8 px-2">
              <p className="font-semibold text-lg">{panelTitle}</p>
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
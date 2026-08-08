import { RoleGuard } from "@/components/dashboard/RoleGuard"
import { DashboardLayout } from "@/components/dashboard/DashboardLayout"

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  return (
    <RoleGuard allowedRole="Admin">
      <DashboardLayout>{children}</DashboardLayout>
    </RoleGuard>
  )
}
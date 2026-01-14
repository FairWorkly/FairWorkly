import { useEffect, useCallback } from 'react'
import { Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '@/shared/hooks/useAuth'
import { Sidebar } from './Sidebar'
import { AppShell, AppMain, AppContent } from './MainLayout.styles'

export function MainLayout() {
  const navigate = useNavigate()
  const { user, switchRole, logout } = useAuth()

  const handleLogout = useCallback(() => {
    logout()
    navigate('/login', { replace: true })
  }, [logout, navigate])

  // DEV ONLY: Expose switchRole to window for easy testing
  useEffect(() => {
    if (import.meta.env.DEV) {
      ;(
        window as typeof window & { switchRole: typeof switchRole }
      ).switchRole = switchRole
      console.log(
        '🔧 DEV: Use window.switchRole("admin") or window.switchRole("manager") to test role switching'
      )
    }
  }, [switchRole])

  return (
    <AppShell>
      <Sidebar user={user ?? undefined} onLogout={handleLogout} />
      <AppMain>
        <AppContent>
          <Outlet />
        </AppContent>
      </AppMain>
    </AppShell>
  )
}

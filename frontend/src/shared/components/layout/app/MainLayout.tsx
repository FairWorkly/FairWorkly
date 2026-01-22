import { useEffect, useCallback, useState } from 'react'
import { IconButton, useMediaQuery } from '@mui/material'
import { useTheme } from '@mui/material/styles'
import MenuOutlinedIcon from '@mui/icons-material/MenuOutlined'
import { Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '@/modules/auth'
import { Sidebar } from './Sidebar'
import {
  AppShell,
  AppMain,
  AppContent,
  AppTopBar,
  AppTopBarInner,
} from './MainLayout.styles'

export function MainLayout() {
  const theme = useTheme()
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'))
  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false)
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
      <Sidebar
        user={user ?? undefined}
        onLogout={handleLogout}
        variant={isMobile ? 'temporary' : 'permanent'}
        open={isMobile ? mobileSidebarOpen : true}
        onClose={() => setMobileSidebarOpen(false)}
      />
      <AppMain>
        <AppTopBar>
          <AppTopBarInner>
            <IconButton
              aria-label="Open navigation"
              onClick={() => setMobileSidebarOpen(true)}
              size="small"
            >
              <MenuOutlinedIcon />
            </IconButton>
          </AppTopBarInner>
        </AppTopBar>
        <AppContent>
          <Outlet />
        </AppContent>
      </AppMain>
    </AppShell>
  )
}

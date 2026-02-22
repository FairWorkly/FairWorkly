import { useCallback, useState } from 'react'
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
  const { user, logout } = useAuth()

  const handleLogout = useCallback(() => {
    logout()
    navigate('/login', { replace: true })
  }, [logout, navigate])

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


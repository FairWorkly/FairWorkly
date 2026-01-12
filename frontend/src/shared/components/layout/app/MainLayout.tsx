import { Outlet } from 'react-router-dom'
import { Box } from '@mui/material'
import { styled } from '@mui/material/styles'
import { Sidebar } from './Sidebar'

const SIDEBAR_WIDTH = 280

const AppShell = styled('div')(() => ({
  display: 'flex',
  minHeight: '100vh',
}))

const MainArea = styled('main')(({ theme }) => ({
  flex: 1,
  minWidth: 0,
  backgroundColor: theme.palette.background.default,
  padding: theme.spacing(4),
}))

export function MainLayout() {
  return (
    <AppShell>
      <Sidebar width={SIDEBAR_WIDTH} />

      {/* 右侧内容区 */}
      <MainArea aria-label="Main content">
        {/* 你页面里自己渲染 breadcrumb / title */}
        <Box sx={{ maxWidth: 1200 }}>
          <Outlet />
        </Box>
      </MainArea>
    </AppShell>
  )
}

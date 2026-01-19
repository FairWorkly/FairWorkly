import { styled } from '@/styles/styled'

export const AppShell = styled('div')(() => ({
  height: '100vh',
  display: 'flex',
  overflow: 'hidden',
}))

export const AppMain = styled('main')(({ theme }) => ({
  flex: 1,
  minWidth: 0,
  display: 'flex',
  flexDirection: 'column',
  background: theme.palette.background.default,
  overflow: 'hidden',
}))

export const AppContent = styled('div')(() => ({
  flex: 1,
  display: 'flex',
  flexDirection: 'column',
  minHeight: 0,
  width: '100%',
  overflowY: 'auto',
}))

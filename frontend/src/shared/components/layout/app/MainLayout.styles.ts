import { styled } from '@/styles/styled'

export const AppShell = styled('div')(() => ({
  minHeight: '100vh',
  display: 'flex',
}))

export const AppMain = styled('main')(({ theme }) => ({
  flex: 1,
  minWidth: 0,
  background: theme.palette.background.default,
  overflow: 'auto',
}))

export const AppContent = styled('div')(({ theme }) => ({
  padding: theme.spacing(3),
  minHeight: '100%',
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
}))

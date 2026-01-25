import { styled } from '@/styles/styled'

export const AppShell = styled('div')(() => ({
  height: '100dvh',
  display: 'flex',
  overflow: 'hidden',
}))

export const AppMain = styled('main')(({ theme }) => ({
  flex: 1,
  minWidth: 0,
  minHeight: 0,
  display: 'flex',
  flexDirection: 'column',
  overflow: 'hidden',
  background: theme.palette.background.default,
}))

export const AppTopBar = styled('header')(({ theme }) => ({
  display: 'none',
  position: 'sticky',
  top: 0,
  zIndex: 1100,
  background: theme.palette.background.default,
  borderBottom: `1px solid ${theme.palette.divider}`,
  [theme.breakpoints.down('sm')]: {
    display: 'block',
  },
}))

export const AppTopBarInner = styled('div')(({ theme }) => ({
  padding: theme.spacing(1.5, 2),
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
}))

export const AppContent = styled('section')(({ theme }) => ({
  flex: 1,
  minHeight: 0,
  width: '100%',
  overflowY: 'auto',
  padding: theme.spacing(2),
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(3),
  },
  [theme.breakpoints.up('md')]: {
    padding: theme.spacing(4),
  },
}))

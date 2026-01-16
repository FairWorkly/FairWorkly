import { alpha } from '@mui/material/styles'
import { styled } from '@/styles/styled'

export const PublicShell = styled('div')(({ theme }) => ({
  minHeight: '100vh',
  display: 'flex',
  backgroundColor: theme.palette.background.default,
  overflowX: 'hidden',
}))

export const PublicBranding = styled('aside')(({ theme }) => ({
  flex: 1,
  position: 'relative',
  background: theme.fairworkly.surface.navDark,
  padding: theme.spacing(3),
  display: 'flex',
  flexDirection: 'column',
  justifyContent: 'space-between',
  overflow: 'hidden',

  '&::before': {
    content: '""',
    position: 'absolute',
    top: '-50%',
    right: '-50%',
    width: '100%',
    height: '100%',
    background: `radial-gradient(circle, ${alpha(theme.palette.primary.main, 0.2)} 0%, transparent 60%)`,
    pointerEvents: 'none',
  },

  [theme.breakpoints.down('lg')]: {
    display: 'none',
  },
}))

export const PublicBrandingFooter = styled('footer')(({ theme }) => ({
  position: 'relative',
  zIndex: 1,
  color: alpha(theme.palette.common.white, 0.5),
  fontSize: theme.typography.body2.fontSize,
  fontWeight: theme.typography.subtitle2.fontWeight,
}))

/** split：右侧表单区域（居中窄容器） */
export const PublicSplitMain = styled('main')(({ theme }) => ({
  flex: 1,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(4),

  [theme.breakpoints.down('lg')]: {
    padding: theme.spacing(3),
  },
}))

export const PublicSplitContainer = styled('div')(() => ({
  width: '100%',
  maxWidth: 480,
}))

/** center：marketing 内容（不强行居中，交给页面自己排版） */
export const PublicCenterMain = styled('main')(() => ({
  flex: 1,
  width: '100%',
}))

export const PublicCenterContainer = styled('div')(({ theme }) => ({
  width: '100%',
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  paddingLeft: theme.spacing(3),
  paddingRight: theme.spacing(3),
}))



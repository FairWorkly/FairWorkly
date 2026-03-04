import { Box, Paper, Typography } from '@mui/material'
import { alpha } from '@mui/material/styles'
import { styled } from '@/styles/styled'

export const SettingsContainer = styled(Box)(({ theme }) => ({
  padding: theme.spacing(4),
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  width: '100%',
}))

export const SettingsLayout = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: `${theme.spacing(30)} 1fr`,
  gap: theme.spacing(3),
  alignItems: 'start',

  [theme.breakpoints.down('md')]: {
    gridTemplateColumns: '1fr',
  },
}))

export const SettingsNav = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(1.5),
  borderRadius: `${theme.fairworkly.radius.lg}px`,
  border: `${theme.spacing(0.125)} solid ${theme.palette.divider}`,
  position: 'sticky',
  top: theme.spacing(12),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),

  [theme.breakpoints.down('md')]: {
    position: 'static',
  },
}))

export const SettingsNavItem = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  padding: theme.spacing(1.5, 2),
  borderRadius: `${theme.fairworkly.radius.md}px`,
  color: theme.palette.text.secondary,
  cursor: 'pointer',
  transition: theme.transitions.create(['background', 'color'], {
    duration: theme.transitions.duration.short,
    easing: theme.transitions.easing.easeInOut,
  }),
  fontWeight: theme.typography.fontWeightMedium,

  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(2.5),
    color: theme.palette.text.secondary,
    transition: theme.transitions.create('color', {
      duration: theme.transitions.duration.short,
      easing: theme.transitions.easing.easeInOut,
    }),
  },

  '&:hover': {
    background: theme.palette.action.hover,
    color: theme.palette.text.primary,

    '& .MuiSvgIcon-root': {
      color: theme.palette.text.primary,
    },
  },

  '&.active': {
    background: alpha(theme.palette.primary.main, 0.12),
    color: theme.palette.primary.main,
    fontWeight: theme.typography.fontWeightBold,

    '& .MuiSvgIcon-root': {
      color: theme.palette.primary.main,
    },
  },
}))

export const SettingsNavItemText = styled(Typography)(() => ({
  fontWeight: 'inherit',
}))

export const SettingsContent = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(3),
}))

export const PageHeader = styled(Box)(({ theme }) => ({
  marginBottom: theme.spacing(4),
}))

export const SectionContent = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
}))

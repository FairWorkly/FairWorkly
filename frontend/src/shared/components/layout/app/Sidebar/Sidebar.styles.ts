import { Avatar, Box, Chip, Drawer, Paper, Typography } from '@mui/material'
import { alpha } from '@mui/material/styles'
import type { Theme } from '@mui/material/styles'
import type { DrawerProps } from '@mui/material/Drawer'
import { NavLink } from 'react-router-dom'
import { styled } from '@/styles/styled'

export const SidebarDrawer = styled(Drawer)(
  ({ theme, ownerState }: { theme: Theme; ownerState?: DrawerProps }) => ({
  width:
    ownerState?.variant === 'temporary'
      ? 0
      : theme.fairworkly.layout.sidebarWidth,
  flexShrink: 0,

  // sm-md 断点下收窄 sidebar 占位宽度
  [theme.breakpoints.between('sm', 'md')]: {
    width: ownerState?.variant === 'temporary' ? 0 : theme.spacing(30),
  },

  '& .MuiDrawer-paper': {
    width: theme.fairworkly.layout.sidebarWidth,
    boxSizing: 'border-box',
    borderRight: `1px solid ${theme.palette.divider}`,
    background: theme.palette.background.paper,

    [theme.breakpoints.between('sm', 'md')]: {
      width: theme.spacing(30),
    },

    [theme.breakpoints.down('sm')]: {
      width: '80vw',
      maxWidth: theme.spacing(40),
    },
  },

  // permanent/docked 模式下使用相对定位
  '&.MuiDrawer-docked .MuiDrawer-paper': {
    position: 'relative',
  },
}))

export const SidebarPaper = styled(Box)(({ theme }) => ({
  height: '100%',
  minHeight: 0,
  display: 'flex',
  flexDirection: 'column',
  overflow: 'hidden',

  '& nav': {
    paddingTop: theme.spacing(1),
    paddingBottom: theme.spacing(1),
  },
}))

export const BrandRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  padding: theme.spacing(4, 4, 2),
  borderBottom: `${theme.spacing(0.125)} solid ${theme.palette.divider}`,
}))

export const BrandTitle = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.fontWeightBold,
  letterSpacing: '-0.01em',
}))

export const LogoBadge = styled(Box)(({ theme }) => ({
  width: theme.spacing(4.5),
  height: theme.spacing(4.5),
  borderRadius: `${theme.fairworkly.radius.md}px`,
  display: 'grid',
  placeItems: 'center',
  color: theme.palette.primary.contrastText,
  background: theme.palette.primary.main,
  boxShadow: theme.fairworkly.shadow.md,

  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(2.5),
  },
}))

export const SectionLabel = styled(Box)(({ theme }) => ({
  padding: theme.spacing(1.5, 2, 1),
  color: theme.palette.text.disabled,
}))

export const SectionTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  letterSpacing: '0.15em',
}))

export const NavItemButton = styled(NavLink)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  textDecoration: 'none',

  margin: theme.spacing(0.125, 2),
  padding: theme.spacing(1.5, 2),
  borderRadius: `${theme.fairworkly.radius.xl}px`,
  color: theme.palette.text.secondary,
  transition: theme.transitions.create(['background', 'color'], {
    duration: theme.transitions.duration.short,
    easing: theme.transitions.easing.easeInOut,
  }),

  '& .MuiListItemIcon-root': {
    minWidth: 'unset',
    color: alpha(theme.palette.text.secondary, 0.6),
    transition: theme.transitions.create('color', {
      duration: theme.transitions.duration.short,
      easing: theme.transitions.easing.easeInOut,
    }),

    '& .MuiSvgIcon-root': {
      fontSize: theme.spacing(2.5),
    },
  },

  '& .MuiTypography-subtitle2': {
    fontWeight: theme.typography.fontWeightBold,
    fontSize: theme.typography.body2.fontSize,
  },

  '&:hover': {
    background: alpha(theme.palette.divider, 0.5),
    color: theme.palette.primary.main,

    '& .MuiListItemIcon-root': {
      color: theme.palette.primary.main,
    },
  },

  '&.active': {
    background: theme.palette.primary.main,
    color: theme.palette.primary.contrastText,
    boxShadow: `${theme.spacing(0)} ${theme.spacing(0.5)} ${theme.spacing(1)} ${alpha(theme.palette.primary.main, 0.1)}`,

    '& .MuiListItemIcon-root': {
      color: theme.palette.primary.contrastText,
    },
  },
}))

export const NavItemText = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.fontWeightMedium,
  fontSize: theme.typography.body2.fontSize,
}))

export const FairBotCard = styled(Paper)(({ theme }) => ({
  margin: theme.spacing(3, 2, 3),
  padding: theme.spacing(3),
  borderRadius: `${theme.fairworkly.radius.xl}px`,
  background: alpha(theme.palette.background.paper, 0.6),
  border: `${theme.spacing(0.125)} solid ${theme.palette.divider}`,
  boxShadow: theme.fairworkly.shadow.sm,
  cursor: 'pointer',
  transition: theme.transitions.create(['border-color', 'box-shadow'], {
    duration: theme.transitions.duration.short,
  }),

  display: 'grid',
  gridTemplateColumns: `${theme.spacing(6)} 1fr`,
  gridTemplateRows: 'auto auto',
  columnGap: theme.spacing(2),
  rowGap: theme.spacing(2),
  alignItems: 'start',

  '&:hover': {
    borderColor: alpha(theme.palette.primary.main, 0.3),
  },
}))

export const FairBotIcon = styled(Box)(({ theme }) => ({
  width: theme.spacing(6),
  height: theme.spacing(6),
  borderRadius: theme.spacing(1.5),
  display: 'grid',
  placeItems: 'center',
  background: alpha(theme.palette.primary.main, 0.08),
  border: `${theme.spacing(0.125)} solid ${alpha(theme.palette.primary.main, 0.1)}`,
  color: theme.palette.primary.main,
  boxShadow: theme.fairworkly.shadow.sm,
  transition: theme.transitions.create(['background-color'], {
    duration: theme.transitions.duration.short,
  }),

  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(3),
  },

  [`${FairBotCard}:hover &`]: {
    background: theme.palette.background.paper,
  },
}))

export const FairBotHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),
  alignItems: 'flex-start',
}))

export const FairBotTitle = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.fontWeightBold,
  letterSpacing: '0.02em',
}))

export const FairBotStatusChip = styled(Chip)(({ theme }) => ({
  height: theme.spacing(2.5),
  background: 'transparent',
  color: theme.palette.success.main,
  padding: theme.spacing(0),
  fontWeight: theme.typography.fontWeightBold,
  fontSize: theme.typography.caption.fontSize,
  letterSpacing: '0.1em',

  '& .MuiChip-icon': {
    color: theme.palette.success.main,
    fontSize: theme.spacing(0.75),
    marginLeft: theme.spacing(0),
    marginRight: theme.spacing(0.75),
    animation: 'pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite',
  },

  '@keyframes pulse': {
    '0%, 100%': {
      opacity: 1,
    },
    '50%': {
      opacity: 0.5,
    },
  },
}))

export const FairBotBody = styled(Box)(({ theme }) => ({
  gridColumn: '1 / -1',
  minWidth: theme.spacing(0),
}))

export const FairBotDescription = styled(Typography)(({ theme }) => ({
  lineHeight: theme.typography.body2.lineHeight,
  wordBreak: 'normal',
  textWrap: 'pretty',
  fontWeight: theme.typography.fontWeightMedium,
}))

export const NavContainer = styled(Box)(({ theme }) => ({
  flex: 1,
  minHeight: 0,
  overflowY: 'auto',
  overflowX: 'hidden',
  paddingTop: theme.spacing(1),
  paddingBottom: theme.spacing(1),

  '&::-webkit-scrollbar': {
    width: theme.spacing(0.75),
  },

  '&::-webkit-scrollbar-track': {
    background: 'transparent',
  },

  '&::-webkit-scrollbar-thumb': {
    background: alpha(theme.palette.text.disabled, 0.2),
    borderRadius: `${theme.fairworkly.radius.sm}px`,

    '&:hover': {
      background: alpha(theme.palette.text.disabled, 0.3),
    },
  },
}))

export const Spacer = styled(Box)(() => ({
  flex: 1,
}))

export const BottomRow = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2, 2),
  borderTop: `${theme.spacing(0.125)} solid ${theme.palette.divider}`,
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
}))

export const UserAvatar = styled(Avatar)(({ theme }) => ({
  width: theme.spacing(4.5),
  height: theme.spacing(4.5),
  borderRadius: `${theme.fairworkly.radius.md}px`,
  background: theme.palette.primary.main,
  fontSize: theme.typography.body2.fontSize,
  fontWeight: theme.typography.fontWeightBold,
}))

export const UserMeta = styled(Box)(({ theme }) => ({
  flex: 1,
  minWidth: theme.spacing(0),
}))

export const UserName = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.fontWeightBold,
}))

export const UserRole = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.fontWeightMedium,
  letterSpacing: '0.05em',
}))

export const LogoutButton = styled('button')(({ theme }) => ({
  width: theme.spacing(5),
  height: theme.spacing(5),
  borderRadius: `${theme.fairworkly.radius.md}px`,
  border: 'none',
  background: 'transparent',
  color: theme.palette.text.secondary,
  cursor: 'pointer',
  display: 'grid',
  placeItems: 'center',
  transition: theme.transitions.create(['background-color', 'color'], {
    duration: theme.transitions.duration.short,
  }),

  '&:hover': {
    background: alpha(theme.palette.error.main, 0.08),
    color: theme.palette.error.main,
  },

  '&:active': {
    background: alpha(theme.palette.error.main, 0.12),
  },

  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(2.5),
  },
}))

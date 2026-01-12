import * as React from 'react'
import {
  Avatar,
  Box,
  Chip,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItemIcon,
  ListItemText,
  Paper,
  Typography,
} from '@mui/material'
import { alpha, styled } from '@mui/material/styles'
import BoltIcon from '@mui/icons-material/Bolt'
import SmartToyOutlinedIcon from '@mui/icons-material/SmartToyOutlined'
import ShieldOutlinedIcon from '@mui/icons-material/ShieldOutlined'
import PaymentsOutlinedIcon from '@mui/icons-material/PaymentsOutlined'
import DescriptionOutlinedIcon from '@mui/icons-material/DescriptionOutlined'
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined'
import CircleIcon from '@mui/icons-material/Circle'
import LogoutOutlinedIcon from '@mui/icons-material/LogoutOutlined'
import { NavLink } from 'react-router-dom'

export interface SidebarUser {
  name: string
  role?: string
  initials?: string
}

export interface SidebarProps {
  width: number
  user?: SidebarUser
  onLogout?: () => void
}

const StyledDrawerPaper = styled('div')(({ theme }) => ({
  height: '100%',
  display: 'flex',
  flexDirection: 'column',
  background: theme.palette.background.default,
  borderRight: `1px solid ${theme.palette.divider}`,
}))

const BrandRow = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  padding: theme.spacing(2.25, 2, 1.5),
}))

const LogoBadge = styled('div')(({ theme }) => ({
  width: 44,
  height: 44,
  borderRadius: 12,
  display: 'grid',
  placeItems: 'center',
  color: theme.palette.primary.contrastText,
  background: theme.palette.primary.main,
  boxShadow: theme.shadows[1],
}))

const SectionLabel = styled(Typography)(({ theme }) => ({
  padding: theme.spacing(1.5, 2, 1),
  fontSize: 12,
  fontWeight: 800,
  letterSpacing: '0.08em',
  textTransform: 'uppercase',
  color: theme.palette.text.disabled,
}))

const NavItemButton = styled(NavLink)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  textDecoration: 'none',

  margin: theme.spacing(0.25, 1.25),
  padding: theme.spacing(1.25, 1.5),
  borderRadius: 12,
  color: theme.palette.text.secondary,

  '& .MuiListItemIcon-root': {
    minWidth: 40,
    color: 'inherit',
  },

  '&:hover': {
    background: alpha(theme.palette.primary.main, 0.06),
    color: theme.palette.text.primary,
  },

  '&.active': {
    background: theme.palette.primary.main,
    color: theme.palette.primary.contrastText,
    boxShadow: theme.shadows[2],
  },

  '&.active .MuiListItemIcon-root': {
    color: theme.palette.primary.contrastText,
  },
}))

function NavItem({
  to,
  icon,
  label,
  subtitle,
  end = true,
}: {
  to: string
  icon: React.ReactNode
  label: string
  subtitle?: string
  /** end=true -> 严格匹配，避免 /tools/roster-settings 误匹配 /tools/roster */
  end?: boolean
}) {
  return (
    <NavItemButton
      to={to}
      end={end}
      className={({ isActive }) => (isActive ? 'active' : undefined)}
      aria-label={label}
    >
      <ListItemIcon>{icon}</ListItemIcon>
      <ListItemText
        primary={
          <Typography sx={{ fontWeight: 700, fontSize: 14 }}>
            {label}
          </Typography>
        }
        secondary={
          subtitle ? (
            <Typography sx={{ fontSize: 12, color: 'inherit', opacity: 0.85 }}>
              {subtitle}
            </Typography>
          ) : null
        }
      />
    </NavItemButton>
  )
}

export function Sidebar({ width, user, onLogout }: SidebarProps) {
  const initials =
    user?.initials ??
    user?.name
      ?.split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(p => p[0]?.toUpperCase())
      .join('') ??
    'U'

  return (
    <Drawer
      variant="permanent"
      open
      sx={{
        width,
        flexShrink: 0,
        '& .MuiDrawer-paper': { width, boxSizing: 'border-box' },
      }}
    >
      <StyledDrawerPaper>
        {/* Brand */}
        <BrandRow>
          <LogoBadge aria-label="FairWorkly logo">
            <BoltIcon />
          </LogoBadge>
          <Typography
            variant="h6"
            sx={{ fontWeight: 900, letterSpacing: '-0.02em' }}
          >
            FairWorkly
          </Typography>
        </BrandRow>

        <Divider />

        {/* FairBot Card */}
        <Box sx={{ p: 2 }}>
          <Paper
            variant="outlined"
            sx={{
              p: 2,
              borderRadius: 4,
              background: 'background.paper',
              boxShadow: 1,
            }}
          >
            <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-start' }}>
              <Box
                sx={theme => ({
                  width: 44,
                  height: 44,
                  borderRadius: 3,
                  display: 'grid',
                  placeItems: 'center',
                  bgcolor: alpha(theme.palette.primary.main, 0.08),
                  color: 'primary.main',
                })}
              >
                <SmartToyOutlinedIcon />
              </Box>

              <Box sx={{ flex: 1 }}>
                <Box
                  sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    mb: 0.5,
                  }}
                >
                  <Typography sx={{ fontWeight: 800 }}>FairBot</Typography>

                  <Chip
                    size="small"
                    icon={<CircleIcon sx={{ fontSize: 10 }} />}
                    label="ACTIVE"
                    sx={theme => ({
                      height: 22,
                      fontWeight: 800,
                      letterSpacing: '0.04em',
                      bgcolor: alpha(theme.palette.success.main, 0.12),
                      color: theme.palette.success.dark,
                      '& .MuiChip-icon': { color: theme.palette.success.main },
                    })}
                  />
                </Box>

                <Typography
                  sx={{
                    fontSize: 13,
                    color: 'text.secondary',
                    lineHeight: 1.5,
                  }}
                >
                  Ask about award rules, payroll and rosters — or upload files
                  to start a compliance check.
                </Typography>

                <Box sx={{ mt: 1.5 }}>
                  <Chip
                    size="small"
                    label="ONLINE & READY"
                    variant="outlined"
                    sx={{
                      fontWeight: 700,
                      borderRadius: 2,
                      color: 'text.secondary',
                    }}
                  />
                </Box>
              </Box>
            </Box>
          </Paper>
        </Box>

        {/* Navigation */}
        <Box component="nav" aria-label="Main navigation">
          <SectionLabel>Quick Actions</SectionLabel>
          <List disablePadding>
            <NavItem
              to="/tools/roster"
              icon={<ShieldOutlinedIcon />}
              label="Check Roster"
              end
            />
            <NavItem
              to="/tools/payroll"
              icon={<PaymentsOutlinedIcon />}
              label="Verify Payroll"
              end
            />
            <NavItem
              to="/tools/documents"
              icon={<DescriptionOutlinedIcon />}
              label="Documents Compliance"
              end
            />
          </List>

          <SectionLabel>Management</SectionLabel>
          <List disablePadding>
            <NavItem
              to="/settings"
              icon={<SettingsOutlinedIcon />}
              label="Settings"
              end
            />
          </List>
        </Box>

        <Box sx={{ flex: 1 }} />

        {/* Bottom User */}
        <Divider />
        <Box sx={{ p: 2, display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <Avatar sx={{ bgcolor: 'primary.main', width: 40, height: 40 }}>
            {initials}
          </Avatar>

          <Box sx={{ flex: 1, minWidth: 0 }}>
            <Typography sx={{ fontWeight: 800, fontSize: 14 }} noWrap>
              {user?.name ?? 'User'}
            </Typography>
            <Typography
              sx={{ fontSize: 12, color: 'text.secondary', fontWeight: 700 }}
              noWrap
            >
              {(user?.role ?? 'MEMBER').toUpperCase()}
            </Typography>
          </Box>

          <IconButton aria-label="logout" size="small" onClick={onLogout}>
            <LogoutOutlinedIcon fontSize="small" />
          </IconButton>
        </Box>
      </StyledDrawerPaper>
    </Drawer>
  )
}

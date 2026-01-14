import {
  Description as DescriptionOutlinedIcon,
  Payments as PaymentsOutlinedIcon,
  Settings as SettingsOutlinedIcon,
  Shield as ShieldOutlinedIcon,
} from '@mui/icons-material'

export type UserRole = 'admin' | 'manager'

export interface NavItemConfig {
  to: string
  icon: React.ReactElement
  label: string
  allowedRoles: readonly UserRole[]
}

export const mainNavItems: NavItemConfig[] = [
  {
    to: '/roster',
    icon: <ShieldOutlinedIcon />,
    label: 'Check Roster',
    allowedRoles: ['admin', 'manager'],
  },
  {
    to: '/payroll',
    icon: <PaymentsOutlinedIcon />,
    label: 'Verify Payroll',
    allowedRoles: ['admin'],
  },
  {
    to: '/documents',
    icon: <DescriptionOutlinedIcon />,
    label: 'Documents Compliance',
    allowedRoles: ['admin'],
  },
]

export const settingsNavItems: NavItemConfig[] = [
  {
    to: '/settings',
    icon: <SettingsOutlinedIcon />,
    label: 'Settings',
    allowedRoles: ['admin'],
  },
]

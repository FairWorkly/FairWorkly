import React from 'react'
import { NavLink } from 'react-router-dom'
import { ListItemIcon } from '@mui/material'
import { NavItemButton, NavItemText } from '../Sidebar.styles'

export interface NavItemProps {
  to: string
  icon: React.ReactNode
  label: string
  /**
   * end=true -> strict matching, avoid /tools/roster-settings matching /tools/roster
   */
  end?: boolean
}

export function NavItem({ to, icon, label, end = true }: NavItemProps) {
  return (
    <NavItemButton
      as={NavLink}
      to={to}
      end={end}
      className={({ isActive }: { isActive: boolean }) => (isActive ? 'active' : undefined)}
      aria-label={label}
    >
      <ListItemIcon>{icon}</ListItemIcon>
      <NavItemText variant="subtitle2">{label}</NavItemText>
    </NavItemButton>
  )
}

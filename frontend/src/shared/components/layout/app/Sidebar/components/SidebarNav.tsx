import { List } from '@mui/material'
import { NavContainer, SectionLabel, SectionTitle } from '../Sidebar.styles'
import { mainNavItems, settingsNavItems, type NavItemConfig } from '../config/navigation.config.tsx'
import { NavItem } from './NavItem'

export interface SidebarNavProps {
  userRole?: string
}

function canAccess(allowedRoles: readonly string[], userRole?: string): boolean {
  return userRole ? allowedRoles.includes(userRole) : false
}

function filterByRole(items: NavItemConfig[], userRole?: string): NavItemConfig[] {
  return items.filter(item => canAccess(item.allowedRoles, userRole))
}

export function SidebarNav({ userRole }: SidebarNavProps) {
  const visibleMainItems = filterByRole(mainNavItems, userRole)
  const visibleSettingsItems = filterByRole(settingsNavItems, userRole)

  return (
    <NavContainer>
      <nav aria-label="Main navigation">
        {visibleMainItems.length > 0 && (
          <>
            <SectionLabel>
              <SectionTitle variant="overline">Quick Actions</SectionTitle>
            </SectionLabel>

            <List disablePadding>
              {visibleMainItems.map(item => (
                <NavItem key={item.to} to={item.to} icon={item.icon} label={item.label} end />
              ))}
            </List>
          </>
        )}

        {visibleSettingsItems.length > 0 && (
          <>
            <SectionLabel>
              <SectionTitle variant="overline">Management</SectionTitle>
            </SectionLabel>

            <List disablePadding>
              {visibleSettingsItems.map(item => (
                <NavItem key={item.to} to={item.to} icon={item.icon} label={item.label} end />
              ))}
            </List>
          </>
        )}
      </nav>
    </NavContainer>
  )
}

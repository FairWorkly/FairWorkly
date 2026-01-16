import { Logout as LogoutIcon } from '@mui/icons-material'
import { BottomRow, LogoutButton, UserAvatar, UserMeta, UserName, UserRole } from '../Sidebar.styles'
import { makeInitials } from '../utils/helpers'

export interface SidebarUserProps {
  name?: string
  role?: string
  initials?: string
  onLogout?: () => void
}

export function SidebarUser({ name, role, initials, onLogout }: SidebarUserProps) {
  const displayInitials = initials ?? makeInitials(name)

  return (
    <BottomRow>
      <UserAvatar aria-label="user avatar">{displayInitials}</UserAvatar>

      <UserMeta>
        <UserName variant="subtitle2" noWrap>
          {name ?? 'User'}
        </UserName>

        <UserRole variant="caption" noWrap color="text.disabled">
          {(role ?? 'MEMBER').toUpperCase()}
        </UserRole>
      </UserMeta>

      {onLogout && (
        <LogoutButton onClick={onLogout} aria-label="Logout">
          <LogoutIcon />
        </LogoutButton>
      )}
    </BottomRow>
  )
}

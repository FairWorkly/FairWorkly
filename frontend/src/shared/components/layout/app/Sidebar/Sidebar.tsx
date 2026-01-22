import type { DrawerProps } from '@mui/material/Drawer'
import { SidebarDrawer, SidebarPaper } from './Sidebar.styles'
import { SidebarBrand } from './components/SidebarBrand'
import { FairBotCard } from './components/FairBotCard'
import { SidebarNav } from './components/SidebarNav'
import { SidebarUser as SidebarUserComponent } from './components/SidebarUser'

export interface SidebarUser {
  name: string
  role?: string
  initials?: string
}

export interface SidebarProps {
  user?: SidebarUser
  onLogout?: () => void
  variant?: DrawerProps['variant']
  open?: boolean
  onClose?: () => void
}

export function Sidebar({
  user,
  onLogout,
  variant = 'permanent',
  open = true,
  onClose,
}: SidebarProps) {
  const isTemporary = variant === 'temporary'
  return (
    <SidebarDrawer
      variant={variant}
      open={open}
      onClose={onClose}
      ModalProps={
        isTemporary
          ? {
              disableScrollLock: true,
              keepMounted: true,
            }
          : undefined
      }
    >
      <SidebarPaper>
        <SidebarBrand />

        <FairBotCard />

        <SidebarNav userRole={user?.role} />

        <SidebarUserComponent
          name={user?.name}
          role={user?.role}
          initials={user?.initials}
          onLogout={onLogout}
        />
      </SidebarPaper>
    </SidebarDrawer>
  )
}

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
}

export function Sidebar({ user, onLogout }: SidebarProps) {
  return (
    <SidebarDrawer variant="permanent" open>
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

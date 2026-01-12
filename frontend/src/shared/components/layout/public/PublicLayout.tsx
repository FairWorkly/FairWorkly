import { Outlet } from 'react-router-dom'
import { PublicShell } from './PublicLayout.styles'

export function PublicLayout() {
  return (
    <PublicShell>
      <Outlet />
    </PublicShell>
  )
}

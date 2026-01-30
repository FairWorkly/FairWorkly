import { Navigate, Outlet } from 'react-router-dom'
import { LoadingSpinner } from '@/shared/components/ui/LoadingSpinner'
import { useAppSelector } from '@/store/hooks'

type Role = 'admin' | 'manager'

export function RoleBasedRoute({ allow }: { allow: Role[] }) {
  const { status, user } = useAppSelector((state) => state.auth)

  if (status === 'initializing') {
    return <LoadingSpinner />
  }

  if (status !== 'authenticated') return <Navigate to="/login" replace />

  const role = (user?.role ?? '').toLowerCase() as Role
  if (!role || !allow.includes(role)) return <Navigate to="/403" replace />

  return <Outlet />
}

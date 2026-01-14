import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '@/shared/hooks/useAuth'
import { LoadingSpinner } from '../ui/LoadingSpinner'

type UserRole = 'admin' | 'manager'

interface RoleBasedRouteProps {
  allowedRoles: UserRole[]
}

export function RoleBasedRoute({ allowedRoles }: RoleBasedRouteProps) {
  const { user, isLoading } = useAuth()

  if (isLoading) {
    return <LoadingSpinner />
  }

  if (!user) {
    return <Navigate to="/login" replace />
  }

  if (!allowedRoles.includes(user.role)) {
    return <Navigate to="/403" replace />
  }

  return <Outlet />
}

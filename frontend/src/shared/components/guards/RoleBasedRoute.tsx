import { Navigate, Outlet } from 'react-router-dom'
import { useAuth, type AuthUser } from '@/shared/hooks/useAuth'
import { LoadingSpinner } from '@/shared/components/ui/LoadingSpinner'

type Role = NonNullable<AuthUser>['role']

export function RoleBasedRoute({ allow }: { allow: Role[] }) {
  const { isLoading, isAuthenticated, user } = useAuth()

  if (isLoading) {
    return <LoadingSpinner />
  }

  if (isLoading) return null
  if (!isAuthenticated) return <Navigate to="/login" replace />

  const role = user?.role
  if (!role || !allow.includes(role)) return <Navigate to="/403" replace />

  return <Outlet />
}

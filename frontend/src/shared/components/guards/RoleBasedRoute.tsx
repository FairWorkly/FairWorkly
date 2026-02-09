import { Navigate, Outlet } from 'react-router-dom'
import { LoadingSpinner } from '@/shared/components/ui/LoadingSpinner'
import { useAppSelector } from '@/store/hooks'

export function RoleBasedRoute({ allow }: { allow: string[] }) {
  const { status, user } = useAppSelector((state) => state.auth)

  if (status === 'initializing') {
    return <LoadingSpinner />
  }

  if (status !== 'authenticated') return <Navigate to="/login" replace />

  const role = (user?.role ?? '').toLowerCase()
  const allowList = allow.map((allowed) => allowed.toLowerCase())
  if (!role || !allowList.includes(role)) return <Navigate to="/403" replace />

  return <Outlet />
}

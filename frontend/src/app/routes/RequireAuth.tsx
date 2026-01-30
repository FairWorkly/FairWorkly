import { Navigate, Outlet } from 'react-router-dom'
import { LoadingSpinner } from '@/shared/components/ui/LoadingSpinner'
import { useAppSelector } from '@/store/hooks'

export function RequireAuth() {
  const { status } = useAppSelector((state) => state.auth)

  if (status === 'initializing') {
    return <LoadingSpinner />
  }

  if (status === 'unauthenticated') {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
}

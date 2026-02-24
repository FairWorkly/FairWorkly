import { Navigate, Outlet } from 'react-router-dom'
import { LoadingSpinner } from '@/shared/components/ui/LoadingSpinner'
import { useAppSelector } from '@/store/hooks'

export function ProtectedRoute() {
  const { status } = useAppSelector(state => state.auth)

  if (status === 'initializing') return <LoadingSpinner />

  return status === 'authenticated' ? (
    <Outlet />
  ) : (
    <Navigate to="/login" replace />
  )
}

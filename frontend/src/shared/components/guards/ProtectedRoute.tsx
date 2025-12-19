import { Navigate } from 'react-router-dom'

interface ProtectedRouteProps {
  children: React.ReactNode
  requiredModule?: string
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  // TODO: 后续接入真实的auth context
  const isAuthenticated = false // 暂时写死false，开发时改成true

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}

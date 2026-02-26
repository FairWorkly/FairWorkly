import { useCallback } from 'react'
import { useSelector, useDispatch } from 'react-redux'
import type { RootState } from '@/store'
import { logout as logoutAction } from '@/slices/auth/authSlice'
import { authApi } from '@/services/authApi'
import type { AuthUser, AuthState } from '../types'

export function useAuth(): AuthState {
  const dispatch = useDispatch()

  // Read auth state from Redux
  const reduxUser = useSelector((state: RootState) => state.auth.user)
  const accessToken = useSelector((state: RootState) => state.auth.accessToken)
  const status = useSelector((state: RootState) => state.auth.status)

  // Convert Redux user to AuthUser format
  const user: AuthUser | null = reduxUser
    ? {
        id: reduxUser.id,
        name: reduxUser.name || reduxUser.email || 'User',
        role: reduxUser.role,
      }
    : null

  const isAuthenticated = !!accessToken && !!user && status === 'authenticated'
  const isLoading = status === 'initializing' || status === 'authenticating'

  const logout = useCallback(async () => {
    try {
      await authApi.logout()
    } catch {
      // Clear local state even if backend call fails
    }
    try {
      if (typeof window !== 'undefined') {
        window.sessionStorage.removeItem('fairbot_conversation')
      }
    } catch {
      // Storage may be unavailable in restricted contexts (e.g. Safari private mode)
    }
    dispatch(logoutAction())
  }, [dispatch])

  return {
    isAuthenticated,
    isLoading,
    user,
    logout,
  }
}

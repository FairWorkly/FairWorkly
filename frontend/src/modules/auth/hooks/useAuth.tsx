import { useCallback } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import type { RootState } from '@/store'
import { logout as logoutAction } from '@/slices/auth/authSlice'
import { authApi } from '@/services/authApi'
import type { AuthUser, AuthState } from '../types'

export function useAuth(): AuthState {
  const dispatch = useDispatch()

  const { user: storeUser, accessToken, status } = useSelector(
    (state: RootState) => state.auth
  )

  const logout = useCallback(async () => {
    try {
      await authApi.logout()
    } catch {
      // ignore logout errors and still clear client state
    } finally {
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      dispatch(logoutAction())
    }
  }, [dispatch])


  const user: AuthUser | null = storeUser
    ? {
        id: storeUser.id ?? '',
        name: storeUser.name ?? '',
        role: (storeUser.role as 'admin' | 'manager') ?? 'manager',
      }
    : null

  return {
    isAuthenticated: Boolean(accessToken) && status === 'authenticated',
    isLoading: status === 'initializing' || status === 'authenticating',
    user,
    logout,
  }
}

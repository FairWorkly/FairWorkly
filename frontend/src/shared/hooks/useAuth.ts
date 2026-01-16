import { useState, useCallback } from 'react'

export type AuthUser = {
  id: string
  name: string
  role: 'admin' | 'manager'
}

export type AuthState = {
  isAuthenticated: boolean
  isLoading: boolean
  user: AuthUser | null
  switchRole: (role: 'admin' | 'manager') => void
  logout: () => void
}

const DEV_ROLE_STORAGE_KEY = 'dev:user-role'

function getStoredRole(): 'admin' | 'manager' {
  if (typeof window === 'undefined') return 'admin'
  const stored = localStorage.getItem(DEV_ROLE_STORAGE_KEY)
  return stored === 'manager' ? 'manager' : 'admin'
}

export function useAuth(): AuthState {
  const [role, setRole] = useState<'admin' | 'manager'>(getStoredRole)

  const switchRole = useCallback((newRole: 'admin' | 'manager') => {
    setRole(newRole)
    localStorage.setItem(DEV_ROLE_STORAGE_KEY, newRole)
  }, [])

  const logout = useCallback(() => {
    // Clear stored role
    localStorage.removeItem(DEV_ROLE_STORAGE_KEY)
    // Clear any other auth data when implemented
    // Future: Clear JWT tokens, reset auth state, etc.
  }, [])

  const user: AuthUser = { id: '1', name: 'Lillian', role }

  return {
    isAuthenticated: true,
    isLoading: false, // DEV: 改成 true 可测试 LoadingSpinner
    user,
    switchRole,
    logout,
  }
}

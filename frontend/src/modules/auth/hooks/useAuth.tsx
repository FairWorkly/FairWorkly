import { useState, useCallback } from 'react'
import type { AuthUser, AuthState } from '../types'

// DEV stub: used to persist role locally until JWT integration.
const DEV_ROLE_STORAGE_KEY = 'dev:user-role'
// DEV stub: used to persist a display name locally for UI testing.
const DEV_USER_NAME_STORAGE_KEY = 'dev:user-name'

function getStoredRole(): 'admin' | 'manager' {
  if (typeof window === 'undefined') return 'admin'
  const stored = localStorage.getItem(DEV_ROLE_STORAGE_KEY)
  return stored === 'manager' ? 'manager' : 'admin'
}

function getStoredUser(): AuthUser | null {
  if (typeof window === 'undefined') return null
  const name = localStorage.getItem(DEV_USER_NAME_STORAGE_KEY)
  if (!name) return { id: '1', name: 'Lillian', role: getStoredRole() }
  return { id: '1', name, role: getStoredRole() }
}

export function useAuth(): AuthState {
  const [user, setUser] = useState<AuthUser | null>(getStoredUser)

  const switchRole = useCallback((newRole: 'admin' | 'manager') => {
    localStorage.setItem(DEV_ROLE_STORAGE_KEY, newRole)
    location.reload()
  }, [])

  const logout = useCallback(() => {
    localStorage.removeItem(DEV_ROLE_STORAGE_KEY)
    localStorage.removeItem(DEV_USER_NAME_STORAGE_KEY)
    setUser(null)
  }, [])

  return {
    isAuthenticated: user !== null,
    isLoading: false,
    user,
    switchRole,
    logout,
  }
}

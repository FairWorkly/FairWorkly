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

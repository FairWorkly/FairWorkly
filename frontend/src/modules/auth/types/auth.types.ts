export type AuthUser = {
  id: string
  name: string
  role?: string
}

export type AuthState = {
  isAuthenticated: boolean
  isLoading: boolean
  user: AuthUser | null
  logout: () => void
}

import type { UserDto } from '@/services/authApi'

export const DEFAULT_ROUTES: Record<string, string> = {
  admin: '/fairbot',
  manager: '/roster/upload',
}

export function normalizeAuthUser(user: UserDto) {
  const name = [user.firstName, user.lastName].filter(Boolean).join(' ') || user.email
  const role = user.role?.toLowerCase()
  const validRole: 'admin' | 'manager' | undefined =
    role === 'admin' || role === 'manager' ? role : undefined

  return {
    normalizedUser: {
      id: user.id,
      email: user.email,
      name,
      role: validRole,
    },
    roleKey: role ?? '',
  }
}

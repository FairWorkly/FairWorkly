import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { authApi } from '@/services/authApi'
import { setAuthData, setStatus } from '@/slices/auth'
import { useAppDispatch } from '@/store/hooks'
import type { LoginFormData } from '../types'
import type { AuthUser } from '@/slices/auth'

type UseLoginResult = {
  login: (values: LoginFormData) => Promise<void>
  isSubmitting: boolean
  error: string | null
}

const DEFAULT_ROUTES: Record<'admin' | 'manager', string> = {
  admin: '/fairbot',
  manager: '/roster/upload',
}

function mapUserName(firstName?: string, lastName?: string, email?: string) {
  const fullName = [firstName, lastName].filter(Boolean).join(' ').trim()
  if (fullName) return fullName
  return email ?? 'User'
}

type AuthRole = NonNullable<AuthUser['role']>

function normalizeRole(role?: string): AuthRole | null {
  const normalized = (role ?? '').toLowerCase()
  if (normalized === 'admin' || normalized === 'manager') return normalized
  return null
}

function mapRoleRedirect(role: AuthRole) {
  return DEFAULT_ROUTES[role] ?? '/403'
}

export function useLogin(): UseLoginResult {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const login = useCallback(
    async (values: LoginFormData) => {
      if (isSubmitting) return
      setIsSubmitting(true)
      setError(null)
      dispatch(setStatus('authenticating'))

      try {
        const response = await authApi.login({
          email: values.email,
          password: values.password,
        })

        const role = normalizeRole(response.user.role)
        if (!role) {
          throw new Error('Unsupported user role')
        }

        const user = {
          id: response.user.id,
          email: response.user.email,
          role,
          name: mapUserName(
            response.user.firstName,
            response.user.lastName,
            response.user.email,
          ),
        }

        dispatch(setAuthData({ user, accessToken: response.accessToken }))

        navigate(mapRoleRedirect(role), { replace: true })
      } catch (err) {
        if (err instanceof Error && err.message === 'Unsupported user role') {
          setError('Login failed: unsupported account role. Please contact support.')
        } else {
          setError(
            err instanceof Error ? err.message : 'Login failed. Please try again.',
          )
        }
        dispatch(setStatus('unauthenticated'))
      } finally {
        setIsSubmitting(false)
      }
    },
    [dispatch, isSubmitting, navigate],
  )

  return { login, isSubmitting, error }
}

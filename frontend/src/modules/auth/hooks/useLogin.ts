import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { authApi } from '@/services/authApi'
import { setAuthData, setStatus } from '@/slices/auth'
import { useAppDispatch } from '@/store/hooks'
import type { LoginFormData } from '../types'

type UseLoginResult = {
  login: (values: LoginFormData) => Promise<void>
  isSubmitting: boolean
  error: string | null
}

const DEFAULT_ROUTES = {
  admin: '/fairbot',
  manager: '/roster/upload',
} as const

function mapUserName(firstName?: string, lastName?: string, email?: string) {
  const fullName = [firstName, lastName].filter(Boolean).join(' ').trim()
  if (fullName) return fullName
  return email ?? 'User'
}

function mapRoleRedirect(role: string) {
  const normalizedRole = role.toLowerCase()
  return DEFAULT_ROUTES[normalizedRole as keyof typeof DEFAULT_ROUTES] ?? '/403'
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

        const role = response.user.role?.toLowerCase()
        const validRole: 'admin' | 'manager' | undefined =
          role === 'admin' || role === 'manager' ? role : undefined

        const user = {
          id: response.user.id,
          email: response.user.email,
          role: validRole,
          name: mapUserName(
            response.user.firstName,
            response.user.lastName,
            response.user.email,
          ),
        }

        dispatch(setAuthData({ user, accessToken: response.accessToken }))

        navigate(mapRoleRedirect(role ?? ''), { replace: true })
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Login failed. Please try again.'
        setError(message)
        dispatch(setStatus('unauthenticated'))
      } finally {
        setIsSubmitting(false)
      }
    },
    [dispatch, isSubmitting, navigate],
  )

  return { login, isSubmitting, error }
}

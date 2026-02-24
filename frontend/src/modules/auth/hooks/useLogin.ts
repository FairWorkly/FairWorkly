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

const DEFAULT_ROUTES: Record<string, string> = {
  admin: '/fairbot',
  manager: '/roster/upload',
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

        const name =
          [response.user.firstName, response.user.lastName]
            .filter(Boolean)
            .join(' ') || response.user.email

        const role = response.user.role?.toLowerCase()
        const validRole: 'admin' | 'manager' | undefined =
          role === 'admin' || role === 'manager' ? role : undefined

        dispatch(
          setAuthData({
            user: {
              id: response.user.id,
              email: response.user.email,
              name,
              role: validRole,
            },
            accessToken: response.accessToken,
          })
        )

        navigate(DEFAULT_ROUTES[role ?? ''] ?? '/403', { replace: true })
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Login failed. Please try again.'
        setError(message)
        dispatch(setStatus('unauthenticated'))
      } finally {
        setIsSubmitting(false)
      }
    },
    [dispatch, isSubmitting, navigate]
  )

  return { login, isSubmitting, error }
}

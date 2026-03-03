import { useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { authApi } from '@/services/authApi'
import { normalizeApiError } from '@/shared/types/api.types'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { setAuthData, setStatus, type AuthUser } from '@/slices/auth'
import { useAppDispatch } from '@/store/hooks'
import type { LoginFormData } from '../types'
import { DEFAULT_ROUTES, normalizeAuthUser } from './authUtils'

type AuthResult = {
  normalizedUser: AuthUser
  roleKey: string
  accessToken: string
}

export function useLogin() {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()

  const { mutate, isPending, error } = useApiMutation<AuthResult, LoginFormData>({
    mutationFn: async (values) => {
      const response = await authApi.login({
        email: values.email,
        password: values.password,
      })
      const { normalizedUser, roleKey } = normalizeAuthUser(response.user)
      return { normalizedUser, roleKey, accessToken: response.accessToken }
    },
    onMutate: () => {
      dispatch(setStatus('authenticating'))
    },
    onSuccess: ({ normalizedUser, roleKey, accessToken }) => {
      dispatch(setAuthData({ user: normalizedUser, accessToken }))
      navigate(DEFAULT_ROUTES[roleKey] ?? '/403', { replace: true })
    },
    onError: () => {
      dispatch(setStatus('unauthenticated'))
    },
  })

  const errorMessage = useMemo(() => {
    if (!error) return null
    return normalizeApiError(error).message
  }, [error])

  return { login: mutate, isSubmitting: isPending, error: errorMessage }
}

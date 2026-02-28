import { useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { authApi } from '@/services/authApi'
import { normalizeApiError } from '@/shared/types/api.types'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { setAuthData, setStatus, type AuthUser } from '@/slices/auth'
import { useAppDispatch } from '@/store/hooks'
import type { SignupFormData } from '../types'
import { DEFAULT_ROUTES, normalizeAuthUser } from './authUtils'

type AuthResult = {
  normalizedUser: AuthUser
  roleKey: string
  accessToken: string
}

export function useRegister() {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()

  const { mutate, isPending, error } = useApiMutation<AuthResult, SignupFormData>({
    mutationFn: async (values) => {
      const response = await authApi.register({
        companyName: values.companyName,
        abn: values.abn,
        industryType: values.industryType,
        addressLine1: values.addressLine1,
        addressLine2: values.addressLine2,
        suburb: values.suburb,
        state: values.state,
        postcode: values.postcode,
        contactEmail: values.contactEmail,
        email: values.email,
        password: values.password,
        firstName: values.firstName,
        lastName: values.lastName,
      })
      // Validate role inside mutationFn so throw → proper error state
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

  // Normalize error: runtime error is AxiosError, normalizeApiError extracts
  // backend envelope msg and validation details
  const errorMessage = useMemo(() => {
    if (!error) return null
    const normalized = normalizeApiError(error)
    const details = normalized.details as
      | { errors?: { message?: string }[] }
      | undefined
    return details?.errors?.[0]?.message ?? normalized.message
  }, [error])

  return { register: mutate, isSubmitting: isPending, error: errorMessage }
}

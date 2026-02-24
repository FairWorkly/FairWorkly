import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import axios from 'axios'
import { authApi } from '@/services/authApi'
import { setAuthData, setStatus } from '@/slices/auth'
import { useAppDispatch } from '@/store/hooks'
import type { SignupFormData } from '../types'
import { DEFAULT_ROUTES, normalizeAuthUser } from './authUtils'

type UseRegisterResult = {
  register: (values: SignupFormData) => Promise<void>
  isSubmitting: boolean
  error: string | null
}

type ApiValidationError = {
  field?: string
  message?: string
}

type ApiErrorEnvelope = {
  msg?: string
  data?: {
    errors?: ApiValidationError[]
  }
}

export function useRegister(): UseRegisterResult {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const register = useCallback(
    async (values: SignupFormData) => {
      if (isSubmitting) return
      setIsSubmitting(true)
      setError(null)
      dispatch(setStatus('authenticating'))

      try {
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

        const { normalizedUser, roleKey } = normalizeAuthUser(response.user)

        dispatch(setAuthData({
          user: normalizedUser,
          accessToken: response.accessToken,
        }))

        navigate(DEFAULT_ROUTES[roleKey] ?? '/403', { replace: true })
      } catch (err) {
        let message = 'Registration failed. Please try again.'

        if (axios.isAxiosError<ApiErrorEnvelope>(err)) {
          const validationMessage = err.response?.data?.data?.errors?.[0]?.message
          const backendMessage = err.response?.data?.msg
          message = validationMessage ?? backendMessage ?? err.message ?? message
        } else if (err instanceof Error) {
          message = err.message
        }

        setError(message)
        dispatch(setStatus('unauthenticated'))
      } finally {
        setIsSubmitting(false)
      }
    },
    [dispatch, isSubmitting, navigate],
  )

  return { register, isSubmitting, error }
}

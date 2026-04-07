import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { authApi } from '@/services/authApi'

export interface ResetPasswordFormValues {
  token: string
  password: string
}

export function useResetPassword() {
  return useApiMutation<boolean, ResetPasswordFormValues>({
    mutationFn: ({ token, password }) => authApi.resetPassword(token, password),
  })
}

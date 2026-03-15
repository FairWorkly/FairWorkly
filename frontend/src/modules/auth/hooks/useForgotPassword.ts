import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { authApi } from '@/services/authApi'

export function useForgotPassword() {
  return useApiMutation<boolean, string>({
    mutationFn: email => authApi.forgotPassword(email),
  })
}

import { useApiMutation } from '@/shared/hooks/useApiMutation'
import {
  settingsApi,
  type AcceptInvitationRequest,
  type AcceptInvitationResponse,
} from '@/services/settingsApi'

export function useAcceptInvitation() {
  return useApiMutation<AcceptInvitationResponse, AcceptInvitationRequest>({
    mutationFn: (payload) => settingsApi.acceptInvitation(payload),
  })
}

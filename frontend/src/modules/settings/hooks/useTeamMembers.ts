import { useQueryClient } from '@tanstack/react-query'
import { useApiQuery } from '@/shared/hooks/useApiQuery'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import {
  settingsApi,
  type TeamMemberDto,
  type TeamMemberUpdatedDto,
  type UpdateTeamMemberRequest,
  type InviteTeamMemberRequest,
  type InviteTeamMemberResponse,
  type ResendInvitationResponse,
} from '@/services/settingsApi'

const QUERY_KEY = ['settings', 'team'] as const

export function useTeamMembers() {
  return useApiQuery<TeamMemberDto[]>({
    queryKey: QUERY_KEY,
    queryFn: settingsApi.getTeamMembers,
  })
}

export function useUpdateTeamMember() {
  const queryClient = useQueryClient()

  return useApiMutation<
    TeamMemberUpdatedDto,
    { userId: string; payload: UpdateTeamMemberRequest }
  >({
    mutationFn: ({ userId, payload }) =>
      settingsApi.updateTeamMember(userId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}

export function useInviteTeamMember() {
  const queryClient = useQueryClient()

  return useApiMutation<InviteTeamMemberResponse, InviteTeamMemberRequest>({
    mutationFn: (payload) => settingsApi.inviteTeamMember(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}

export function useResendInvitation() {
  const queryClient = useQueryClient()

  return useApiMutation<ResendInvitationResponse, string>({
    mutationFn: (userId) => settingsApi.resendInvitation(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}

export function useCancelInvitation() {
  const queryClient = useQueryClient()

  return useApiMutation<void, string>({
    mutationFn: (userId) => settingsApi.cancelInvitation(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}

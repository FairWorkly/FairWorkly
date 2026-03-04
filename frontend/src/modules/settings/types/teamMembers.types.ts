export type {
  TeamMemberDto,
  UpdateTeamMemberRequest,
  TeamMemberUpdatedDto,
  InviteTeamMemberRequest,
  InviteTeamMemberResponse,
  ResendInvitationResponse,
} from '@/services/settingsApi'

export const ROLE_OPTIONS = [
  { value: 'Admin', label: 'Admin' },
  { value: 'Manager', label: 'Manager' },
] as const

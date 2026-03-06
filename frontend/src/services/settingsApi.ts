import * as baseApi from './baseApi'

// --- Team Members ---

export interface TeamMemberDto {
  userId: string
  fullName: string
  email: string
  role: 'Admin' | 'Manager'
  isActive: boolean
  invitationStatus: 'None' | 'Pending' | 'Accepted'
  invitationTokenExpiry: string | null
  lastLoginAt: string | null
}

export interface UpdateTeamMemberRequest {
  role?: 'Admin' | 'Manager'
  isActive?: boolean
}

export interface TeamMemberUpdatedDto {
  userId: string
  role: 'Admin' | 'Manager'
  isActive: boolean
}

export interface InviteTeamMemberRequest {
  email: string
  firstName: string
  lastName: string
  role: 'Admin' | 'Manager'
}

export interface InviteTeamMemberResponse {
  userId: string
  inviteLink: string
}

export interface ResendInvitationResponse {
  inviteLink: string
}

export interface ValidateInvitationTokenResponse {
  email: string
  fullName: string
}

export interface AcceptInvitationRequest {
  token: string
  password: string
}

export interface AcceptInvitationResponse {
  email: string
  fullName: string
}

// --- Organization Profile ---

export interface OrganizationProfileDto {
  companyName: string
  abn: string
  industryType: string
  primaryAward: string | null
  contactEmail: string
  phoneNumber: string | null
  addressLine1: string
  addressLine2: string | null
  suburb: string
  state: string
  postcode: string
  logoUrl: string | null
}

export interface UpdateOrganizationProfileRequest {
  companyName: string
  abn: string
  industryType: string
  primaryAward?: string | null
  contactEmail: string
  phoneNumber?: string | null
  addressLine1: string
  addressLine2?: string | null
  suburb: string
  state: string
  postcode: string
  logoUrl?: string | null
}

export const settingsApi = {
  // --- Team Members ---

  getTeamMembers: () => baseApi.get<TeamMemberDto[]>('/settings/team'),

  updateTeamMember: (userId: string, payload: UpdateTeamMemberRequest) =>
    baseApi.patch<TeamMemberUpdatedDto>(`/settings/team/${userId}`, payload),

  inviteTeamMember: (payload: InviteTeamMemberRequest) =>
    baseApi.post<InviteTeamMemberResponse>('/settings/team/invite', payload),

  resendInvitation: (userId: string) =>
    baseApi.post<ResendInvitationResponse>(`/settings/team/${userId}/resend-invite`),

  cancelInvitation: (userId: string) =>
    baseApi.del<void>(`/settings/team/${userId}/invite`),

  validateInvitationToken: (token: string) =>
    baseApi.get<ValidateInvitationTokenResponse>(`/invite/validate?token=${encodeURIComponent(token)}`),

  acceptInvitation: (payload: AcceptInvitationRequest) =>
    baseApi.post<AcceptInvitationResponse>('/invite/accept', payload),

  // --- Organization Profile ---

  getOrganizationProfile: () =>
    baseApi.get<OrganizationProfileDto>('/settings/organization-profile'),

  updateOrganizationProfile: (payload: UpdateOrganizationProfileRequest) =>
    baseApi.put<OrganizationProfileDto>('/settings/organization-profile', payload),
}

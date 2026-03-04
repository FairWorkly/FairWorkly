import httpClient from './httpClient'

// --- Team Members ---

export interface TeamMemberDto {
  userId: string
  fullName: string
  email: string
  role: 'Admin' | 'Manager'
  isActive: boolean
  invitationStatus: 'None' | 'Pending' | 'Accepted'
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

  async getTeamMembers(): Promise<TeamMemberDto[]> {
    const response = await httpClient.get<TeamMemberDto[]>('/settings/team')
    return response.data
  },

  async updateTeamMember(
    userId: string,
    payload: UpdateTeamMemberRequest
  ): Promise<TeamMemberUpdatedDto> {
    const response = await httpClient.patch<TeamMemberUpdatedDto>(
      `/settings/team/${userId}`,
      payload
    )
    return response.data
  },

  async inviteTeamMember(
    payload: InviteTeamMemberRequest
  ): Promise<InviteTeamMemberResponse> {
    const response = await httpClient.post<InviteTeamMemberResponse>(
      '/settings/team/invite',
      payload
    )
    return response.data
  },

  async resendInvitation(userId: string): Promise<ResendInvitationResponse> {
    const response = await httpClient.post<ResendInvitationResponse>(
      `/settings/team/${userId}/resend-invite`
    )
    return response.data
  },

  async acceptInvitation(
    payload: AcceptInvitationRequest
  ): Promise<AcceptInvitationResponse> {
    const response = await httpClient.post<AcceptInvitationResponse>(
      '/invite/accept',
      payload
    )
    return response.data
  },

  // --- Organization Profile ---

  async getOrganizationProfile(): Promise<OrganizationProfileDto> {
    const response = await httpClient.get<OrganizationProfileDto>(
      '/settings/organization-profile'
    )
    return response.data
  },

  async updateOrganizationProfile(
    payload: UpdateOrganizationProfileRequest
  ): Promise<OrganizationProfileDto> {
    const response = await httpClient.put<OrganizationProfileDto>(
      '/settings/organization-profile',
      payload
    )
    return response.data
  },
}

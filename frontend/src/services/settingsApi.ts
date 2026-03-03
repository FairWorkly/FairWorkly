import httpClient from './httpClient'

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

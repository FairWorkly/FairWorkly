import httpClient from './httpClient'

export interface LoginRequest {
  email: string
  password: string
}

export interface UserDto {
  id: string
  email: string
  firstName: string
  lastName: string
  role: string
  organizationId: string
}

export interface LoginResponse {
  accessToken: string
  user: UserDto
}

export const authApi = {
  /**
   * Login with email and password
   * Returns access token and user info. Refresh token is set as HttpOnly cookie.
   */
  async login(payload: LoginRequest): Promise<LoginResponse> {
    const response = await httpClient.post<LoginResponse>(
      '/auth/login',
      payload
    )
    return response.data
  },

  /**
   * Logout - clears the refresh token cookie
   */
  async logout(): Promise<void> {
    await httpClient.post('/auth/logout')
  },

  /**
   * Get current user info
   */
  async me(): Promise<UserDto> {
    const response = await httpClient.get<UserDto>('/auth/me')
    return response.data
  },
}

import * as baseApi from './baseApi'
import httpClient from './httpClient'

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  companyName: string
  abn: string
  industryType: string
  addressLine1: string
  addressLine2?: string
  suburb: string
  state: string
  postcode: string
  contactEmail: string
  email: string
  password: string
  firstName: string
  lastName: string
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

export interface ResetPasswordPayload {
  token: string
  password: string
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
   * Register a new organization + first admin user
   */
  async register(payload: RegisterRequest): Promise<LoginResponse> {
    const response = await httpClient.post<LoginResponse>(
      '/auth/register',
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

  /**
   * Request a password reset link for the given email.
   */
  forgotPassword(email: string): Promise<boolean> {
    return baseApi.post<boolean, { email: string }>('/auth/forgot-password', {
      email,
    })
  },

  /**
   * Validate a password reset token before showing the reset form.
   */
  validateResetPasswordToken(token: string): Promise<boolean> {
    return baseApi.get<boolean>(
      `/auth/reset-password/validate?token=${encodeURIComponent(token)}`
    )
  },

  /**
   * Reset a password with a valid reset token.
   */
  resetPassword(token: string, password: string): Promise<boolean> {
    return baseApi.post<boolean, ResetPasswordPayload>('/auth/reset-password', {
      token,
      password,
    })
  },
}

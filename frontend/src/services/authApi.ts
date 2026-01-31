import { post } from './baseApi'

export type LoginRequest = {
  email: string
  password: string
}

export type LoginUserDto = {
  id: string
  email: string
  firstName: string
  lastName: string
  role: 'admin' | 'manager'
  organizationId: string
}

export type LoginResponse = {
  accessToken: string
  user: LoginUserDto
}

export const authApi = {
  login: (payload: LoginRequest) =>
    post<LoginResponse, LoginRequest>('/auth/login', payload),
  logout: () => post<void, void>('/auth/logout'),
}

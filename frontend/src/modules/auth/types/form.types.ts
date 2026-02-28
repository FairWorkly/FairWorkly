export interface LoginFormData {
  email: string
  password: string
  rememberMe: boolean
}

export interface SignupFormData {
  firstName: string
  lastName: string
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
  confirmPassword: string
}

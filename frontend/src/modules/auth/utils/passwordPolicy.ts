export const PASSWORD_POLICY_HINT =
  'Use 8-128 characters with both letters and numbers'

export function isPasswordPolicyValid(password: string): boolean {
  return (
    password.length >= 8 &&
    password.length <= 128 &&
    /[A-Za-z]/.test(password) &&
    /\d/.test(password)
  )
}

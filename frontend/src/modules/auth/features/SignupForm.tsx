import { useState } from 'react'
import TextField from '@mui/material/TextField'
import ArrowForwardIcon from '@mui/icons-material/ArrowForward'
import type { SignupFormData } from '../types'
import {
  AuthFormContainer,
  GoogleButton,
  GoogleIcon,
  FormDivider,
  FormDividerText,
  AuthFieldset,
  FormRow,
  StrengthBar,
  StrengthFill,
  StrengthText,
  FormActions,
  SubmitButton,
  FormTerms,
} from '../ui'

interface SignupFormProps {
  onSubmit: (values: SignupFormData) => void
  onGoogleLogin: () => void
  isSubmitting?: boolean
  isGoogleLoading?: boolean
}

function getPasswordStrength(
  password: string
): 'weak' | 'medium' | 'strong' | '' {
  if (!password) return ''
  let strength = 0
  if (password.length >= 8) strength++
  if (/[A-Z]/.test(password) && /[a-z]/.test(password)) strength++
  if (/[0-9]/.test(password) && /[^A-Za-z0-9]/.test(password)) strength++
  if (strength === 1) return 'weak'
  if (strength === 2) return 'medium'
  if (strength === 3) return 'strong'
  return 'weak'
}

function getStrengthText(strength: 'weak' | 'medium' | 'strong' | ''): string {
  switch (strength) {
    case 'weak':
      return 'Weak - add more variety'
    case 'medium':
      return 'Medium - almost there'
    case 'strong':
      return 'Strong password!'
    default:
      return 'Use 8+ characters with letters, numbers & symbols'
  }
}

export function SignupForm({
  onSubmit,
  onGoogleLogin,
  isSubmitting = false,
  isGoogleLoading = false,
}: SignupFormProps) {
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [companyName, setCompanyName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const passwordStrength = getPasswordStrength(password)
  const passwordsMatch = confirmPassword !== '' && confirmPassword === password
  const isSubmitDisabled = isSubmitting || isGoogleLoading || !passwordsMatch
  const isGoogleDisabled = isGoogleLoading

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSubmit({
      firstName,
      lastName,
      companyName,
      email,
      password,
      confirmPassword,
    })
  }

  return (
    <AuthFormContainer onSubmit={handleSubmit}>
      <GoogleButton
        type="button"
        onClick={onGoogleLogin}
        disabled={isGoogleDisabled}
      >
        <GoogleIcon />
        {isGoogleLoading ? 'Signing up...' : 'Continue with Google'}
      </GoogleButton>

      <FormDivider>
        <FormDividerText>or sign up with email</FormDividerText>
      </FormDivider>

      <AuthFieldset aria-label="Account details">
        <FormRow>
          <TextField
            label="First Name"
            placeholder="John"
            required
            fullWidth
            autoComplete="given-name"
            value={firstName}
            onChange={e => setFirstName(e.target.value)}
          />
          <TextField
            label="Last Name"
            placeholder="Smith"
            required
            fullWidth
            autoComplete="family-name"
            value={lastName}
            onChange={e => setLastName(e.target.value)}
          />
        </FormRow>

        <TextField
          label="Company Name"
          placeholder="Your Company Pty Ltd"
          required
          fullWidth
          autoComplete="organization"
          value={companyName}
          onChange={e => setCompanyName(e.target.value)}
        />

        <TextField
          label="Work Email"
          type="email"
          placeholder="you@company.com"
          required
          fullWidth
          autoComplete="email"
          value={email}
          onChange={e => setEmail(e.target.value)}
        />

        <div>
          <TextField
            label="Password"
            type="password"
            placeholder="Create a password"
            required
            fullWidth
            value={password}
            onChange={e => setPassword(e.target.value)}
            autoComplete="new-password"
          />
          <StrengthBar>
            <StrengthFill strength={passwordStrength} />
          </StrengthBar>
          <StrengthText>{getStrengthText(passwordStrength)}</StrengthText>
        </div>

        <TextField
          label="Confirm Password"
          type="password"
          placeholder="Repeat your password"
          required
          fullWidth
          value={confirmPassword}
          onChange={e => setConfirmPassword(e.target.value)}
          error={!passwordsMatch}
          helperText={!passwordsMatch ? 'Passwords do not match' : ' '}
          autoComplete="new-password"
        />
      </AuthFieldset>

      <FormActions>
        <SubmitButton type="submit" disabled={isSubmitDisabled}>
          {isSubmitting ? 'Creating account...' : 'Create Account'}
          <ArrowForwardIcon fontSize="small" />
        </SubmitButton>
      </FormActions>

      <FormTerms>
        By signing up, you agree to our <a href="#">Terms of Service</a> and{' '}
        <a href="#">Privacy Policy</a>.
      </FormTerms>
    </AuthFormContainer>
  )
}

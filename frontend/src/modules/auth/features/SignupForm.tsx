import { useState } from 'react'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
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
  StepIndicator,
  FormActions,
  SubmitButton,
  FormLink,
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

function isPasswordPolicyValid(password: string): boolean {
  return password.length >= 8 && /[A-Za-z]/.test(password) && /\d/.test(password)
}

export function SignupForm({
  onSubmit,
  onGoogleLogin,
  isSubmitting = false,
  isGoogleLoading = false,
}: SignupFormProps) {
  const [step, setStep] = useState(1)
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [companyName, setCompanyName] = useState('')
  const [abn, setAbn] = useState('')
  const [industryType, setIndustryType] = useState('')
  const [addressLine1, setAddressLine1] = useState('')
  const [addressLine2, setAddressLine2] = useState('')
  const [suburb, setSuburb] = useState('')
  const [state, setState] = useState('VIC')
  const [postcode, setPostcode] = useState('')
  const [contactEmail, setContactEmail] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const passwordStrength = getPasswordStrength(password)
  const isPasswordValid = isPasswordPolicyValid(password)
  const passwordsMatch = confirmPassword !== '' && confirmPassword === password
  const showPasswordPolicyError = password !== '' && !isPasswordValid
  const showConfirmMismatch = confirmPassword !== '' && !passwordsMatch
  const isAbnValid = /^\d{11}$/.test(abn)
  const isPostcodeValid = /^\d{4}$/.test(postcode)
  const canProceedToUserStep =
    companyName.trim() !== '' &&
    abn.trim() !== '' &&
    industryType.trim() !== '' &&
    addressLine1.trim() !== '' &&
    suburb.trim() !== '' &&
    state.trim() !== '' &&
    postcode.trim() !== '' &&
    contactEmail.trim() !== '' &&
    isAbnValid &&
    isPostcodeValid
  const canSubmit =
    firstName.trim() !== '' &&
    lastName.trim() !== '' &&
    email.trim() !== '' &&
    isPasswordValid &&
    confirmPassword.trim() !== '' &&
    passwordsMatch
  const isSubmitDisabled = isSubmitting || isGoogleLoading || !canSubmit
  const isGoogleDisabled = isGoogleLoading || isSubmitting

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!canSubmit) return

    onSubmit({
      firstName,
      lastName,
      companyName,
      abn,
      industryType,
      addressLine1,
      addressLine2: addressLine2 || undefined,
      suburb,
      state,
      postcode,
      contactEmail,
      email,
      password,
      confirmPassword,
    })
  }

  const handleNextStep = (e: React.MouseEvent<HTMLButtonElement>) => {
    const form = e.currentTarget.form
    if (!form?.reportValidity()) return
    if (!canProceedToUserStep) return
    setStep(2)
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
        {step === 1 ? (
          <>
            <StepIndicator>Step 1 of 2 — Organization details</StepIndicator>

            <TextField
              label="Company Name"
              placeholder="Your Company Pty Ltd"
              required
              fullWidth
              autoComplete="organization"
              value={companyName}
              onChange={(e) => setCompanyName(e.target.value)}
            />

            <TextField
              label="ABN"
              placeholder="11 digit ABN"
              required
              fullWidth
              value={abn}
              onChange={(e) => setAbn(e.target.value)}
              error={abn !== '' && !isAbnValid}
              helperText={abn !== '' && !isAbnValid ? 'ABN must be exactly 11 digits' : ' '}
              inputProps={{ inputMode: 'numeric', pattern: '\\d{11}' }}
            />

            <TextField
              label="Industry Type"
              placeholder="e.g. Retail, Hospitality"
              required
              fullWidth
              value={industryType}
              onChange={(e) => setIndustryType(e.target.value)}
            />

            <TextField
              label="Address Line 1"
              placeholder="Street address"
              required
              fullWidth
              autoComplete="address-line1"
              value={addressLine1}
              onChange={(e) => setAddressLine1(e.target.value)}
            />

            <TextField
              label="Address Line 2"
              placeholder="Apartment, suite, etc. (optional)"
              fullWidth
              autoComplete="address-line2"
              value={addressLine2}
              onChange={(e) => setAddressLine2(e.target.value)}
            />

            <TextField
              label="Suburb"
              placeholder="Suburb"
              required
              fullWidth
              autoComplete="address-level2"
              value={suburb}
              onChange={(e) => setSuburb(e.target.value)}
            />

            <FormRow>
              <TextField
                select
                label="State"
                required
                fullWidth
                value={state}
                onChange={(e) => setState(e.target.value)}
              >
                {['VIC', 'NSW', 'QLD', 'SA', 'WA', 'TAS', 'ACT', 'NT'].map((code) => (
                  <MenuItem key={code} value={code}>
                    {code}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                label="Postcode"
                placeholder="3000"
                required
                fullWidth
                value={postcode}
                onChange={(e) => setPostcode(e.target.value)}
                error={postcode !== '' && !isPostcodeValid}
                helperText={postcode !== '' && !isPostcodeValid ? 'Postcode must be 4 digits' : ' '}
                inputProps={{ inputMode: 'numeric', pattern: '\\d{4}' }}
              />
            </FormRow>

            <TextField
              label="Company Contact Email"
              type="email"
              placeholder="contact@company.com"
              required
              fullWidth
              autoComplete="email"
              value={contactEmail}
              onChange={(e) => setContactEmail(e.target.value)}
            />
          </>
        ) : (
          <>
            <StepIndicator>Step 2 of 2 — User details</StepIndicator>

            <FormRow>
              <TextField
                label="First Name"
                placeholder="John"
                required
                fullWidth
                autoComplete="given-name"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
              />
              <TextField
                label="Last Name"
                placeholder="Smith"
                required
                fullWidth
                autoComplete="family-name"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
              />
            </FormRow>

            <TextField
              label="Work Email"
              type="email"
              placeholder="you@company.com"
              required
              fullWidth
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />

            <div>
              <TextField
                label="Password"
                type="password"
                placeholder="Create a password"
                required
                fullWidth
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                error={showPasswordPolicyError}
                helperText={showPasswordPolicyError ? 'Use at least 8 characters with both letters and numbers' : ' '}
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
              onChange={(e) => setConfirmPassword(e.target.value)}
              error={showConfirmMismatch}
              helperText={showConfirmMismatch ? 'Passwords do not match' : ' '}
              autoComplete="new-password"
            />
          </>
        )}
      </AuthFieldset>

      <FormActions>
        {step === 1 ? (
          <SubmitButton type="button" onClick={handleNextStep}>
            Next
            <ArrowForwardIcon fontSize="small" />
          </SubmitButton>
        ) : (
          <>
            <FormLink type="button" onClick={() => setStep(1)}>
              Back to organization details
            </FormLink>
            <SubmitButton type="submit" disabled={isSubmitDisabled}>
              {isSubmitting ? 'Creating account...' : 'Create Account'}
              <ArrowForwardIcon fontSize="small" />
            </SubmitButton>
          </>
        )}
      </FormActions>

      <FormTerms>
        By signing up, you agree to our <a href="#">Terms of Service</a> and{' '}
        <a href="#">Privacy Policy</a>.
      </FormTerms>
    </AuthFormContainer>
  )
}

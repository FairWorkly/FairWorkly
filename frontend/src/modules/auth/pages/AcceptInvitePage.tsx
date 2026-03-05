import { useState } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import { TextField, Alert, CircularProgress } from '@mui/material'
import { useApiQuery } from '@/shared/hooks/useApiQuery'
import { settingsApi } from '@/services/settingsApi'
import { useAcceptInvitation } from '../hooks/useAcceptInvitation'
import {
  AuthHeader,
  AuthTitle,
  AuthSubtitle,
  AuthFieldset,
  AuthFormContainer,
  AuthErrorAlert,
  SubmitButton,
  FormActions,
  LoadingCenter,
  InputHint,
} from '../ui'

export function AcceptInvitePage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const token = searchParams.get('token')
  const acceptMutation = useAcceptInvitation()

  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [validationError, setValidationError] = useState<string | null>(null)
  const [success, setSuccess] = useState<{ email: string; fullName: string } | null>(null)

  // Pre-validate token on page load
  const tokenQuery = useApiQuery({
    queryKey: ['invitation-validate', token] as const,
    queryFn: () => settingsApi.validateInvitationToken(token!),
    enabled: !!token,
    retry: false,
  })

  if (!token) {
    return (
      <section>
        <AuthHeader>
          <AuthTitle>Invalid Invitation</AuthTitle>
        </AuthHeader>
        <Alert severity="error">
          No invitation token found. Please check the link you received or ask your admin to resend
          the invitation.
        </Alert>
      </section>
    )
  }

  if (tokenQuery.isLoading) {
    return (
      <section>
        <AuthHeader>
          <AuthTitle>Verifying Invitation...</AuthTitle>
        </AuthHeader>
        <LoadingCenter>
          <CircularProgress />
        </LoadingCenter>
      </section>
    )
  }

  if (tokenQuery.error) {
    return (
      <section>
        <AuthHeader>
          <AuthTitle>Invalid Invitation</AuthTitle>
        </AuthHeader>
        <Alert severity="error">
          {tokenQuery.error.message || 'This invitation link is invalid or has expired.'}
        </Alert>
      </section>
    )
  }

  if (success) {
    return (
      <section>
        <AuthHeader>
          <AuthTitle>Welcome, {success.fullName}!</AuthTitle>
          <AuthSubtitle>
            Your account has been set up successfully. You can now sign in with your email.
          </AuthSubtitle>
        </AuthHeader>
        <FormActions>
          <SubmitButton type="button" onClick={() => navigate('/login')}>
            Go to Login
          </SubmitButton>
        </FormActions>
      </section>
    )
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setValidationError(null)

    if (password.length < 8) {
      setValidationError('Password must be at least 8 characters.')
      return
    }
    if (password !== confirmPassword) {
      setValidationError('Passwords do not match.')
      return
    }

    acceptMutation.mutate(
      { token, password },
      {
        onSuccess: (data) => {
          setSuccess({ email: data.email, fullName: data.fullName })
        },
      }
    )
  }

  const invitee = tokenQuery.data

  return (
    <section>
      <AuthHeader>
        <AuthTitle>Set Your Password</AuthTitle>
        <AuthSubtitle>{invitee?.fullName ? `Hi ${invitee.fullName}, create a password to complete your account setup.` : 'Create a password to complete your account setup.'}</AuthSubtitle>
      </AuthHeader>

      {(validationError || acceptMutation.error) && (
        <AuthErrorAlert severity="error">
          {validationError || acceptMutation.error?.message || 'Something went wrong.'}
        </AuthErrorAlert>
      )}

      <AuthFormContainer onSubmit={handleSubmit}>
        <AuthFieldset disabled={acceptMutation.isPending}>
          <div>
            <TextField
              label="Password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              fullWidth
              required
              autoComplete="new-password"
            />
            <InputHint variant="caption">Minimum 8 characters</InputHint>
          </div>
          <TextField
            label="Confirm Password"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            fullWidth
            required
            autoComplete="new-password"
          />
          <FormActions>
            <SubmitButton type="submit" disabled={acceptMutation.isPending}>
              {acceptMutation.isPending && <CircularProgress size={18} color="inherit" />}
              {acceptMutation.isPending ? 'Setting up...' : 'Set Password & Join'}
            </SubmitButton>
          </FormActions>
        </AuthFieldset>
      </AuthFormContainer>
    </section>
  )
}

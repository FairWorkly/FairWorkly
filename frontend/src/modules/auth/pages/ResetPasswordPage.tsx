import { useEffect, useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Alert, CircularProgress, TextField } from '@mui/material'
import { useApiQuery } from '@/shared/hooks/useApiQuery'
import { authApi } from '@/services/authApi'
import { useResetPassword } from '../hooks'
import {
  PASSWORD_POLICY_HINT,
  isPasswordPolicyValid,
} from '../utils/passwordPolicy'
import {
  AuthErrorAlert,
  AuthFieldset,
  AuthFormContainer,
  AuthHeader,
  AuthSubtitle,
  AuthTitle,
  FormActions,
  InputHint,
  LoadingCenter,
  SubmitButton,
} from '../ui'

export function ResetPasswordPage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const tokenFromUrl = searchParams.get('token')
  const [token] = useState(tokenFromUrl)
  const resetPasswordMutation = useResetPassword()

  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [validationError, setValidationError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  useEffect(() => {
    if (tokenFromUrl) {
      navigate('/reset-password', { replace: true })
    }
  }, [navigate, tokenFromUrl])

  const tokenQuery = useApiQuery({
    queryKey: ['reset-password-validate', token] as const,
    queryFn: () => authApi.validateResetPasswordToken(token!),
    enabled: !!token,
    retry: false,
  })

  if (!token) {
    return (
      <section>
        <AuthHeader>
          <AuthTitle>Invalid Reset Link</AuthTitle>
        </AuthHeader>
        <Alert severity="error">
          No password reset token was found. Please request a new reset link.
        </Alert>
      </section>
    )
  }

  if (tokenQuery.isLoading) {
    return (
      <section>
        <AuthHeader>
          <AuthTitle>Verifying Reset Link...</AuthTitle>
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
          <AuthTitle>Invalid Reset Link</AuthTitle>
        </AuthHeader>
        <Alert severity="error">
          {tokenQuery.error.message ||
            'This password reset link is invalid or has expired.'}
        </Alert>
        <FormActions>
          <SubmitButton type="button" onClick={() => navigate('/login')}>
            Back to Login
          </SubmitButton>
        </FormActions>
      </section>
    )
  }

  if (success) {
    return (
      <section>
        <AuthHeader>
          <AuthTitle>Password Reset</AuthTitle>
          <AuthSubtitle>
            Your password has been updated. You can now sign in with your new
            password.
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

  const showPasswordPolicyError =
    password !== '' && !isPasswordPolicyValid(password)
  const showConfirmMismatch =
    confirmPassword !== '' && confirmPassword !== password

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setValidationError(null)

    if (!isPasswordPolicyValid(password)) {
      setValidationError(PASSWORD_POLICY_HINT)
      return
    }

    if (password !== confirmPassword) {
      setValidationError('Passwords do not match.')
      return
    }

    resetPasswordMutation.mutate(
      { token, password },
      {
        onSuccess: () => {
          setSuccess(true)
        },
      }
    )
  }

  return (
    <section>
      <AuthHeader>
        <AuthTitle>Reset Your Password</AuthTitle>
        <AuthSubtitle>Create a new password for your account.</AuthSubtitle>
      </AuthHeader>

      {validationError && (
        <AuthErrorAlert severity="error">{validationError}</AuthErrorAlert>
      )}
      {resetPasswordMutation.error && (
        <AuthErrorAlert severity="error">
          {resetPasswordMutation.error.message || 'Something went wrong.'}
        </AuthErrorAlert>
      )}

      <AuthFormContainer onSubmit={handleSubmit}>
        <AuthFieldset disabled={resetPasswordMutation.isPending}>
          <div>
            <TextField
              label="New Password"
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              fullWidth
              required
              autoComplete="new-password"
              error={showPasswordPolicyError}
              helperText={showPasswordPolicyError ? PASSWORD_POLICY_HINT : ' '}
            />
            <InputHint variant="caption">{PASSWORD_POLICY_HINT}</InputHint>
          </div>
          <TextField
            label="Confirm Password"
            type="password"
            value={confirmPassword}
            onChange={e => setConfirmPassword(e.target.value)}
            fullWidth
            required
            autoComplete="new-password"
            error={showConfirmMismatch}
            helperText={showConfirmMismatch ? 'Passwords do not match.' : ' '}
          />
          <FormActions>
            <SubmitButton
              type="submit"
              disabled={resetPasswordMutation.isPending}
            >
              {resetPasswordMutation.isPending && (
                <CircularProgress size={18} color="inherit" />
              )}
              {resetPasswordMutation.isPending
                ? 'Resetting...'
                : 'Reset Password'}
            </SubmitButton>
          </FormActions>
        </AuthFieldset>
      </AuthFormContainer>
    </section>
  )
}

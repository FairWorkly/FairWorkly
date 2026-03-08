import { useState } from 'react'
import TextField from '@mui/material/TextField'
import Dialog from '@mui/material/Dialog'
import CircularProgress from '@mui/material/CircularProgress'
import LockResetIcon from '@mui/icons-material/LockReset'
import CheckIcon from '@mui/icons-material/Check'
import CloseIcon from '@mui/icons-material/Close'
import SendIcon from '@mui/icons-material/Send'
import { useForgotPassword } from '../hooks'
import {
  ModalContent,
  ModalCloseButton,
  ModalIcon,
  ModalTitle,
  ModalSubtitle,
  ModalSuccessIcon,
  SubmitButton,
  FormLink,
  FormTerms,
  ModalFooter,
  ModalBody,
  FormActions,
  AuthErrorAlert,
} from '../ui'

interface ForgotPasswordModalProps {
  open: boolean
  onClose: () => void
}

export function ForgotPasswordModal({
  open,
  onClose,
}: ForgotPasswordModalProps) {
  const [step, setStep] = useState<'email' | 'success'>('email')
  const [email, setEmail] = useState('')
  const forgotPasswordMutation = useForgotPassword()

  const handleClose = () => {
    setStep('email')
    setEmail('')
    forgotPasswordMutation.reset()
    onClose()
  }

  const submitEmail = () => {
    const trimmedEmail = email.trim()
    if (!trimmedEmail) return

    forgotPasswordMutation.mutate(trimmedEmail, {
      onSuccess: () => {
        setStep('success')
      },
    })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    submitEmail()
  }

  const handleResend = () => {
    submitEmail()
  }

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <ModalContent>
        <ModalCloseButton onClick={handleClose} size="small">
          <CloseIcon fontSize="small" />
        </ModalCloseButton>

        {step === 'email' ? (
          <ModalBody>
            <ModalIcon>
              <LockResetIcon />
            </ModalIcon>
            <ModalTitle>Forgot Password?</ModalTitle>
            <ModalSubtitle>
              Enter your email and we&apos;ll send you a reset link.
            </ModalSubtitle>

            <form onSubmit={handleSubmit}>
              <TextField
                label="Email Address"
                type="email"
                placeholder="you@company.com"
                required
                fullWidth
                value={email}
                onChange={e => setEmail(e.target.value)}
              />

              {forgotPasswordMutation.error && (
                <AuthErrorAlert severity="error" sx={{ mt: 2 }}>
                  {forgotPasswordMutation.error.message}
                </AuthErrorAlert>
              )}

              <FormActions>
                <SubmitButton
                  type="submit"
                  disabled={forgotPasswordMutation.isPending}
                >
                  {forgotPasswordMutation.isPending ? (
                    <CircularProgress size={18} color="inherit" />
                  ) : (
                    <SendIcon fontSize="small" />
                  )}
                  {forgotPasswordMutation.isPending
                    ? 'Sending...'
                    : 'Send Reset Link'}
                </SubmitButton>
              </FormActions>
            </form>

            <ModalFooter>
              <FormLink type="button" onClick={handleClose}>
                Back to login
              </FormLink>
            </ModalFooter>
          </ModalBody>
        ) : (
          <ModalBody>
            <ModalSuccessIcon>
              <CheckIcon />
            </ModalSuccessIcon>
            <ModalTitle>Check Your Email</ModalTitle>
            <ModalSubtitle>
              Check your email for reset instructions
            </ModalSubtitle>

            {forgotPasswordMutation.error && (
              <AuthErrorAlert severity="error" sx={{ mb: 2 }}>
                {forgotPasswordMutation.error.message}
              </AuthErrorAlert>
            )}

            <SubmitButton type="button" onClick={handleClose}>
              Back to Login
            </SubmitButton>

            <FormTerms>
              Didn&apos;t receive the email?{' '}
              <FormLink
                type="button"
                onClick={handleResend}
                disabled={forgotPasswordMutation.isPending}
              >
                {forgotPasswordMutation.isPending ? 'Resending...' : 'Resend'}
              </FormLink>
            </FormTerms>
          </ModalBody>
        )}
      </ModalContent>
    </Dialog>
  )
}

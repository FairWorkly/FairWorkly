import { useState } from 'react'
import TextField from '@mui/material/TextField'
import Dialog from '@mui/material/Dialog'
import LockResetIcon from '@mui/icons-material/LockReset'
import CheckIcon from '@mui/icons-material/Check'
import CloseIcon from '@mui/icons-material/Close'
import SendIcon from '@mui/icons-material/Send'
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
} from '../ui'

interface ForgotPasswordModalProps {
  open: boolean
  onClose: () => void
}

export function ForgotPasswordModal({ open, onClose }: ForgotPasswordModalProps) {
  const [step, setStep] = useState<'email' | 'success'>('email')
  const [email, setEmail] = useState('')

  const handleClose = () => {
    setStep('email')
    setEmail('')
    onClose()
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (email) {
      // TODO: Implement actual password reset logic
      setStep('success')
    }
  }

  const handleResend = () => {
    setStep('email')
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
              Enter your email and we'll send you a reset link.
            </ModalSubtitle>

            <form onSubmit={handleSubmit}>
              <TextField
                label="Email Address"
                type="email"
                placeholder="you@company.com"
                required
                fullWidth
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />

              <FormActions>
                <SubmitButton type="submit">
                  Send Reset Link
                  <SendIcon fontSize="small" />
                </SubmitButton>
              </FormActions>
            </form>

            <ModalFooter>
              <FormLink type="button" onClick={handleClose}>
                ← Back to login
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
              We've sent a password reset link to <strong>{email}</strong>
            </ModalSubtitle>

            <SubmitButton type="button" onClick={handleClose}>
              Back to Login
            </SubmitButton>

            <FormTerms>
              Didn't receive the email?{' '}
              <FormLink type="button" onClick={handleResend}>
                Resend
              </FormLink>
            </FormTerms>
          </ModalBody>
        )}
      </ModalContent>
    </Dialog>
  )
}

import { useEffect, useRef, useState } from 'react'
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
  const [submittedEmail, setSubmittedEmail] = useState<string | null>(null)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const { mutateAsync, reset: resetMutation } = useForgotPassword()
  const requestVersionRef = useRef(0)
  const wasOpenRef = useRef(open)

  const getErrorMessage = (error: unknown) => {
    if (
      typeof error === 'object' &&
      error !== null &&
      'message' in error &&
      typeof error.message === 'string' &&
      error.message.trim()
    ) {
      return error.message
    }

    return 'Something went wrong.'
  }

  useEffect(() => {
    if (wasOpenRef.current && !open) {
      requestVersionRef.current += 1
      setStep('email')
      setEmail('')
      setSubmittedEmail(null)
      setErrorMessage(null)
      setIsSubmitting(false)
      resetMutation()
    }

    wasOpenRef.current = open
  }, [open, resetMutation])

  const handleClose = () => {
    requestVersionRef.current += 1
    setStep('email')
    setEmail('')
    setSubmittedEmail(null)
    setErrorMessage(null)
    setIsSubmitting(false)
    resetMutation()
    onClose()
  }

  const submitEmail = async (nextEmail = email) => {
    const trimmedEmail = nextEmail.trim()
    if (!trimmedEmail || isSubmitting) return

    const requestVersion = requestVersionRef.current + 1
    requestVersionRef.current = requestVersion
    setEmail(trimmedEmail)
    setSubmittedEmail(trimmedEmail)
    setErrorMessage(null)
    setIsSubmitting(true)

    try {
      await mutateAsync(trimmedEmail)

      if (requestVersionRef.current !== requestVersion) {
        return
      }

      setStep('success')
    } catch (error) {
      if (requestVersionRef.current !== requestVersion) {
        return
      }

      setErrorMessage(getErrorMessage(error))
    } finally {
      if (requestVersionRef.current === requestVersion) {
        setIsSubmitting(false)
      }
    }
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    void submitEmail()
  }

  const handleResend = () => {
    void submitEmail(submittedEmail ?? email)
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
                disabled={isSubmitting}
              />

              {errorMessage && (
                <AuthErrorAlert severity="error" sx={{ mt: 2 }}>
                  {errorMessage}
                </AuthErrorAlert>
              )}

              <FormActions>
                <SubmitButton type="submit" disabled={isSubmitting}>
                  {isSubmitting ? (
                    <CircularProgress size={18} color="inherit" />
                  ) : (
                    <SendIcon fontSize="small" />
                  )}
                  {isSubmitting ? 'Sending...' : 'Send Reset Link'}
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

            {errorMessage && (
              <AuthErrorAlert severity="error" sx={{ mb: 2 }}>
                {errorMessage}
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
                disabled={isSubmitting}
              >
                {isSubmitting ? 'Resending...' : 'Resend'}
              </FormLink>
            </FormTerms>
          </ModalBody>
        )}
      </ModalContent>
    </Dialog>
  )
}

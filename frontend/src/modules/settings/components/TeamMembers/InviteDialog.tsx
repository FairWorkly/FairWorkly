import { useState } from 'react'
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  InputAdornment,
  MenuItem,
  TextField,
  Typography,
} from '@mui/material'
import ContentCopyIcon from '@mui/icons-material/ContentCopy'
import CheckIcon from '@mui/icons-material/Check'
import type { InviteTeamMemberRequest } from '../../types/teamMembers.types'
import { ROLE_OPTIONS } from '../../types/teamMembers.types'

interface InviteDialogProps {
  open: boolean
  isSubmitting: boolean
  inviteLink: string | null
  error: string | null
  onSubmit: (data: InviteTeamMemberRequest) => void
  onClose: () => void
}

const initialForm: InviteTeamMemberRequest = {
  email: '',
  firstName: '',
  lastName: '',
  role: 'Manager',
}

export function InviteDialog({
  open,
  isSubmitting,
  inviteLink,
  error,
  onSubmit,
  onClose,
}: InviteDialogProps) {
  const [form, setForm] = useState<InviteTeamMemberRequest>(initialForm)
  const [copied, setCopied] = useState(false)
  const [copyError, setCopyError] = useState(false)

  const handleClose = () => {
    if (isSubmitting) return
    setForm(initialForm)
    setCopied(false)
    setCopyError(false)
    onClose()
  }

  const handleSubmit = () => {
    onSubmit(form)
  }

  const handleCopy = async () => {
    if (!inviteLink) return
    try {
      await navigator.clipboard.writeText(inviteLink)
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    } catch {
      setCopyError(true)
      setTimeout(() => setCopyError(false), 3000)
    }
  }

  const isValidEmail = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())

  const isFormValid =
    isValidEmail(form.email) &&
    form.firstName.trim() !== '' &&
    form.lastName.trim() !== '' &&
    !!form.role

  // Success step: show invite link
  if (inviteLink) {
    return (
      <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogTitle>Invitation Sent</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Share this link with the team member to complete their registration:
          </Typography>
          <TextField
            fullWidth
            value={inviteLink}
            slotProps={{
              input: {
                readOnly: true,
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={handleCopy} size="small">
                      {copied ? <CheckIcon color="success" /> : <ContentCopyIcon />}
                    </IconButton>
                  </InputAdornment>
                ),
              },
            }}
            size="small"
          />
          {copyError && (
            <Typography variant="caption" color="error" sx={{ mt: 0.5, display: 'block' }}>
              Could not copy — please copy the link manually.
            </Typography>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} variant="contained">
            Done
          </Button>
        </DialogActions>
      </Dialog>
    )
  }

  // Form step: collect invite details
  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Invite Team Member</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          <TextField
            label="Email"
            type="email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            fullWidth
            required
            disabled={isSubmitting}
          />
          <Box sx={{ display: 'flex', gap: 2 }}>
            <TextField
              label="First Name"
              value={form.firstName}
              onChange={(e) => setForm({ ...form, firstName: e.target.value })}
              fullWidth
              required
              disabled={isSubmitting}
            />
            <TextField
              label="Last Name"
              value={form.lastName}
              onChange={(e) => setForm({ ...form, lastName: e.target.value })}
              fullWidth
              required
              disabled={isSubmitting}
            />
          </Box>
          <TextField
            label="Role"
            select
            value={form.role}
            onChange={(e) =>
              setForm({ ...form, role: e.target.value as 'Admin' | 'Manager' })
            }
            fullWidth
            required
            disabled={isSubmitting}
          >
            {ROLE_OPTIONS.map((opt) => (
              <MenuItem key={opt.value} value={opt.value}>
                {opt.label}
              </MenuItem>
            ))}
          </TextField>
          {error && (
            <Typography variant="body2" color="error">
              {error}
            </Typography>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} variant="outlined" color="inherit" disabled={isSubmitting}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={!isFormValid || isSubmitting}
          startIcon={isSubmitting ? <CircularProgress size={16} /> : undefined}
        >
          {isSubmitting ? 'Sending...' : 'Send Invite'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

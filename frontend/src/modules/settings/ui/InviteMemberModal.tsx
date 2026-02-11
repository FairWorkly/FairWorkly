import { useState } from 'react'
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Stack,
} from '@mui/material'
import { RoleDropdown } from './RoleDropdown'
import type { InviteMemberFormData, TeamMemberRole } from '@/modules/settings/types'

interface Props {
  open: boolean
  onSubmit: (data: InviteMemberFormData) => void
  onClose: () => void
}

export function InviteMemberModal({ open, onSubmit, onClose }: Props) {
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [role, setRole] = useState<TeamMemberRole>('Manager')

  const resetForm = () => {
    setName('')
    setEmail('')
    setRole('Manager')
  }

  const handleSubmit = () => {
    if (name.trim() && email.trim()) {
      onSubmit({ name: name.trim(), email: email.trim(), role })
      resetForm()
      onClose()
    }
  }

  const handleClose = () => {
    resetForm()
    onClose()
  }

  const isValid = name.trim().length > 0 && email.trim().length > 0

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Invite Team Member</DialogTitle>
      <DialogContent>
        <Stack spacing={3} sx={{ mt: 1 }}>
          <TextField
            label="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            fullWidth
            required
            autoFocus
          />
          <TextField
            label="Email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            fullWidth
            required
          />
          <RoleDropdown value={role} onChange={setRole} />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={!isValid}>
          Send Invitation
        </Button>
      </DialogActions>
    </Dialog>
  )
}

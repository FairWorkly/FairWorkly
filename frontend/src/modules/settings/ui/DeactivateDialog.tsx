import React from 'react'
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
} from '@mui/material'
import type { TeamMember } from '../types/settings.types'
import { SETTINGS_LABELS } from '../constants/settings.constants'

interface DeactivateDialogProps {
  open: boolean
  member: TeamMember | null
  onClose: () => void
  onConfirm: () => void
}

export const DeactivateDialog: React.FC<DeactivateDialogProps> = ({
  open,
  member,
  onClose,
  onConfirm,
}) => {
  if (!member) return null

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{SETTINGS_LABELS.MODALS.DEACTIVATE_TITLE}</DialogTitle>
      <DialogContent>
        <DialogContentText>
          {SETTINGS_LABELS.MODALS.DEACTIVATE_CONFIRM}
        </DialogContentText>
        <DialogContentText sx={{ mt: 2, fontWeight: 'bold' }}>
          {member.name} ({member.email})
        </DialogContentText>
      </DialogContent>
      <DialogActions sx={{ p: 2 }}>
        <Button onClick={onClose} color="inherit">
          {SETTINGS_LABELS.ACTIONS.CANCEL}
        </Button>
        <Button onClick={onConfirm} color="error" variant="contained">
          {SETTINGS_LABELS.ACTIONS.DEACTIVATE}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

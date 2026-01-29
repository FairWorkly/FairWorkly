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

interface DeactivateDialogProps {
  open: boolean
  member: TeamMember | null
  onClose: () => void
  onConfirm: () => void
}

const DEACTIVATE_DIALOG_LABELS = {
  TITLE: 'Deactivate Member',
  CONFIRM_MESSAGE: 'Are you sure you want to deactivate this team member?',
  ACTIONS: {
    CANCEL: 'Cancel',
    DEACTIVATE: 'Deactivate',
  },
} as const

export const DeactivateDialog: React.FC<DeactivateDialogProps> = ({
  open,
  member,
  onClose,
  onConfirm,
}) => {
  if (!member) return null

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>{DEACTIVATE_DIALOG_LABELS.TITLE}</DialogTitle>
      <DialogContent>
        <DialogContentText>
          {DEACTIVATE_DIALOG_LABELS.CONFIRM_MESSAGE}
        </DialogContentText>
        <DialogContentText sx={{ mt: 2, fontWeight: 'bold' }}>
          {member.name} ({member.email})
        </DialogContentText>
      </DialogContent>
      <DialogActions sx={{ p: 2 }}>
        <Button onClick={onClose} color="inherit">
          {DEACTIVATE_DIALOG_LABELS.ACTIONS.CANCEL}
        </Button>
        <Button onClick={onConfirm} color="error" variant="contained">
          {DEACTIVATE_DIALOG_LABELS.ACTIONS.DEACTIVATE}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

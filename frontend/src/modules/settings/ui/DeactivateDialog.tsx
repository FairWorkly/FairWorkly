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

const SETTINGS_LABELS = {
  TEAM_MEMBERS: {
    TITLE: 'Team Members',
    DESCRIPTION: 'Manage your team members and their access levels',
    INVITE_BUTTON: 'Invite Member',
    EMPTY_STATE: 'No team members yet. Invite your first team member.',
  },
  TABLE_HEADERS: {
    NAME: 'Name',
    EMAIL: 'Email',
    ROLE: 'Role',
    STATUS: 'Status',
    LAST_LOGIN: 'Last Login',
    ACTIONS: 'Actions',
  },
  MODALS: {
    INVITE_TITLE: 'Invite Team Member',
    DEACTIVATE_TITLE: 'Deactivate Member',
    DEACTIVATE_CONFIRM: 'Are you sure you want to deactivate this team member?',
  },
  FORM: {
    NAME_LABEL: 'Full Name',
    NAME_PLACEHOLDER: 'Enter full name',
    EMAIL_LABEL: 'Email Address',
    EMAIL_PLACEHOLDER: 'Enter email address',
    ROLE_LABEL: 'Role',
  },
  ACTIONS: {
    CANCEL: 'Cancel',
    INVITE: 'Send Invite',
    DEACTIVATE: 'Deactivate',
    CONFIRM: 'Confirm',
  },
  ROLES: {
    ADMIN: 'Admin',
    MANAGER: 'Manager',
  },
  STATUS: {
    ACTIVE: 'Active',
    INACTIVE: 'Inactive',
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

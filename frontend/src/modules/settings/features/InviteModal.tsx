import React, { useState } from 'react'
import {
  DialogTitle,
  TextField,
  Button,
  MenuItem,
} from '@mui/material'
import type {
  InviteTeamMemberFormData,
  TeamMemberRole,
} from '@/modules/settings/types'
import {
  StyledDialog,
  StyledDialogContent,
  StyledDialogActions,
} from '@/modules/settings/ui'

interface InviteModalProps {
  open: boolean
  onClose: () => void
  onSubmit: (data: InviteTeamMemberFormData) => void
}

const TEAM_MEMBER_ROLES: TeamMemberRole[] = ['Admin', 'Manager']

const INVITE_MODAL_LABELS = {
  TITLE: 'Invite Team Member',
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
  },
} as const

const INITIAL_FORM_STATE: InviteTeamMemberFormData = {
  name: '',
  email: '',
  role: 'Manager',
}

interface InviteModalFormProps {
  onClose: () => void
  onSubmit: (data: InviteTeamMemberFormData) => void
}

const InviteModalForm: React.FC<InviteModalFormProps> = ({
  onClose,
  onSubmit,
}) => {
  const [formData, setFormData] =
    useState<InviteTeamMemberFormData>(INITIAL_FORM_STATE)
  const [errors, setErrors] = useState<{ name?: string; email?: string }>({})

  const handleChange = (field: keyof InviteTeamMemberFormData, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }))
    // Clear error when user types
    if (errors[field as keyof typeof errors]) {
      setErrors((prev) => ({ ...prev, [field]: undefined }))
    }
  }

  const validate = () => {
    const newErrors: { name?: string; email?: string } = {}
    if (!formData.name.trim()) {
      newErrors.name = 'Name is required'
    }
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required'
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Invalid email format'
    }
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (validate()) {
      try {
        await onSubmit(formData)
        onClose()
      } catch (error) {
        // Handle submission error (e.g., show toast)
        console.error('Failed to invite member:', error)
      }
    }
  }

  return (
    <>
      <DialogTitle>{INVITE_MODAL_LABELS.TITLE}</DialogTitle>
      <form onSubmit={handleSubmit}>
        <StyledDialogContent>
          <TextField
            label={INVITE_MODAL_LABELS.FORM.NAME_LABEL}
            placeholder={INVITE_MODAL_LABELS.FORM.NAME_PLACEHOLDER}
            value={formData.name}
            onChange={(e) => handleChange('name', e.target.value)}
            error={!!errors.name}
            helperText={errors.name}
            fullWidth
            autoFocus
          />
          <TextField
            label={INVITE_MODAL_LABELS.FORM.EMAIL_LABEL}
            placeholder={INVITE_MODAL_LABELS.FORM.EMAIL_PLACEHOLDER}
            value={formData.email}
            onChange={(e) => handleChange('email', e.target.value)}
            error={!!errors.email}
            helperText={errors.email}
            fullWidth
          />
          <TextField
            select
            label={INVITE_MODAL_LABELS.FORM.ROLE_LABEL}
            value={formData.role}
            onChange={(e) =>
              handleChange('role', e.target.value as TeamMemberRole)
            }
            fullWidth
          >
            {TEAM_MEMBER_ROLES.map((role) => (
              <MenuItem key={role} value={role}>
                {role}
              </MenuItem>
            ))}
          </TextField>
        </StyledDialogContent>
        <StyledDialogActions>
          <Button onClick={onClose}>{INVITE_MODAL_LABELS.ACTIONS.CANCEL}</Button>
          <Button type="submit" variant="contained" color="primary">
            {INVITE_MODAL_LABELS.ACTIONS.INVITE}
          </Button>
        </StyledDialogActions>
      </form>
    </>
  )
}

export const InviteModal: React.FC<InviteModalProps> = ({
  open,
  onClose,
  onSubmit,
}) => {
  return (
    <StyledDialog open={open} onClose={onClose}>
      {open && <InviteModalForm onClose={onClose} onSubmit={onSubmit} />}
    </StyledDialog>
  )
}

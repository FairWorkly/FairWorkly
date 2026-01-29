import React, { useState, useEffect } from 'react'
import {
  DialogTitle,
  TextField,
  Button,
  MenuItem,
} from '@mui/material'
import type {
  InviteTeamMemberFormData,
  TeamMemberRole,
} from '../types/settings.types'
import {
  SETTINGS_LABELS,
  TEAM_MEMBER_ROLES,
} from '../constants/settings.constants'
import {
  StyledDialog,
  StyledDialogContent,
  StyledDialogActions,
} from './InviteModal.styles'

interface InviteModalProps {
  open: boolean
  onClose: () => void
  onSubmit: (data: InviteTeamMemberFormData) => void
}

const INITIAL_FORM_STATE: InviteTeamMemberFormData = {
  name: '',
  email: '',
  role: 'Manager',
}

export const InviteModal: React.FC<InviteModalProps> = ({
  open,
  onClose,
  onSubmit,
}) => {
  const [formData, setFormData] =
    useState<InviteTeamMemberFormData>(INITIAL_FORM_STATE)
  const [errors, setErrors] = useState<{ name?: string; email?: string }>({})

  // Reset form when modal opens/closes
  useEffect(() => {
    if (open) {
      setFormData(INITIAL_FORM_STATE)
      setErrors({})
    }
  }, [open])

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

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (validate()) {
      onSubmit(formData)
      onClose()
    }
  }

  return (
    <StyledDialog open={open} onClose={onClose}>
      <DialogTitle>{SETTINGS_LABELS.MODALS.INVITE_TITLE}</DialogTitle>
      <form onSubmit={handleSubmit}>
        <StyledDialogContent>
          <TextField
            label={SETTINGS_LABELS.FORM.NAME_LABEL}
            placeholder={SETTINGS_LABELS.FORM.NAME_PLACEHOLDER}
            value={formData.name}
            onChange={(e) => handleChange('name', e.target.value)}
            error={!!errors.name}
            helperText={errors.name}
            fullWidth
            autoFocus
          />
          <TextField
            label={SETTINGS_LABELS.FORM.EMAIL_LABEL}
            placeholder={SETTINGS_LABELS.FORM.EMAIL_PLACEHOLDER}
            value={formData.email}
            onChange={(e) => handleChange('email', e.target.value)}
            error={!!errors.email}
            helperText={errors.email}
            fullWidth
          />
          <TextField
            select
            label={SETTINGS_LABELS.FORM.ROLE_LABEL}
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
          <Button onClick={onClose}>{SETTINGS_LABELS.ACTIONS.CANCEL}</Button>
          <Button type="submit" variant="contained" color="primary">
            {SETTINGS_LABELS.ACTIONS.INVITE}
          </Button>
        </StyledDialogActions>
      </form>
    </StyledDialog>
  )
}

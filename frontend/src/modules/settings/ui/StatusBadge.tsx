import React from 'react'
import { Box, Typography } from '@mui/material'
import { styled } from '@/styles/styled'
import type { TeamMemberStatus } from '../types/settings.types'

interface StatusBadgeProps {
  status: TeamMemberStatus
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

const StyledBadge = styled(Box, {
  shouldForwardProp: (prop) => prop !== 'status',
})<{ status: TeamMemberStatus }>(({ theme, status }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(0.5, 1.5),
  borderRadius: theme.shape.borderRadius,
  backgroundColor:
    status === 'Active'
      ? theme.palette.success.light
      : theme.palette.action.hover,
  color:
    status === 'Active'
      ? theme.palette.success.dark
      : theme.palette.text.disabled,
  fontWeight: 500,
  fontSize: '0.75rem',
}))

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
  return (
    <StyledBadge status={status}>
      <Typography variant="caption" fontWeight="bold">
        {status === 'Active'
          ? SETTINGS_LABELS.STATUS.ACTIVE
          : SETTINGS_LABELS.STATUS.INACTIVE}
      </Typography>
    </StyledBadge>
  )
}

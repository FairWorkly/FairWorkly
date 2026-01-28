import React from 'react'
import { Box, Typography } from '@mui/material'
import { styled } from '@/styles/styled'
import type { TeamMemberStatus } from '../types/settings.types'
import { SETTINGS_LABELS } from '../constants/settings.constants'

interface StatusBadgeProps {
  status: TeamMemberStatus
}

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

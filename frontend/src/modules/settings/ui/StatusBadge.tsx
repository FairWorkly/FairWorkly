import { Chip } from '@mui/material'
import { styled } from '@/styles/styled'
import type { TeamMemberStatus } from '@/modules/settings/types'

interface Props {
  status: TeamMemberStatus
}

const StyledChip = styled(Chip, {
  shouldForwardProp: (prop) => prop !== 'isActive',
})<{ isActive: boolean }>(({ theme, isActive }) => ({
  fontWeight: theme.typography.fontWeightMedium,
  backgroundColor: isActive
    ? theme.palette.success.light
    : theme.palette.grey[200],
  color: isActive
    ? theme.palette.success.dark
    : theme.palette.text.secondary,
}))

export function StatusBadge({ status }: Props) {
  const isActive = status === 'Active'
  return <StyledChip label={status} size="small" isActive={isActive} />
}

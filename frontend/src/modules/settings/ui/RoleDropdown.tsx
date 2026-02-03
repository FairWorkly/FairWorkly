import { Select, MenuItem } from '@mui/material'
import { styled } from '@/styles/styled'
import type { TeamMemberRole } from '../types'

interface Props {
  value: TeamMemberRole
  onChange: (role: TeamMemberRole) => void
  disabled?: boolean
}

const StyledFormControl = styled('div')(({ theme }) => ({
  minWidth: theme.spacing(14),
}))

export function RoleDropdown({ value, onChange, disabled }: Props) {
  return (
    <StyledFormControl>
      <Select
        value={value}
        onChange={(e) => onChange(e.target.value as TeamMemberRole)}
        disabled={disabled}
      >
        <MenuItem value="Admin">Admin</MenuItem>
        <MenuItem value="Manager">Manager</MenuItem>
      </Select>
    </StyledFormControl>
  )
}

import { Select, MenuItem, FormControl } from '@mui/material'
import type { TeamMemberRole } from '../types'

interface Props {
  value: TeamMemberRole
  onChange: (role: TeamMemberRole) => void
  disabled?: boolean
}

export function RoleDropdown({ value, onChange, disabled }: Props) {
  return (
    <FormControl size="small" sx={{ minWidth: 110 }}>
      <Select
        value={value}
        onChange={(e) => onChange(e.target.value as TeamMemberRole)}
        disabled={disabled}
      >
        <MenuItem value="Admin">Admin</MenuItem>
        <MenuItem value="Manager">Manager</MenuItem>
      </Select>
    </FormControl>
  )
}

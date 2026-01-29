import React from 'react'
import { MenuItem, Select, type SelectChangeEvent, FormControl } from '@mui/material'
import type { TeamMemberRole } from '../types/settings.types'

interface RoleDropdownProps {
  value: TeamMemberRole
  onChange: (role: TeamMemberRole) => void
  disabled?: boolean
}

const TEAM_MEMBER_ROLES: TeamMemberRole[] = ['Admin', 'Manager']

export const RoleDropdown: React.FC<RoleDropdownProps> = ({
  value,
  onChange,
  disabled = false,
}) => {
  const handleChange = (event: SelectChangeEvent) => {
    onChange(event.target.value as TeamMemberRole)
  }

  return (
    <FormControl size="small" fullWidth variant="outlined">
      <Select
        value={value}
        onChange={handleChange}
        disabled={disabled}
        sx={{
          fontSize: '0.875rem',
          height: 32,
          '& .MuiSelect-select': {
            paddingTop: '4px',
            paddingBottom: '4px',
          },
        }}
      >
        {TEAM_MEMBER_ROLES.map((role) => (
          <MenuItem key={role} value={role} sx={{ fontSize: '0.875rem' }}>
            {role}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  )
}

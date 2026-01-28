import React from 'react'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  Tooltip,
} from '@mui/material'
import { PersonOff } from '@mui/icons-material'
import type { TeamMember, TeamMemberRole } from '../types/settings.types'
import { SETTINGS_LABELS } from '../constants/settings.constants'
import { StatusBadge } from './StatusBadge'
import { RoleDropdown } from './RoleDropdown'
import {
  StyledTableContainer,
  HeaderCell,
  StyledTableRow,
  ActionButton,
} from './TeamMembersTable.styles'

interface TeamMembersTableProps {
  members: TeamMember[]
  onRoleChange: (memberId: string, newRole: TeamMemberRole) => void
  onDeactivate: (member: TeamMember) => void
}

export const TeamMembersTable: React.FC<TeamMembersTableProps> = ({
  members,
  onRoleChange,
  onDeactivate,
}) => {
  return (
    <StyledTableContainer>
      <Table>
        <TableHead>
          <StyledTableRow>
            <HeaderCell>{SETTINGS_LABELS.TABLE_HEADERS.NAME}</HeaderCell>
            <HeaderCell>{SETTINGS_LABELS.TABLE_HEADERS.EMAIL}</HeaderCell>
            <HeaderCell>{SETTINGS_LABELS.TABLE_HEADERS.ROLE}</HeaderCell>
            <HeaderCell>{SETTINGS_LABELS.TABLE_HEADERS.STATUS}</HeaderCell>
            <HeaderCell>{SETTINGS_LABELS.TABLE_HEADERS.LAST_LOGIN}</HeaderCell>
            <HeaderCell align="right">
              {SETTINGS_LABELS.TABLE_HEADERS.ACTIONS}
            </HeaderCell>
          </StyledTableRow>
        </TableHead>
        <TableBody>
          {members.map((member) => (
            <StyledTableRow key={member.id}>
              <TableCell>{member.name}</TableCell>
              <TableCell>{member.email}</TableCell>
              <TableCell>
                <RoleDropdown
                  value={member.role}
                  onChange={(newRole) => onRoleChange(member.id, newRole)}
                  disabled={member.status === 'Inactive'}
                />
              </TableCell>
              <TableCell>
                <StatusBadge status={member.status} />
              </TableCell>
              <TableCell>{member.lastLogin}</TableCell>
              <TableCell align="right">
                <Tooltip title={SETTINGS_LABELS.ACTIONS.DEACTIVATE}>
                  <span>
                    <ActionButton
                      onClick={() => onDeactivate(member)}
                      disabled={member.status === 'Inactive'}
                      size="small"
                    >
                      <PersonOff fontSize="small" />
                    </ActionButton>
                  </span>
                </Tooltip>
              </TableCell>
            </StyledTableRow>
          ))}
        </TableBody>
      </Table>
    </StyledTableContainer>
  )
}

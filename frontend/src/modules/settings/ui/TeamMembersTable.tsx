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

const TABLE_LABELS = {
  HEADERS: {
    NAME: 'Name',
    EMAIL: 'Email',
    ROLE: 'Role',
    STATUS: 'Status',
    LAST_LOGIN: 'Last Login',
    ACTIONS: 'Actions',
  },
  DEACTIVATE: 'Deactivate',
} as const

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
            <HeaderCell>{TABLE_LABELS.HEADERS.NAME}</HeaderCell>
            <HeaderCell>{TABLE_LABELS.HEADERS.EMAIL}</HeaderCell>
            <HeaderCell>{TABLE_LABELS.HEADERS.ROLE}</HeaderCell>
            <HeaderCell>{TABLE_LABELS.HEADERS.STATUS}</HeaderCell>
            <HeaderCell>{TABLE_LABELS.HEADERS.LAST_LOGIN}</HeaderCell>
            <HeaderCell align="right">
              {TABLE_LABELS.HEADERS.ACTIONS}
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
                <Tooltip title={TABLE_LABELS.DEACTIVATE}>
                  <span>
                    <ActionButton aria-label={TABLE_LABELS.DEACTIVATE}
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
    </StyledTableContainer >
  )
}

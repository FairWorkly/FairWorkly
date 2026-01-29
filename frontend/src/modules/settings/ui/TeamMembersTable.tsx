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
                    <ActionButton aria-label={SETTINGS_LABELS.ACTIONS.DEACTIVATE}
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

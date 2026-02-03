import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Tooltip,
  Box,
  Typography,
} from '@mui/material'
import { PersonOff, PersonAdd } from '@mui/icons-material'
import { styled } from '@/styles/styled'
import { StatusBadge } from './StatusBadge'
import { RoleDropdown } from './RoleDropdown'
import type { TeamMember, TeamMemberRole } from '../types'

interface Props {
  members: TeamMember[]
  onRoleChange: (id: string, role: TeamMemberRole) => void
  onDeactivate: (member: TeamMember) => void
  onActivate: (id: string) => void
}

const StyledPaper = styled(Paper)(({ theme }) => ({
  borderRadius: `${theme.fairworkly.radius.md}px`,
  border: `1px solid ${theme.palette.divider}`,
  overflow: 'hidden',
}))

const EmptyState = styled(Box)(({ theme }) => ({
  padding: theme.spacing(6),
  textAlign: 'center',
}))

export function TeamMembersTable({
  members,
  onRoleChange,
  onDeactivate,
  onActivate,
}: Props) {
  if (members.length === 0) {
    return (
      <Paper sx={{ borderRadius: 2 }}>
        <EmptyState>
          <Typography variant="h6" color="text.secondary" gutterBottom>
            No team members yet
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Invite your first team member to get started.
          </Typography>
        </EmptyState>
      </Paper>
    )
  }

  return (
    <TableContainer component={StyledPaper}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Email</TableCell>
            <TableCell>Role</TableCell>
            <TableCell>Status</TableCell>
            <TableCell>Last Login</TableCell>
            <TableCell align="right">Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {members.map((member) => (
            <TableRow key={member.id}>
              <TableCell>{member.name}</TableCell>
              <TableCell>{member.email}</TableCell>
              <TableCell>
                <RoleDropdown
                  value={member.role}
                  onChange={(role) => onRoleChange(member.id, role)}
                />
              </TableCell>
              <TableCell>
                <StatusBadge status={member.status} />
              </TableCell>
              <TableCell>{member.lastLogin}</TableCell>
              <TableCell align="right">
                {member.status === 'Active' ? (
                  <Tooltip title="Deactivate">
                    <IconButton
                      onClick={() => onDeactivate(member)}
                      color="error"
                      size="small"
                    >
                      <PersonOff />
                    </IconButton>
                  </Tooltip>
                ) : (
                  <Tooltip title="Activate">
                    <IconButton
                      onClick={() => onActivate(member.id)}
                      color="success"
                      size="small"
                    >
                      <PersonAdd />
                    </IconButton>
                  </Tooltip>
                )}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  )
}

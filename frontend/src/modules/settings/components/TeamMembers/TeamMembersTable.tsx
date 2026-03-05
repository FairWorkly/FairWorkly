import { useState } from 'react'
import { Button, Chip, CircularProgress, MenuItem, Stack, Table, TableBody, TableContainer, TableHead, Typography } from '@mui/material'
import type { SelectChangeEvent } from '@mui/material'
import { useAuth } from '@/modules/auth/hooks/useAuth'
import type { TeamMemberDto, UpdateTeamMemberRequest } from '../../types/teamMembers.types'
import { ROLE_OPTIONS } from '../../types/teamMembers.types'
import { DeactivateDialog } from './DeactivateDialog'
import { RoleChangeDialog } from './RoleChangeDialog'
import { CancelInviteDialog } from './CancelInviteDialog'
import {
  TeamCard,
  CardHeader,
  CardTitle,
  MemberCount,
  StyledTableRow,
  StyledTableCell,
  HeaderCell,
  RoleSelect,
  StatusSwitch,
  YouChip,
  LastLoginText,
  EmptyState,
} from './TeamMembers.styles'

interface RoleChangeTarget {
  member: TeamMemberDto
  newRole: 'Admin' | 'Manager'
}

interface TeamMembersTableProps {
  members: TeamMemberDto[]
  onUpdate: (userId: string, payload: UpdateTeamMemberRequest) => void
  updatingUserId: string | null
  onResendInvite: (userId: string) => void
  resendingUserId: string | null
  onCancelInvite: (userId: string) => void
  cancellingUserId: string | null
}

function isInvitationExpired(member: TeamMemberDto): boolean {
  if (member.invitationStatus !== 'Pending' || !member.invitationTokenExpiry) return false
  return new Date(member.invitationTokenExpiry) < new Date()
}

function formatLastLogin(lastLoginAt: string | null): string {
  if (!lastLoginAt) return 'Never'
  return new Date(lastLoginAt).toLocaleDateString('en-AU', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}

export function TeamMembersTable({ members, onUpdate, updatingUserId, onResendInvite, resendingUserId, onCancelInvite, cancellingUserId }: TeamMembersTableProps) {
  const { user: currentUser } = useAuth()
  const [deactivateTarget, setDeactivateTarget] = useState<TeamMemberDto | null>(null)
  const [roleChangeTarget, setRoleChangeTarget] = useState<RoleChangeTarget | null>(null)
  const [cancelTarget, setCancelTarget] = useState<TeamMemberDto | null>(null)

  const isRowUpdating = (member: TeamMemberDto) => updatingUserId === member.userId
  const isDeactivating = !!deactivateTarget && updatingUserId === deactivateTarget.userId
  const isChangingRole = !!roleChangeTarget && updatingUserId === roleChangeTarget.member.userId
  const isCancelling = !!cancelTarget && cancellingUserId === cancelTarget.userId


  const handleRoleChange = (member: TeamMemberDto, event: SelectChangeEvent<unknown>) => {
    const newRole = event.target.value as 'Admin' | 'Manager'
    if (newRole !== member.role) {
      setRoleChangeTarget({ member, newRole })
    }
  }

  const handleStatusToggle = (member: TeamMemberDto, checked: boolean) => {
    if (!checked) {
      setDeactivateTarget(member)
    } else {
      onUpdate(member.userId, { isActive: true })
    }
  }

  const handleConfirmDeactivate = () => {
    if (deactivateTarget) {
      onUpdate(deactivateTarget.userId, { isActive: false })
      setDeactivateTarget(null)
    }
  }

  const handleConfirmRoleChange = () => {
    if (roleChangeTarget) {
      onUpdate(roleChangeTarget.member.userId, { role: roleChangeTarget.newRole })
      setRoleChangeTarget(null)
    }
  }

  const handleConfirmCancel = () => {
    if (cancelTarget) {
      onCancelInvite(cancelTarget.userId)
      setCancelTarget(null)
    }
  }

  if (members.length === 0) {
    return (
      <TeamCard elevation={0}>
        <EmptyState>
          <Typography variant="body1">No team members found.</Typography>
        </EmptyState>
      </TeamCard>
    )
  }

  const isSelf = (member: TeamMemberDto) => member.userId === currentUser?.id

  return (
    <>
      <TeamCard elevation={0}>
        <CardHeader>
          <CardTitle>Team Members</CardTitle>
          <MemberCount>{members.length} member{members.length !== 1 ? 's' : ''}</MemberCount>
        </CardHeader>

        <TableContainer>
          <Table>
            <TableHead>
              <StyledTableRow>
                <HeaderCell>Name</HeaderCell>
                <HeaderCell>Email</HeaderCell>
                <HeaderCell>Role</HeaderCell>
                <HeaderCell>Status</HeaderCell>
                <HeaderCell>Last Login</HeaderCell>
                <HeaderCell>Actions</HeaderCell>
              </StyledTableRow>
            </TableHead>
            <TableBody>
              {members.map(member => (
                <StyledTableRow key={member.userId}>
                  <StyledTableCell>
                    <Typography variant="body2" fontWeight={500}>
                      {member.fullName}
                      {isSelf(member) && (
                        <YouChip label="You" size="small" color="primary" variant="outlined" />
                      )}
                      {member.invitationStatus === 'Pending' && (
                        isInvitationExpired(member)
                          ? <Chip label="Expired" size="small" color="error" sx={{ ml: 1, height: 20, fontSize: '0.7rem' }} />
                          : <Chip label="Pending" size="small" color="warning" sx={{ ml: 1, height: 20, fontSize: '0.7rem' }} />
                      )}
                    </Typography>
                  </StyledTableCell>

                  <StyledTableCell>
                    <Typography variant="body2" color="text.secondary">
                      {member.email}
                    </Typography>
                  </StyledTableCell>

                  <StyledTableCell>
                    {isSelf(member) ? (
                      <Typography variant="body2">{member.role}</Typography>
                    ) : (
                      <RoleSelect
                        size="small"
                        value={member.role}
                        onChange={e => handleRoleChange(member, e)}
                        disabled={isRowUpdating(member)}
                      >
                        {ROLE_OPTIONS.map(opt => (
                          <MenuItem key={opt.value} value={opt.value}>
                            {opt.label}
                          </MenuItem>
                        ))}
                      </RoleSelect>
                    )}
                  </StyledTableCell>

                  <StyledTableCell>
                    <StatusSwitch
                      checked={member.isActive}
                      onChange={(_, checked) => handleStatusToggle(member, checked)}
                      disabled={isRowUpdating(member) || isSelf(member)}
                      color={member.isActive ? 'success' : 'default'}
                    />
                  </StyledTableCell>

                  <StyledTableCell>
                    <LastLoginText>{formatLastLogin(member.lastLoginAt)}</LastLoginText>
                  </StyledTableCell>

                  <StyledTableCell>
                    {member.invitationStatus === 'Pending' && (
                      <Stack direction="row" spacing={1}>
                        <Button
                          size="small"
                          variant="outlined"
                          onClick={() => onResendInvite(member.userId)}
                          disabled={resendingUserId === member.userId}
                          startIcon={
                            resendingUserId === member.userId ? (
                              <CircularProgress size={14} />
                            ) : undefined
                          }
                        >
                          {resendingUserId === member.userId ? 'Resending...' : 'Resend'}
                        </Button>
                        <Button
                          size="small"
                          variant="outlined"
                          color="error"
                          onClick={() => setCancelTarget(member)}
                          disabled={cancellingUserId === member.userId}
                        >
                          Cancel
                        </Button>
                      </Stack>
                    )}
                  </StyledTableCell>
                </StyledTableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </TeamCard>

      <DeactivateDialog
        open={!!deactivateTarget}
        memberName={deactivateTarget?.fullName ?? ''}
        isUpdating={isDeactivating}
        onConfirm={handleConfirmDeactivate}
        onCancel={() => setDeactivateTarget(null)}
      />

      <RoleChangeDialog
        open={!!roleChangeTarget}
        memberName={roleChangeTarget?.member.fullName ?? ''}
        newRole={roleChangeTarget?.newRole ?? ''}
        isUpdating={isChangingRole}
        onConfirm={handleConfirmRoleChange}
        onCancel={() => setRoleChangeTarget(null)}
      />

      <CancelInviteDialog
        open={!!cancelTarget}
        memberName={cancelTarget?.fullName ?? ''}
        isCancelling={isCancelling}
        onConfirm={handleConfirmCancel}
        onCancel={() => setCancelTarget(null)}
      />
    </>
  )
}

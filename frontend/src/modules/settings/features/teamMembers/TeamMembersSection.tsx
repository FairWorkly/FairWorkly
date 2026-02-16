import { useState } from 'react'
import { Box, Button, Typography, Stack } from '@mui/material'
import { Add } from '@mui/icons-material'
import { styled } from '@/styles/styled'
import { useTeamMembers } from '@/modules/settings/hooks'
import {
  TeamMembersTable,
  InviteMemberModal,
  DeactivateDialog,
} from '../../ui'
import type { TeamMember } from '@/modules/settings/types'

const SectionHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(3),
}))

export function TeamMembersSection() {
  const [isInviteOpen, setIsInviteOpen] = useState(false)
  const [memberToDeactivate, setMemberToDeactivate] = useState<TeamMember | null>(null)

  const {
    members,
    updateRole,
    deactivateMember,
    activateMember,
    inviteMember,
  } = useTeamMembers()

  const handleDeactivateConfirm = () => {
    if (memberToDeactivate) {
      deactivateMember(memberToDeactivate.id)
      setMemberToDeactivate(null)
    }
  }

  return (
    <Box>
      <SectionHeader>
        <Stack>
          <Typography variant="h5" fontWeight={600}>
            Team Members
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Manage your organization team members
          </Typography>
        </Stack>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => setIsInviteOpen(true)}
        >
          Invite Member
        </Button>
      </SectionHeader>

      <TeamMembersTable
        members={members}
        onRoleChange={updateRole}
        onDeactivate={setMemberToDeactivate}
        onActivate={activateMember}
      />

      <InviteMemberModal
        open={isInviteOpen}
        onSubmit={inviteMember}
        onClose={() => setIsInviteOpen(false)}
      />

      <DeactivateDialog
        open={memberToDeactivate !== null}
        memberName={memberToDeactivate?.name ?? ''}
        onConfirm={handleDeactivateConfirm}
        onCancel={() => setMemberToDeactivate(null)}
      />
    </Box>
  )
}

import React from 'react'
import { Typography, Button, Box } from '@mui/material'
import { Add as AddIcon } from '@mui/icons-material'
import { styled } from '@/styles/styled'
import { SectionContent } from './Settings.styles'
import { useTeamMembers } from '../hooks/useTeamMembers'
import { TeamMembersTable } from './TeamMembersTable'
import { InviteModal } from './InviteModal'
import { DeactivateDialog } from './DeactivateDialog'

const TEAM_MEMBERS_SECTION_LABELS = {
  TITLE: 'Team Members',
  DESCRIPTION: 'Manage your team members and their access levels',
  INVITE_BUTTON: 'Invite Member',
  EMPTY_STATE: 'No team members yet. Invite your first team member.',
} as const

const SectionHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(3),
}))

const EmptyStateContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(8),
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.shape.borderRadius,
  border: `1px dashed ${theme.palette.divider}`,
  color: theme.palette.text.secondary,
}))

export const TeamMembersSection: React.FC = () => {
  const {
    members,
    inviteModalOpen,
    deactivateDialogOpen,
    memberToDeactivate,
    openInviteModal,
    closeInviteModal,
    closeDeactivateDialog,
    handleInvite,
    handleRoleChange,
    handleDeactivate,
    confirmDeactivate,
  } = useTeamMembers()

  return (
    <SectionContent>
      <SectionHeader>
        <Box>
          <Typography variant="h5">
            {TEAM_MEMBERS_SECTION_LABELS.TITLE}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {TEAM_MEMBERS_SECTION_LABELS.DESCRIPTION}
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={openInviteModal}
        >
          {TEAM_MEMBERS_SECTION_LABELS.INVITE_BUTTON}
        </Button>
      </SectionHeader>

      {members.length === 0 ? (
        <EmptyStateContainer>
          <Typography>{TEAM_MEMBERS_SECTION_LABELS.EMPTY_STATE}</Typography>
        </EmptyStateContainer>
      ) : (
        <TeamMembersTable
          members={members}
          onRoleChange={handleRoleChange}
          onDeactivate={handleDeactivate}
        />
      )}

      <InviteModal
        open={inviteModalOpen}
        onClose={closeInviteModal}
        onSubmit={handleInvite}
      />

      <DeactivateDialog
        open={deactivateDialogOpen}
        member={memberToDeactivate}
        onClose={closeDeactivateDialog}
        onConfirm={confirmDeactivate}
      />
    </SectionContent>
  )
}

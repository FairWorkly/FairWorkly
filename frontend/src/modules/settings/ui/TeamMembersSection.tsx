import React from 'react'
import { Typography, Button, Box } from '@mui/material'
import { Add as AddIcon } from '@mui/icons-material'
import { styled } from '@/styles/styled'
import { SectionContent } from './Settings.styles'
import { useTeamMembers } from '../hooks/useTeamMembers'
import { TeamMembersTable } from './TeamMembersTable'
import { InviteModal } from './InviteModal'
import { DeactivateDialog } from './DeactivateDialog'

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
            {SETTINGS_LABELS.TEAM_MEMBERS.TITLE}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {SETTINGS_LABELS.TEAM_MEMBERS.DESCRIPTION}
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={openInviteModal}
        >
          {SETTINGS_LABELS.TEAM_MEMBERS.INVITE_BUTTON}
        </Button>
      </SectionHeader>

      {members.length === 0 ? (
        <EmptyStateContainer>
          <Typography>{SETTINGS_LABELS.TEAM_MEMBERS.EMPTY_STATE}</Typography>
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

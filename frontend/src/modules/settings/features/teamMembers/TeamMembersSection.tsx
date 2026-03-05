import { useState } from 'react'
import { Alert, Box, Button, Snackbar } from '@mui/material'
import PersonAddIcon from '@mui/icons-material/PersonAdd'
import { TeamMembersTable } from '../../components/TeamMembers/TeamMembersTable'
import { InviteDialog } from '../../components/TeamMembers/InviteDialog'
import { useTeamMembers, useUpdateTeamMember, useInviteTeamMember, useResendInvitation, useCancelInvitation } from '../../hooks/useTeamMembers'
import { useNotification } from '@/shared/hooks'
import type { UpdateTeamMemberRequest, InviteTeamMemberRequest } from '../../types/teamMembers.types'
import { SectionWrapper, TableSkeleton } from './TeamMembersSection.styles'

export function TeamMembersSection() {
  const { data: members, isLoading, error: loadError } = useTeamMembers()
  const updateMutation = useUpdateTeamMember()
  const inviteMutation = useInviteTeamMember()
  const resendMutation = useResendInvitation()
  const cancelMutation = useCancelInvitation()
  const { notification, notify, clear } = useNotification()
  const [updatingUserId, setUpdatingUserId] = useState<string | null>(null)
  const [inviteDialogOpen, setInviteDialogOpen] = useState(false)
  const [inviteLink, setInviteLink] = useState<string | null>(null)
  const [inviteError, setInviteError] = useState<string | null>(null)
  const [resendingUserId, setResendingUserId] = useState<string | null>(null)
  const [cancellingUserIds, setCancellingUserIds] = useState<Set<string>>(new Set())

  if (isLoading) {
    return (
      <SectionWrapper>
        <TableSkeleton variant="rounded" />
      </SectionWrapper>
    )
  }

  if (loadError) {
    return (
      <SectionWrapper>
        <Alert severity="error">
          Failed to load team members. {loadError.message}
        </Alert>
      </SectionWrapper>
    )
  }

  if (!members) return null

  const handleUpdate = (userId: string, payload: UpdateTeamMemberRequest) => {
    setUpdatingUserId(userId)
    updateMutation.mutate(
      { userId, payload },
      {
        onSuccess: () => {
          notify('Team member updated successfully')
          setUpdatingUserId(null)
        },
        onError: (error) => {
          notify(error.message || 'Failed to update. Please try again.', 'error')
          setUpdatingUserId(null)
        },
      }
    )
  }

  const handleInviteSubmit = (data: InviteTeamMemberRequest) => {
    setInviteError(null)
    inviteMutation.mutate(data, {
      onSuccess: (response) => {
        setInviteLink(response.inviteLink)
        notify('Invitation sent successfully')
      },
      onError: (error) => {
        setInviteError(error.message || 'Failed to send invitation.')
      },
    })
  }

  const handleInviteDialogClose = () => {
    setInviteDialogOpen(false)
    setInviteLink(null)
    setInviteError(null)
  }

  const handleResendInvite = (userId: string) => {
    setResendingUserId(userId)
    resendMutation.mutate(userId, {
      onSuccess: () => {
        notify('Invitation resent successfully')
        setResendingUserId(null)
      },
      onError: (error) => {
        notify(error.message || 'Failed to resend invitation.', 'error')
        setResendingUserId(null)
      },
    })
  }

  const handleCancelInvite = (userId: string) => {
    if (cancellingUserIds.has(userId)) return
    setCancellingUserIds(prev => new Set(prev).add(userId))
    cancelMutation.mutate(userId, {
      onSuccess: () => {
        notify('Invitation cancelled successfully')
      },
      onError: (error) => {
        notify(error.message || 'Failed to cancel invitation.', 'error')
      },
      onSettled: () => {
        setCancellingUserIds(prev => { const s = new Set(prev); s.delete(userId); return s })
      },
    })
  }

  return (
    <SectionWrapper>
      <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
        <Button
          variant="contained"
          startIcon={<PersonAddIcon />}
          onClick={() => setInviteDialogOpen(true)}
        >
          Invite Member
        </Button>
      </Box>

      <TeamMembersTable
        members={members}
        onUpdate={handleUpdate}
        updatingUserId={updatingUserId}
        onResendInvite={handleResendInvite}
        resendingUserId={resendingUserId}
        onCancelInvite={handleCancelInvite}
        cancellingUserIds={cancellingUserIds}
      />

      <InviteDialog
        open={inviteDialogOpen}
        isSubmitting={inviteMutation.isPending}
        inviteLink={inviteLink}
        error={inviteError}
        onSubmit={handleInviteSubmit}
        onClose={handleInviteDialogClose}
      />

      <Snackbar open={!!notification} autoHideDuration={3000} onClose={clear}>
        {notification ? (
          <Alert onClose={clear} severity={notification.severity} variant="filled">
            {notification.message}
          </Alert>
        ) : undefined}
      </Snackbar>
    </SectionWrapper>
  )
}

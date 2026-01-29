import { useState } from 'react'
import type {
  TeamMember,
  TeamMemberRole,
  InviteTeamMemberFormData,
} from '../types/settings.types'
import { MOCK_TEAM_MEMBERS } from '../constants/settings.constants'

export const useTeamMembers = () => {
  const [members, setMembers] = useState<TeamMember[]>(MOCK_TEAM_MEMBERS)
  const [inviteModalOpen, setInviteModalOpen] = useState(false)
  const [deactivateDialogOpen, setDeactivateDialogOpen] = useState(false)
  const [memberToDeactivate, setMemberToDeactivate] =
    useState<TeamMember | null>(null)

  const handleInvite = (data: InviteTeamMemberFormData) => {
    const newMember: TeamMember = {
      id: Date.now().toString(),
      name: data.name,
      email: data.email,
      role: data.role,
      status: 'Active',
      lastLogin: '-', // Placeholder for new user
    }
    setMembers((prev) => [...prev, newMember])
  }

  const handleRoleChange = (memberId: string, newRole: TeamMemberRole) => {
    setMembers((prev) =>
      prev.map((m) => (m.id === memberId ? { ...m, role: newRole } : m))
    )
  }

  const handleDeactivate = (member: TeamMember) => {
    setMemberToDeactivate(member)
    setDeactivateDialogOpen(true)
  }

  const confirmDeactivate = () => {
    if (memberToDeactivate) {
      setMembers((prev) =>
        prev.map((m) =>
          m.id === memberToDeactivate.id ? { ...m, status: 'Inactive' } : m
        )
      )
      closeDeactivateDialog()
    }
  }

  const openInviteModal = () => setInviteModalOpen(true)
  const closeInviteModal = () => setInviteModalOpen(false)

  const closeDeactivateDialog = () => {
    setDeactivateDialogOpen(false)
    setMemberToDeactivate(null)
  }

  return {
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
  }
}

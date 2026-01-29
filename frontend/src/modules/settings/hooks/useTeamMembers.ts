import { useState } from 'react'
import type {
  TeamMember,
  TeamMemberRole,
  InviteTeamMemberFormData,
} from '../types/settings.types'

const MOCK_TEAM_MEMBERS: TeamMember[] = [
  {
    id: '1',
    name: 'Alice Chen',
    email: 'alice@demo.com',
    role: 'Admin',
    status: 'Active',
    lastLogin: '2024-02-01 09:12',
  },
  {
    id: '2',
    name: 'Ben Lee',
    email: 'ben@demo.com',
    role: 'Manager',
    status: 'Inactive',
    lastLogin: '2024-01-28 17:40',
  },
]

export const useTeamMembers = () => {
  const [members, setMembers] = useState<TeamMember[]>(MOCK_TEAM_MEMBERS)
  const [inviteModalOpen, setInviteModalOpen] = useState(false)
  const [deactivateDialogOpen, setDeactivateDialogOpen] = useState(false)
  const [memberToDeactivate, setMemberToDeactivate] =
    useState<TeamMember | null>(null)

    const handleInvite = (data: InviteTeamMemberFormData) => {
      // Check for duplicate email
      const emailExists = members.some(
        (m) => m.email.toLowerCase() === data.email.toLowerCase()
      )
      if (emailExists) {
        // Consider returning an error or throwing
        console.warn('Email already exists')
        return
      }
      const newMember: TeamMember = {
        id: crypto.randomUUID(),
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

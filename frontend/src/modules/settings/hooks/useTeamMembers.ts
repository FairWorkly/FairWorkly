import { useState, useCallback } from 'react'
import type { TeamMember, TeamMemberRole, InviteMemberFormData } from '../types'

// Mock data - local to this hook
const initialMockData: TeamMember[] = [
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

export function useTeamMembers() {
  const [members, setMembers] = useState<TeamMember[]>(initialMockData)

  const updateRole = useCallback((id: string, role: TeamMemberRole) => {
    setMembers((prev) =>
      prev.map((m) => (m.id === id ? { ...m, role } : m))
    )
  }, [])

  const deactivateMember = useCallback((id: string) => {
    setMembers((prev) =>
      prev.map((m) => (m.id === id ? { ...m, status: 'Inactive' as const } : m))
    )
  }, [])

  const activateMember = useCallback((id: string) => {
    setMembers((prev) =>
      prev.map((m) => (m.id === id ? { ...m, status: 'Active' as const } : m))
    )
  }, [])

  const inviteMember = useCallback((data: InviteMemberFormData) => {
    const newMember: TeamMember = {
      id: String(Date.now()),
      name: data.name,
      email: data.email,
      role: data.role,
      status: 'Active',
      lastLogin: 'Never',
    }
    setMembers((prev) => [...prev, newMember])
  }, [])

  return {
    members,
    updateRole,
    deactivateMember,
    activateMember,
    inviteMember,
  }
}

export interface TeamMember {
  id: string
  name: string
  email: string
  role: TeamMemberRole
  status: TeamMemberStatus
  lastLogin: string
}

export type TeamMemberRole = 'Admin' | 'Manager'
export type TeamMemberStatus = 'Active' | 'Inactive'

export interface InviteTeamMemberFormData {
  name: string
  email: string
  role: TeamMemberRole
}

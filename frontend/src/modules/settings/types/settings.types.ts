// Team member data structure (matches mock data)
export interface TeamMember {
  id: string
  name: string
  email: string
  role: TeamMemberRole
  status: TeamMemberStatus
  lastLogin: string
}

// Role options
export type TeamMemberRole = 'Admin' | 'Manager'

// Status options
export type TeamMemberStatus = 'Active' | 'Inactive'

// Form data for invite modal
export interface InviteMemberFormData {
  name: string
  email: string
  role: TeamMemberRole
}

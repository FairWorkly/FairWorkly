import type { TeamMember, TeamMemberRole } from '../types/settings.types'

export const SETTINGS_LABELS = {
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

export const TEAM_MEMBER_ROLES: TeamMemberRole[] = ['Admin', 'Manager']

export const MOCK_TEAM_MEMBERS: TeamMember[] = [
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

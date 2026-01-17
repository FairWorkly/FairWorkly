import { useAuth } from '@/modules/auth'
import type { FairBotPermission } from '../types/fairbot.types'

interface UsePermissionsResult {
  hasPermission: (permission: FairBotPermission | null) => boolean
}

/**
 * Role-based permission mapping for FairBot features
 * - admin: Full access to all compliance tools
 * - manager: Limited to roster compliance checks
 */
const ROLE_PERMISSIONS: Record<string, FairBotPermission[]> = {
  admin: ['CheckPayrollCompliance', 'CheckRosterCompliance', 'ManageDocuments'],
  manager: ['CheckRosterCompliance'],
}

export const usePermissions = (): UsePermissionsResult => {
  const { user } = useAuth()

  const hasPermission = (permission: FairBotPermission | null): boolean => {
    // Actions without permission requirements are available to all users
    if (!permission) {
      return true
    }

    // No user or role means no permissions
    if (!user?.role) {
      return false
    }

    // Check if user's role includes the required permission
    const userPermissions = ROLE_PERMISSIONS[user.role] ?? []
    return userPermissions.includes(permission)
  }

  return { hasPermission }
}

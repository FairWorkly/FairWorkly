import type { FairBotPermission } from '../types/fairbot.types'

interface UsePermissionsResult {
  hasPermission: (permission: FairBotPermission | null) => boolean
}

export const usePermissions = (): UsePermissionsResult => {
  const hasPermission = (permission: FairBotPermission | null) => {
    if (!permission) {
      return true
    }

    return true
  }

  return { hasPermission }
}

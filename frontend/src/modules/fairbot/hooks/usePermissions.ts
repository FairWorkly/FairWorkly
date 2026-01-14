import type { FairBotPermission } from '../types/fairbot.types'

interface UsePermissionsResult {
  hasPermission: (permission: FairBotPermission | null) => boolean
}

export const usePermissions = (): UsePermissionsResult => {
  const hasPermission = (permission: FairBotPermission | null) => {
    // Placeholder: wire to real permission checks when auth is available.
    if (!permission) {
      return true
    }

    return true
  }

  return { hasPermission }
}

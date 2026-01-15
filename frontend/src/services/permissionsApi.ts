import { get } from './baseApi'
import type { PermissionsResponse } from '@/shared/types/permissions.types'

/**
 * Permissions API client
 *
 * GET /api/auth/permissions
 *
 * Returns the authenticated user's role, permissions, and module access.
 * Used by PermissionProvider to populate permission context.
 *
 * @returns PermissionsResponse containing role, permissions[], and modules[]
 * @throws ApiError with status 401 if not authenticated
 */
export function getPermissions(): Promise<PermissionsResponse> {
  return get<PermissionsResponse>('/auth/permissions')
}

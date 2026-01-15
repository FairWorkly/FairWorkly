import type { ReactNode } from 'react'
import { usePermissions } from '@/shared/hooks/usePermissions'
import type { Permission } from '@/shared/types/permissions.types'

interface PermissionGateProps {
  /** The permission required to render children */
  permission: Permission
  /** Content to render if user lacks permission (default: null) */
  fallback?: ReactNode
  /** Content to render if user has permission */
  children: ReactNode
}

/**
 * Conditionally renders children based on user permissions.
 *
 * Use this to hide UI elements that require specific permissions.
 * For route-level protection, use RoleBasedRoute instead.
 *
 * @example
 * ```tsx
 * <PermissionGate permission={Permission.CheckPayrollCompliance}>
 *   <PayrollCheckButton />
 * </PermissionGate>
 *
 * // With fallback
 * <PermissionGate
 *   permission={Permission.ManageDocuments}
 *   fallback={<DisabledButton />}
 * >
 *   <EditDocumentButton />
 * </PermissionGate>
 * ```
 */
export function PermissionGate({
  permission,
  fallback = null,
  children,
}: PermissionGateProps) {
  const { hasPermission, isLoading } = usePermissions()

  // Don't render anything while loading to prevent flash of content
  if (isLoading) {
    return null
  }

  return hasPermission(permission) ? <>{children}</> : <>{fallback}</>
}

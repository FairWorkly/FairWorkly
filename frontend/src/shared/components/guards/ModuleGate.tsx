import type { ReactNode } from 'react'
import { usePermissions } from '@/shared/hooks/usePermissions'
import type { ModuleId } from '@/shared/types/permissions.types'

interface ModuleGateProps {
  /** The module ID required to render children */
  moduleId: ModuleId
  /** Content to render if user cannot access module (default: null) */
  fallback?: ReactNode
  /** Content to render if user can access module */
  children: ReactNode
}

/**
 * Conditionally renders children based on module access.
 *
 * Use this to hide entire sections or features that belong to a specific module.
 * For fine-grained permission checks, use PermissionGate instead.
 *
 * @example
 * ```tsx
 * <ModuleGate moduleId="payroll">
 *   <PayrollSection />
 * </ModuleGate>
 *
 * // With fallback
 * <ModuleGate
 *   moduleId="documents"
 *   fallback={<UpgradePrompt />}
 * >
 *   <DocumentManager />
 * </ModuleGate>
 * ```
 */
export function ModuleGate({
  moduleId,
  fallback = null,
  children,
}: ModuleGateProps) {
  const { canAccessModule, isLoading } = usePermissions()

  // Don't render anything while loading to prevent flash of content
  if (isLoading) {
    return null
  }

  return canAccessModule(moduleId) ? <>{children}</> : <>{fallback}</>
}

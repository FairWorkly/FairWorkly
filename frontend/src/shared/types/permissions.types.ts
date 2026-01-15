/**
 * Shared permission types and constants for role-based access control.
 *
 * These types are used across the application for:
 * - Route guards (RoleBasedRoute)
 * - UI gating (PermissionGate, ModuleGate)
 * - Navigation filtering
 * - Feature access checks
 */

// -----------------------------------------------------------------------------
// Roles
// -----------------------------------------------------------------------------

/**
 * User roles mapped from backend.
 * - admin: Full access (maps to backend Owner)
 * - manager: Limited to roster compliance (maps to backend Manager)
 * - staff: Read-only own profile/docs (maps to backend Staff)
 */
export type Role = 'admin' | 'manager' | 'staff'

/**
 * Array of all valid roles for iteration and validation.
 */
export const ROLES = ['admin', 'manager', 'staff'] as const

// -----------------------------------------------------------------------------
// Permissions
// -----------------------------------------------------------------------------

/**
 * Permission tokens matching backend Permission enum.
 * Used for fine-grained access control beyond role checks.
 */
export const Permission = {
  // Payroll permissions
  ViewAllPayroll: 'ViewAllPayroll',
  CheckPayrollCompliance: 'CheckPayrollCompliance',
  ViewOwnPayslip: 'ViewOwnPayslip',

  // Roster permissions
  ManageRoster: 'ManageRoster',
  CheckRosterCompliance: 'CheckRosterCompliance',
  ViewOwnRoster: 'ViewOwnRoster',

  // Employee permissions
  ManageAllEmployees: 'ManageAllEmployees',
  ViewAllEmployees: 'ViewAllEmployees',
  ViewOwnProfile: 'ViewOwnProfile',
  UpdateOwnProfile: 'UpdateOwnProfile',

  // Document permissions
  ManageDocuments: 'ManageDocuments',
  ViewAllDocuments: 'ViewAllDocuments',
  ViewOwnDocuments: 'ViewOwnDocuments',
} as const

export type Permission = (typeof Permission)[keyof typeof Permission]

/**
 * Array of all permission values for iteration and validation.
 */
export const PERMISSIONS = Object.values(Permission)

// -----------------------------------------------------------------------------
// Modules
// -----------------------------------------------------------------------------

/**
 * Module identifiers for coarse-grained access control.
 * Used by canAccessModule() to show/hide entire sections.
 */
export type ModuleId =
  | 'payroll'
  | 'roster'
  | 'documents'
  | 'settings'
  | 'fairbot'
  | 'employees'

/**
 * Array of all module IDs for iteration and validation.
 */
export const MODULE_IDS: ModuleId[] = [
  'payroll',
  'roster',
  'documents',
  'settings',
  'fairbot',
  'employees',
]

// -----------------------------------------------------------------------------
// API Response DTOs
// -----------------------------------------------------------------------------

/**
 * Module access information from backend.
 */
export interface ModuleAccess {
  module: ModuleId
  canAccess: boolean
  features: string[]
}

/**
 * Response shape from GET /api/auth/permissions.
 */
export interface PermissionsResponse {
  role: Role
  permissions: Permission[]
  modules: ModuleAccess[]
}

// -----------------------------------------------------------------------------
// Role-Permission Mapping (for mock/fallback)
// -----------------------------------------------------------------------------

/**
 * Default role-to-permission mapping used when backend is unavailable.
 * This mirrors the backend's permission assignment logic.
 */
export const DEFAULT_ROLE_PERMISSIONS: Record<Role, Permission[]> = {
  admin: [
    Permission.ViewAllPayroll,
    Permission.CheckPayrollCompliance,
    Permission.ViewOwnPayslip,
    Permission.ManageRoster,
    Permission.CheckRosterCompliance,
    Permission.ViewOwnRoster,
    Permission.ManageAllEmployees,
    Permission.ViewAllEmployees,
    Permission.ViewOwnProfile,
    Permission.UpdateOwnProfile,
    Permission.ManageDocuments,
    Permission.ViewAllDocuments,
    Permission.ViewOwnDocuments,
  ],
  manager: [
    Permission.CheckRosterCompliance,
    Permission.ViewOwnRoster,
    Permission.ViewAllEmployees,
    Permission.ViewOwnProfile,
    Permission.UpdateOwnProfile,
    Permission.ViewOwnDocuments,
  ],
  staff: [
    Permission.ViewOwnPayslip,
    Permission.ViewOwnRoster,
    Permission.ViewOwnProfile,
    Permission.UpdateOwnProfile,
    Permission.ViewOwnDocuments,
  ],
}

/**
 * Default role-to-module mapping used when backend is unavailable.
 */
export const DEFAULT_ROLE_MODULES: Record<Role, ModuleId[]> = {
  admin: ['payroll', 'roster', 'documents', 'settings', 'fairbot', 'employees'],
  manager: ['roster', 'fairbot', 'employees'],
  staff: ['fairbot'],
}

// -----------------------------------------------------------------------------
// Helper Types
// -----------------------------------------------------------------------------

/**
 * Props for permission-gated components.
 */
export interface PermissionGateProps {
  permission: Permission
  fallback?: React.ReactNode
  children: React.ReactNode
}

/**
 * Props for module-gated components.
 */
export interface ModuleGateProps {
  moduleId: ModuleId
  fallback?: React.ReactNode
  children: React.ReactNode
}

/**
 * Context value shape for PermissionProvider.
 */
export interface PermissionContextValue {
  role: Role | null
  permissions: Permission[]
  modules: ModuleAccess[]
  hasPermission: (permission: Permission) => boolean
  canAccessModule: (moduleId: ModuleId) => boolean
  isLoading: boolean
  error: Error | null
}

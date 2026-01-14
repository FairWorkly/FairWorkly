// Shape for nav items used by the sidebar/topbar components.
export interface NavigationItem {
  // Text displayed in the UI.
  label: string
  // Route path the item links to.
  to: string
}

// Branding used in navigation UI.
export const APP_LABELS = {
  // Product name shown in header/brand areas.
  BRAND: 'FairWorkly',
} as const

// ARIA labels for accessible navigation regions.
export const NAV_ARIA = {
  // Label for the main sidebar nav.
  PRIMARY: 'Primary navigation',
  // Label for the top navigation bar.
  TOP: 'Top navigation',
} as const

// Layout tokens for navigation spacing and sizing.
export const NAV_LAYOUT = {
  // Vertical gap between topbar items.
  TOP_GAP_PX: 8,
  // Flex value for brand/title spacing.
  TITLE_FLEX: 1,
} as const

// Centralized route paths to avoid hardcoded strings.
export const NAV_ROUTES = {
  // Landing page route.
  HOME: '/',
  // Compliance module route.
  COMPLIANCE: '/compliance',
  // Payroll module route.
  PAYROLL: '/payroll',
  // Documents module route.
  DOCUMENTS: '/documents',
  // Employee module route.
  EMPLOYEE: '/employee',
  // FairBot chat route.
  FAIRBOT: '/fairbot',
  // Compliance Q&A route.
  COMPLIANCE_QA: '/compliance/qa',
  // Employee help route.
  EMPLOYEE_HELP: '/employee/help',
} as const

// Human-friendly labels for navigation items.
export const NAV_LABELS = {
  // Label for home route.
  HOME: 'Home',
  // Label for compliance route.
  COMPLIANCE: 'Compliance',
  // Label for payroll route.
  PAYROLL: 'Payroll',
  // Label for documents route.
  DOCUMENTS: 'Documents',
  // Label for employee route.
  EMPLOYEES: 'Employees',
  // Label for FairBot route.
  FAIRBOT: 'FairBot',
  // Label for compliance Q&A route.
  COMPLIANCE_QA: 'Compliance Q&A',
  // Label for employee help route.
  EMPLOYEE_HELP: 'Employee Help',
} as const

// Sidebar navigation ordering shown in the main layout.
export const SIDEBAR_NAV_ITEMS: NavigationItem[] = [
  { label: NAV_LABELS.HOME, to: NAV_ROUTES.HOME },
  { label: NAV_LABELS.COMPLIANCE, to: NAV_ROUTES.COMPLIANCE },
  { label: NAV_LABELS.PAYROLL, to: NAV_ROUTES.PAYROLL },
  { label: NAV_LABELS.DOCUMENTS, to: NAV_ROUTES.DOCUMENTS },
  { label: NAV_LABELS.FAIRBOT, to: NAV_ROUTES.FAIRBOT },
  { label: NAV_LABELS.EMPLOYEES, to: NAV_ROUTES.EMPLOYEE },
]

// Topbar shortcuts for frequently used destinations.
export const TOPBAR_NAV_ITEMS: NavigationItem[] = [
  { label: NAV_LABELS.COMPLIANCE_QA, to: NAV_ROUTES.COMPLIANCE_QA },
  { label: NAV_LABELS.FAIRBOT, to: NAV_ROUTES.FAIRBOT },
  { label: NAV_LABELS.PAYROLL, to: NAV_ROUTES.PAYROLL },
  { label: NAV_LABELS.DOCUMENTS, to: NAV_ROUTES.DOCUMENTS },
  { label: NAV_LABELS.EMPLOYEE_HELP, to: NAV_ROUTES.EMPLOYEE_HELP },
]

export interface NavigationItem {
  label: string
  to: string
}

export const APP_LABELS = {
  BRAND: 'FairWorkly',
} as const

export const NAV_ARIA = {
  PRIMARY: 'Primary navigation',
  TOP: 'Top navigation',
} as const

export const NAV_LAYOUT = {
  TOP_GAP_PX: 8,
  TITLE_FLEX: 1,
} as const

export const NAV_ROUTES = {
  HOME: '/',
  COMPLIANCE: '/compliance',
  PAYROLL: '/payroll',
  DOCUMENTS: '/documents',
  EMPLOYEE: '/employee',
  FAIRBOT: '/fairbot',
  COMPLIANCE_QA: '/compliance/qa',
  EMPLOYEE_HELP: '/employee/help',
} as const

export const NAV_LABELS = {
  HOME: 'Home',
  COMPLIANCE: 'Compliance',
  PAYROLL: 'Payroll',
  DOCUMENTS: 'Documents',
  EMPLOYEES: 'Employees',
  FAIRBOT: 'FairBot',
  COMPLIANCE_QA: 'Compliance Q&A',
  EMPLOYEE_HELP: 'Employee Help',
} as const

export const SIDEBAR_NAV_ITEMS: NavigationItem[] = [
  { label: NAV_LABELS.HOME, to: NAV_ROUTES.HOME },
  { label: NAV_LABELS.COMPLIANCE, to: NAV_ROUTES.COMPLIANCE },
  { label: NAV_LABELS.PAYROLL, to: NAV_ROUTES.PAYROLL },
  { label: NAV_LABELS.DOCUMENTS, to: NAV_ROUTES.DOCUMENTS },
  { label: NAV_LABELS.FAIRBOT, to: NAV_ROUTES.FAIRBOT },
  { label: NAV_LABELS.EMPLOYEES, to: NAV_ROUTES.EMPLOYEE },
]

export const TOPBAR_NAV_ITEMS: NavigationItem[] = [
  { label: NAV_LABELS.COMPLIANCE_QA, to: NAV_ROUTES.COMPLIANCE_QA },
  { label: NAV_LABELS.FAIRBOT, to: NAV_ROUTES.FAIRBOT },
  { label: NAV_LABELS.PAYROLL, to: NAV_ROUTES.PAYROLL },
  { label: NAV_LABELS.DOCUMENTS, to: NAV_ROUTES.DOCUMENTS },
  { label: NAV_LABELS.EMPLOYEE_HELP, to: NAV_ROUTES.EMPLOYEE_HELP },
]

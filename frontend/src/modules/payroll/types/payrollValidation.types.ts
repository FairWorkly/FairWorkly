// --- Auxiliary types ---

export type CategoryType =
  | 'BaseRate'
  | 'PenaltyRate'
  | 'Superannuation'
  | 'CasualLoading'

/** 1 = Info, 2 = Warning, 3 = Error, 4 = Critical (higher = more severe) */
export type Severity = 1 | 2 | 3 | 4

export type ValidationStatus = 'Passed' | 'Failed'

export type UnitType = 'Hour' | 'Currency'

// --- Request ---

export interface PayrollValidationRequest {
  awardType: string
  state: string
  enableBaseRateCheck: boolean
  enablePenaltyCheck: boolean
  enableCasualLoadingCheck: boolean
  enableSuperCheck: boolean
}

// --- Success response (200) ---

export interface IssueDescription {
  actualValue: number
  expectedValue: number
  affectedUnits: number
  unitType: UnitType
  contextLabel: string
}

export interface ValidationIssue {
  issueId: string
  categoryType: CategoryType
  employeeName: string
  employeeId: string
  severity: Severity
  impactAmount: number
  description: IssueDescription | null
  warning: string | null
}

export interface ValidationCategory {
  key: CategoryType
  affectedEmployeeCount: number
  totalUnderpayment: number
}

export interface ValidationSummary {
  passedCount: number
  totalIssues: number
  totalUnderpayment: number
  affectedEmployees: number
}

export interface PayrollValidationResult {
  validationId: string
  timestamp: string
  status: ValidationStatus
  payPeriodStart: string
  payPeriodEnd: string
  summary: ValidationSummary
  categories: ValidationCategory[]
  issues: ValidationIssue[]
}

// --- Error responses ---

export interface PayrollRequestValidationError {
  code: number
  msg: string
  data: {
    errors: Array<{ field: string; message: string }>
  }
}

export interface PayrollCsvError {
  code: number
  msg: string
  data: {
    errors: Array<{ rowNumber: number; field: string; message: string }>
  }
}

export interface PayrollServerError {
  status: number
  title: string
  detail?: string
  instance?: string
}

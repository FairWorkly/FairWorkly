export type ValidationStatus = 'idle' | 'processing' | 'completed'

export interface UploadedFile {
  id: number
  name: string
  size: string
  date: string
  status: 'ready' | 'validating' | 'validated'
}

export interface ValidationMetadata {
  award: string
  payPeriod: string
  weekStarting: string
  weekEnding: string
  validatedAt?: string
  validationId?: string
}

export interface IssueItem {
  id: number
  name: string
  empId: string
  actualValue: string
  expectedValue: string
  reason: string
  variance: string
  breakdown: string
}

export interface IssueCategory {
  id: string
  title: string
  icon: string
  color: string
  employeeCount: number
  totalUnderpayment: string
  issues: IssueItem[]
}

export interface ValidationSummary {
  employeesCompliant: number
  totalIssues: number
  criticalIssuesCount: number
  totalUnderpayment: string
  employeesAffected: number
}

export interface ComplianceConfig {
  title: string
  subtitle?: string
  fileTypes: string[]
  maxFileSize: string
  coverageAreas: string[]
}

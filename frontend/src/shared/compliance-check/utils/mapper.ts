import type {
  ValidationMetadata,
  ValidationSummary,
  IssueCategory,
  IssueItem,
} from '../types/complianceCheck.type'
import type { ComplianceApiResponse } from '../types/complianceApiResponse.type'

// TODO: Align these fallbacks with backend defaults once DTO is finalized.
const DEFAULT_TEXT = 'Pending'
const DEFAULT_VALUE = 'N/A'
const DEFAULT_ICON = 'report_problem'
const DEFAULT_COLOR = '#ef4444'

export const mapBackendToComplianceResults = (
  // TODO: Align payload shape with backend DTO before wiring to API.
  payload: ComplianceApiResponse
): {
  metadata: ValidationMetadata
  summary: ValidationSummary
  categories: IssueCategory[]
} => {
  const metadata: ValidationMetadata = {
    award: payload.metadata?.award ?? DEFAULT_TEXT,
    payPeriod: DEFAULT_TEXT,
    weekStarting: payload.metadata?.pay_period?.start ?? '',
    weekEnding: payload.metadata?.pay_period?.end ?? '',
    validatedAt: payload.metadata?.validated_at,
    validationId: payload.metadata?.validation_id,
  }

  const summary: ValidationSummary = {
    employeesCompliant: payload.summary?.employees_compliant ?? 0,
    totalIssues: payload.summary?.total_issues ?? 0,
    criticalIssuesCount: payload.summary?.critical_issues_count ?? 0,
    totalUnderpayment:
      payload.summary?.total_underpayment ??
      payload.summary?.total_variance ??
      DEFAULT_VALUE,
    employeesAffected: payload.summary?.employees_affected ?? 0,
  }

  const categories: IssueCategory[] = (payload.categories ?? []).map(
    category => {
      const issues: IssueItem[] = (category.issues ?? []).map(issue => ({
        id: issue.id ?? 0,
        name: issue.name ?? DEFAULT_TEXT,
        empId: issue.emp_id ?? DEFAULT_TEXT,
        actualValue: issue.actual_value ?? DEFAULT_VALUE,
        expectedValue: issue.expected_value ?? DEFAULT_VALUE,
        reason: issue.reason ?? '',
        variance: issue.variance ?? DEFAULT_VALUE,
        breakdown: issue.breakdown ?? '',
      }))

      return {
        id: category.id ?? 'unknown',
        title: category.title ?? DEFAULT_TEXT,
        icon: category.icon ?? DEFAULT_ICON,
        color: category.color ?? DEFAULT_COLOR,
        employeeCount: category.employee_count ?? issues.length,
        totalUnderpayment:
          category.total_underpayment ?? category.total_variance ?? DEFAULT_VALUE,
        issues,
      }
    }
  )

  return { metadata, summary, categories }
}

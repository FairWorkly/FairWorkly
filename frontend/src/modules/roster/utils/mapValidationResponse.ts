import type {
  ValidateRosterResponse,
  RosterIssueSummary,
} from '@/services/rosterApi'
import type {
  ValidationMetadata,
  ValidationSummary,
  IssueCategory,
} from '@/shared/compliance-check'
import { checkTypeDisplayMap } from '../config/checkTypeDisplay'

export interface RosterComplianceResults {
  metadata: ValidationMetadata
  summary: ValidationSummary
  categories: IssueCategory[]
}

/**
 * Transforms the backend ValidateRosterResponse (flat issues list)
 * directly into the component props shape (camelCase) used by ComplianceResults.
 */
export function mapValidationToComplianceResults(
  response: ValidateRosterResponse
): RosterComplianceResults {
  // Filter out DataQuality issues — these are data problems, not compliance violations.
  // They belong in the upload stage, not the compliance results page.
  const complianceIssues = response.issues.filter(
    i => i.checkType !== 'DataQuality'
  )

  // Group issues by checkType
  const issuesByCheckType = new Map<string, RosterIssueSummary[]>()
  for (const issue of complianceIssues) {
    const existing = issuesByCheckType.get(issue.checkType) ?? []
    existing.push(issue)
    issuesByCheckType.set(issue.checkType, existing)
  }

  // Keep IssueItem.id globally unique across all categories by mapping
  // backend issue IDs to a stable numeric ID for the current payload.
  const issueIdMap = new Map<string, number>()
  let nextIssueId = 1
  for (const issue of complianceIssues) {
    if (!issueIdMap.has(issue.id)) {
      issueIdMap.set(issue.id, nextIssueId)
      nextIssueId += 1
    }
  }

  // Build categories from grouped issues
  const categories: IssueCategory[] = Array.from(
    issuesByCheckType.entries()
  ).map(([checkType, issues]) => {
    const display = checkTypeDisplayMap[checkType] ?? {
      id: checkType.toLowerCase(),
      title: checkType,
      icon: 'report_problem',
      color: '#ef4444',
    }

    const uniqueEmployees = new Set(issues.map(i => i.employeeId))

    return {
      id: display.id,
      title: display.title,
      icon: display.icon,
      color: display.color,
      employeeCount: uniqueEmployees.size,
      totalUnderpayment: `${issues.length} violation${issues.length !== 1 ? 's' : ''}`,
      issues: issues.map(issue => ({
        id: issueIdMap.get(issue.id)!,
        name: issue.employeeName ?? 'Unknown',
        empId: issue.employeeNumber ?? issue.employeeId.substring(0, 8),
        actualValue: formatIssueValue(issue.actualValue, issue.checkType),
        expectedValue: formatIssueValue(issue.expectedValue, issue.checkType),
        reason: issue.description,
        variance: computeVariance(issue),
        breakdown: issue.affectedDates
          ? `Affected date: ${issue.affectedDates}`
          : '',
      })),
    }
  })

  // Calculate stats from filtered compliance issues only
  const affectedEmployeeIds = new Set(complianceIssues.map(i => i.employeeId))
  const compliantEmployees = Math.max(
    0,
    response.totalEmployees - affectedEmployeeIds.size
  )
  const metadata: ValidationMetadata = {
    award: 'Australian Workplace Award',
    payPeriod: 'Pending',
    weekStarting: response.weekStartDate,
    weekEnding: response.weekEndDate,
    validatedAt: response.validatedAt
      ? new Date(response.validatedAt).toLocaleDateString('en-AU', {
          year: 'numeric',
          month: 'short',
          day: 'numeric',
        })
      : undefined,
    validationId: response.validationId.substring(0, 8).toUpperCase(),
  }

  const summary: ValidationSummary = {
    employeesCompliant: compliantEmployees,
    totalIssues: complianceIssues.length,
    // Use backend-computed failing issues count to stay aligned with validation truth.
    criticalIssuesCount: response.criticalIssues,
    totalUnderpayment: 'N/A',
    employeesAffected: affectedEmployeeIds.size,
  }

  return { metadata, summary, categories }
}

function formatIssueValue(value: number | null, checkType: string): string {
  if (value === null || value === undefined) return 'N/A'

  switch (checkType) {
    case 'MinimumShiftHours':
    case 'WeeklyHoursLimit':
      return `${value} hrs`
    case 'MealBreak':
      return value > 0 ? `${value} min break` : 'No break'
    case 'RestPeriodBetweenShifts':
      return `${value} hrs gap`
    case 'MaximumConsecutiveDays':
      return `${value} days`
    default:
      return `${value}`
  }
}

function computeVariance(issue: RosterIssueSummary): string {
  if (issue.expectedValue == null || issue.actualValue == null) return 'N/A'
  const diff = Math.abs(issue.actualValue - issue.expectedValue)

  switch (issue.checkType) {
    case 'MinimumShiftHours':
    case 'WeeklyHoursLimit':
    case 'RestPeriodBetweenShifts':
      return issue.actualValue < issue.expectedValue
        ? `${diff} hr${diff !== 1 ? 's' : ''} short`
        : `${diff} hr${diff !== 1 ? 's' : ''} over`
    case 'MealBreak':
      return `${diff} min${diff !== 1 ? 's' : ''} short`
    case 'MaximumConsecutiveDays':
      return `${diff} day${diff !== 1 ? 's' : ''} over`
    default:
      return `${diff}`
  }
}

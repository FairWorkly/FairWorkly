// Payroll CSV export. Generates a downloadable CSV file from
// PayrollValidationResult. Each row represents one issue with:
// Employee Name, Employee ID, Category, Severity, Impact Amount, Description.
//
// Uses the same description template logic as PayrollIssueRow, but
// outputs plain text (single line) instead of JSX.

import type {
  PayrollValidationResult,
  ValidationIssue,
} from '../types/payrollValidation.types'
import { categoryConfig, severityConfig } from './payrollCategoryConfig'

function fmt(n: number): string {
  return n.toFixed(2)
}

function escapeCsvField(value: string): string {
  if (value.includes(',') || value.includes('"') || value.includes('\n')) {
    return `"${value.replace(/"/g, '""')}"`
  }
  return value
}

function buildIssueDescription(issue: ValidationIssue): string {
  if (issue.warning) return issue.warning

  const d = issue.description
  if (!d) return ''

  const diff = fmt(d.expectedValue - d.actualValue)

  switch (issue.categoryType) {
    case 'BaseRate':
      return (
        `Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr (${d.contextLabel}). ` +
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`
      )
    case 'PenaltyRate':
      return (
        `${d.contextLabel}: Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr. ` +
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`
      )
    case 'Superannuation':
      return (
        `Super paid $${fmt(d.actualValue)}, should be $${fmt(d.expectedValue)} (${d.contextLabel} gross). ` +
        `Underpayment: $${fmt(issue.impactAmount)}`
      )
    case 'CasualLoading':
      return (
        `Casual employee: Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr (${d.contextLabel}). ` +
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`
      )
  }
}

export function exportPayrollCsv(result: PayrollValidationResult): void {
  const headers = [
    'Employee Name',
    'Employee ID',
    'Category',
    'Severity',
    'Impact Amount',
    'Description',
  ]

  const rows = result.issues.map(issue => [
    escapeCsvField(issue.employeeName),
    escapeCsvField(issue.employeeId),
    escapeCsvField(categoryConfig[issue.categoryType].title),
    escapeCsvField(severityConfig[issue.severity].label),
    escapeCsvField(`$${fmt(issue.impactAmount)}`),
    escapeCsvField(buildIssueDescription(issue)),
  ])

  const csvContent = [
    headers.join(','),
    ...rows.map(row => row.join(',')),
  ].join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')

  const filename = `payroll-report-${result.payPeriodStart}-to-${result.payPeriodEnd}.csv`
  link.href = url
  link.download = filename
  link.click()

  URL.revokeObjectURL(url)
}

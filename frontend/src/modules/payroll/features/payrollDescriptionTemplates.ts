// Shared description template logic for payroll issues.
// Single source of truth for the 4 category-specific text templates
// (BaseRate, PenaltyRate, Superannuation, CasualLoading).
//
// Consumers:
//   - PayrollIssueRow: renders two lines as separate <Typography>
//   - exportPayrollCsv: joins two lines with '. ' for a CSV cell

import type { ValidationIssue } from '../types/payrollValidation.types'

function fmt(n: number): string {
  return n.toFixed(2)
}

export function buildDescriptionLines(
  issue: ValidationIssue
): [string, string] | null {
  const d = issue.description
  if (!d) return null

  const diff = fmt(d.expectedValue - d.actualValue)

  switch (issue.categoryType) {
    case 'BaseRate':
      return [
        `Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr (${d.contextLabel})`,
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`,
      ]
    case 'PenaltyRate':
      return [
        `${d.contextLabel}: Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr`,
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`,
      ]
    case 'Superannuation':
      return [
        `Super paid $${fmt(d.actualValue)}, should be $${fmt(d.expectedValue)} (${d.contextLabel} gross)`,
        `Underpayment: $${fmt(issue.impactAmount)}`,
      ]
    case 'CasualLoading':
      return [
        `Casual employee: Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr (${d.contextLabel})`,
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`,
      ]
  }
}

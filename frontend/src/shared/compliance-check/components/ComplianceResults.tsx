// High-level results page shell for compliance validation.
// Composes three shared building blocks: ValidationHeader (breadcrumb +
// metadata), SummaryCards (stat grid), and IssuesByCategory (accordion
// list). Roster uses this component directly; Payroll composes the
// building blocks independently with its own issue-row components.
//
// The stats array is assembled here based on resultType — roster shows
// "Critical Issues" while payroll shows "Total Underpayment" in the
// third card slot.

import React from 'react'
import { Box } from '@mui/material'
import type {
  ValidationSummary,
  ValidationMetadata,
  IssueCategory,
} from '../types/complianceCheck.type'
import { ValidationHeader } from './ValidationHeader'
import { IssuesByCategory } from './IssuesByCategory'
import { SummaryCards } from './SummaryCards'
import { exportComplianceCsv, formatMoney } from '../utils/formatters'
import { useTheme } from '@mui/material/styles'

interface ComplianceResultsProps {
  metadata: ValidationMetadata
  summary: ValidationSummary
  categories: IssueCategory[]
  onNewValidation: () => void
  onNavigateBack: () => void
  breadcrumbLabel?: string
  periodLabel?: string
  resultType?: 'payroll' | 'roster'
}

export const ComplianceResults: React.FC<ComplianceResultsProps> = ({
  metadata,
  summary,
  categories,
  onNewValidation,
  onNavigateBack,
  breadcrumbLabel = 'Roster',
  periodLabel = 'Pay period',
  resultType = 'roster',
}) => {
  const theme = useTheme()
  const handleExport = () => {
    exportComplianceCsv(metadata, categories)
  }
  // Roster: compliant | issues | critical | affected
  // Payroll: compliant | issues | underpayment | affected
  const stats =
    resultType === 'roster'
      ? [
          {
            value: summary.employeesCompliant,
            label: 'Employees Compliant',
            variant: 'success' as const,
            valueColor: theme.palette.success.main,
          },
          {
            value: summary.totalIssues,
            label: 'Total Issues Found',
            variant: 'error' as const,
            valueColor: theme.palette.error.main,
          },
          {
            value: summary.criticalIssuesCount,
            label: 'Critical Issues',
            variant: 'warning' as const,
            valueColor: theme.palette.warning.main,
          },
          {
            value: summary.employeesAffected,
            label: 'Employees Affected',
            variant: 'info' as const,
            valueColor: theme.palette.primary.main,
          },
        ]
      : [
          {
            value: summary.employeesCompliant,
            label: 'Employees Compliant',
            variant: 'success' as const,
            valueColor: theme.palette.success.main,
          },
          {
            value: summary.totalIssues,
            label: 'Total Issues Found',
            variant: 'error' as const,
            valueColor: theme.palette.error.main,
          },
          {
            value: formatMoney(summary.totalUnderpayment),
            label: 'Total Underpayment',
            variant: 'warning' as const,
            valueColor: theme.palette.warning.main,
          },
          {
            value: summary.employeesAffected,
            label: 'Employees Affected',
            variant: 'info' as const,
            valueColor: theme.palette.primary.main,
          },
        ]

  return (
    <Box>
      <ValidationHeader
        metadata={metadata}
        onNewValidation={onNewValidation}
        onExport={handleExport}
        onNavigateBack={onNavigateBack}
        breadcrumbLabel={breadcrumbLabel}
        periodLabel={periodLabel}
      />

      <SummaryCards items={stats} />

      <IssuesByCategory
        categories={categories}
        onExport={handleExport}
        resultType={resultType}
      />
    </Box>
  )
}

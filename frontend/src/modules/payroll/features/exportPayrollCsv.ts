// Payroll CSV export. Generates a downloadable CSV file from
// PayrollValidationResult. Each row represents one issue with:
// Employee Name, Employee ID, Category, Severity, Impact Amount, Description.

import type { PayrollValidationResult } from '../types/payrollValidation.types'
import { categoryConfig, severityConfig } from './payrollCategoryConfig'
import { buildDescriptionLines } from './payrollDescriptionTemplates'

function fmt(n: number): string {
  return n.toFixed(2)
}

function escapeCsvField(value: string): string {
  if (value.includes(',') || value.includes('"') || value.includes('\n')) {
    return `"${value.replace(/"/g, '""')}"`
  }
  return value
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

  const rows = result.issues.map(issue => {
    const lines = buildDescriptionLines(issue)
    const description = lines
      ? lines.join('. ')
      : (issue.warning ?? '')

    return [
      escapeCsvField(issue.employeeName),
      escapeCsvField(issue.employeeId),
      escapeCsvField(categoryConfig[issue.categoryType].title),
      escapeCsvField(severityConfig[issue.severity].label),
      escapeCsvField(`$${fmt(issue.impactAmount)}`),
      escapeCsvField(description),
    ]
  })

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

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
  if (/[,"\n\r]/.test(value)) {
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
    const description = lines ? lines.join('. ') : (issue.warning ?? '')

    const catTitle =
      categoryConfig[issue.categoryType]?.title ?? issue.categoryType
    const sevLabel =
      severityConfig[issue.severity]?.label ?? String(issue.severity)

    return [
      escapeCsvField(issue.employeeName),
      escapeCsvField(issue.employeeId),
      escapeCsvField(catTitle),
      escapeCsvField(sevLabel),
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
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)

  setTimeout(() => URL.revokeObjectURL(url), 100)
}

/**
 * Format a date string to a readable format
 * @param dateString - Date string in YYYY-MM-DD format
 * @returns Formatted date string (e.g., "25 Nov 2024")
 */
export function formatDate(dateString: string): string {
  if (!dateString) return ''

  try {
    const date = new Date(dateString)
    return date.toLocaleDateString('en-GB', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    })
  } catch {
    return dateString
  }
}

/**
 * Format a number as currency (AUD)
 * @param amount - Number or string amount
 * @returns Formatted currency string (e.g., "$1,234.56")
 */
export function formatMoney(amount: number | string): string {
  const numAmount = typeof amount === 'string' ? parseFloat(amount) : amount

  if (isNaN(numAmount)) return '$0.00'

  return new Intl.NumberFormat('en-AU', {
    style: 'currency',
    currency: 'AUD',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(numAmount)
}

/**
 * Format a timestamp to a readable date and time
 * @param timestamp - Date timestamp
 * @returns Formatted date and time string (e.g., "17 Jan 2026, 9:56 pm")
 */
export function formatDateTime(timestamp: Date | string | number): string {
  const date = timestamp instanceof Date ? timestamp : new Date(timestamp)

  const dateStr = date.toLocaleDateString('en-GB', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })

  const timeStr = date.toLocaleTimeString('en-AU', {
    hour: 'numeric',
    minute: '2-digit',
    hour12: true,
  })

  return `${dateStr}, ${timeStr}`
}

/**
 * Generate a validation ID
 * @returns A unique validation ID string
 */
export function generateValidationId(): string {
  return `VAL-${Date.now()}`
}

/**
 * Format file size for display
 * @param bytes - File size in bytes
 * @returns Formatted file size string (e.g., "1.5 MB")
 */
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 Bytes'

  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))

  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i]
}

/**
 * Escape a CSV field value
 * Wraps in quotes if contains comma, quote, or newline
 */
function escapeCsvField(value: string): string {
  if (value.includes(',') || value.includes('"') || value.includes('\n')) {
    return `"${value.replace(/"/g, '""')}"`
  }
  return value
}

interface ExportableIssue {
  name: string
  empId: string
  actualValue: string
  expectedValue: string
  variance: string
  reason: string
  breakdown: string
}

interface ExportableCategory {
  title: string
  issues: ExportableIssue[]
}

interface ExportMetadata {
  award: string
  weekStarting: string
  weekEnding: string
  validatedAt?: string
  validationId?: string
}

/**
 * Export compliance results to CSV and trigger download
 */
export function exportComplianceCsv(
  metadata: ExportMetadata,
  categories: ExportableCategory[]
): void {
  const headers = [
    'Category',
    'Employee',
    'Employee ID',
    'Actual',
    'Expected',
    'Variance',
    'Reason',
    'Breakdown',
  ]

  const rows = categories.flatMap(category =>
    category.issues.map(issue => [
      escapeCsvField(category.title),
      escapeCsvField(issue.name),
      escapeCsvField(issue.empId),
      escapeCsvField(issue.actualValue),
      escapeCsvField(issue.expectedValue),
      escapeCsvField(issue.variance),
      escapeCsvField(issue.reason),
      escapeCsvField(issue.breakdown),
    ])
  )

  const csvContent = [headers.join(','), ...rows.map(row => row.join(','))].join(
    '\n'
  )

  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')

  const filename = `compliance-report-${metadata.weekStarting}-to-${metadata.weekEnding}.csv`
  link.href = url
  link.download = filename
  link.click()

  URL.revokeObjectURL(url)
}

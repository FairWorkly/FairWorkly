// TODO: Align with backend DTO once API response is finalized.
export interface ComplianceApiResponse {
  metadata?: {
    award?: string
    pay_period?: {
      start?: string
      end?: string
    }
    validated_at?: string
    validation_id?: string
  }
  summary?: {
    employees_compliant?: number
    total_issues?: number
    critical_issues_count?: number
    total_underpayment?: string
    total_variance?: string
    employees_affected?: number
  }
  categories?: Array<{
    id?: string
    title?: string
    icon?: string
    color?: string
    employee_count?: number
    total_underpayment?: string
    total_variance?: string
    issues?: Array<{
      id?: number
      name?: string
      emp_id?: string
      actual_value?: string
      expected_value?: string
      reason?: string
      variance?: string
      breakdown?: string
    }>
  }>
  [key: string]: unknown
}

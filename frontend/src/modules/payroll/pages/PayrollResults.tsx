import { useNavigate } from 'react-router-dom'
import {
  ComplianceResults,
  mapBackendToComplianceResults,
} from '@/shared/compliance-check'
import type { ComplianceApiResponse } from '@/shared/compliance-check'

// TODO: [Backend Integration] Replace mock data with real API call.
// When integrating with backend:
// 1. Get validation ID from route params (e.g., useParams<{ id: string }>())
// 2. Fetch results from API (e.g., GET /api/compliance/results/:id)
// 3. Add loading and error states
// 4. Remove mockBackendPayload after integration

const mockBackendPayload: ComplianceApiResponse = {
  metadata: {
    award: 'General Retail Industry Award',
    pay_period: {
      start: '2026-01-01',
      end: '2026-01-07',
    },
    validated_at: 'Jan 8, 2026',
    validation_id: 'VAL-001',
  },
  summary: {
    employees_compliant: 12,
    total_issues: 10,
    total_underpayment: '$2,830.00',
    employees_affected: 8,
  },
  categories: [
    {
      id: 'base-rate',
      title: 'Base Rate Issues',
      icon: 'attach_money',
      color: '#ef4444',
      employee_count: 2,
      total_underpayment: '$480.00',
      issues: [
        {
          id: 1,
          name: 'Avery Johnson',
          emp_id: 'EMP-001',
          actual_value: '$650.00',
          expected_value: '$800.00',
          reason: 'base rate misclassified',
          variance: '$150.00',
          breakdown: 'Incorrect level applied for 20 hours.',
        },
        {
          id: 2,
          name: 'Jordan Lee',
          emp_id: 'EMP-002',
          actual_value: '$720.00',
          expected_value: '$1,050.00',
          reason: 'base rate below minimum',
          variance: '$330.00',
          breakdown: 'Hourly rate $22.50 below award minimum $26.75.',
        },
      ],
    },
    {
      id: 'penalty-rates',
      title: 'Penalty Rates',
      icon: 'gavel',
      color: '#f97316',
      employee_count: 2,
      total_underpayment: '$620.00',
      issues: [
        {
          id: 3,
          name: 'Samantha Williams',
          emp_id: 'EMP-003',
          actual_value: '$380.00',
          expected_value: '$570.00',
          reason: 'weekend penalties missing',
          variance: '$190.00',
          breakdown:
            'Saturday 150% and Sunday 200% rates not applied for 8 hours.',
        },
        {
          id: 4,
          name: 'Michael Chen',
          emp_id: 'EMP-004',
          actual_value: '$420.00',
          expected_value: '$850.00',
          reason: 'public holiday rate missing',
          variance: '$430.00',
          breakdown:
            'Public holiday 250% rate not applied for 6 hours on Jan 1.',
        },
      ],
    },
    {
      id: 'overtime',
      title: 'Overtime Issues',
      icon: 'schedule',
      color: '#eab308',
      employee_count: 2,
      total_underpayment: '$545.00',
      issues: [
        {
          id: 5,
          name: 'Emily Rodriguez',
          emp_id: 'EMP-005',
          actual_value: '$1,200.00',
          expected_value: '$1,450.00',
          reason: 'overtime not calculated',
          variance: '$250.00',
          breakdown:
            'Worked 48 hours but overtime (150%) not applied for 10 hours.',
        },
        {
          id: 6,
          name: 'David Kim',
          emp_id: 'EMP-006',
          actual_value: '$980.00',
          expected_value: '$1,275.00',
          reason: 'double time missing',
          variance: '$295.00',
          breakdown:
            'Hours beyond 10hrs/day should be 200%, only paid at 150%.',
        },
      ],
    },
    {
      id: 'allowances',
      title: 'Allowances',
      icon: 'card_giftcard',
      color: '#22c55e',
      employee_count: 2,
      total_underpayment: '$385.00',
      issues: [
        {
          id: 7,
          name: 'Sarah Thompson',
          emp_id: 'EMP-007',
          actual_value: '$850.00',
          expected_value: '$1,010.00',
          reason: 'meal allowance missing',
          variance: '$160.00',
          breakdown:
            'Worked 5 shifts over 6 hours without meal allowance ($32 each).',
        },
        {
          id: 8,
          name: 'James Wilson',
          emp_id: 'EMP-008',
          actual_value: '$920.00',
          expected_value: '$1,145.00',
          reason: 'uniform allowance missing',
          variance: '$225.00',
          breakdown: 'Weekly uniform allowance of $45 not paid for 5 weeks.',
        },
      ],
    },
    {
      id: 'leave',
      title: 'Leave Entitlements',
      icon: 'beach_access',
      color: '#3b82f6',
      employee_count: 2,
      total_underpayment: '$800.00',
      issues: [
        {
          id: 9,
          name: 'Christopher Martinez',
          emp_id: 'EMP-009',
          actual_value: '$0.00',
          expected_value: '$450.00',
          reason: 'annual leave underpaid',
          variance: '$450.00',
          breakdown:
            '3 days annual leave paid at base rate instead of leave loading (17.5%).',
        },
        {
          id: 10,
          name: 'Jessica Brown',
          emp_id: 'EMP-010',
          actual_value: '$200.00',
          expected_value: '$550.00',
          reason: 'sick leave not paid',
          variance: '$350.00',
          breakdown: '2 days sick leave not paid at full rate.',
        },
      ],
    },
  ],
}

export function PayrollResults() {
  const navigate = useNavigate()
  const { metadata, summary, categories } =
    mapBackendToComplianceResults(mockBackendPayload)

  return (
    <ComplianceResults
      metadata={metadata}
      summary={summary}
      categories={categories}
      onNewValidation={() => navigate('/payroll/upload')}
      onNavigateBack={() => navigate('/payroll/upload')}
    />
  )
}

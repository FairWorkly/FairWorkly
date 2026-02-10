import { useNavigate } from 'react-router-dom'
import {
  ComplianceResults,
  mapBackendToComplianceResults,
} from '@/shared/compliance-check'
import type { ComplianceApiResponse } from '@/shared/compliance-check'

// TODO: [Backend Integration] Replace mock data with real API call.
// When integrating with backend:
// 1. Get rosterId from route params (useParams<{ rosterId: string }>())
// 2. Fetch compliance results from API (e.g., GET /api/roster/:rosterId/compliance)
// 3. Add loading and error states
// 4. Remove mockBackendPayload after integration

const mockBackendPayload: ComplianceApiResponse = {
  metadata: {
    award: 'Clerks—Private Sector Award',
    pay_period: {
      start: '2026-02-02',
      end: '2026-02-08',
    },
    validated_at: 'Feb 10, 2026',
    validation_id: 'RVAL-001',
  },
  summary: {
    employees_compliant: 8,
    total_issues: 7,
    critical_issues_count: 3,
    employees_affected: 5,
  },
  categories: [
    {
      id: 'minimum-hours',
      title: 'Minimum Shift Hours',
      icon: 'schedule',
      color: '#ef4444',
      employee_count: 2,
      total_underpayment: '2 violations',
      issues: [
        {
          id: 1,
          name: 'Tom Scott',
          emp_id: 'CLK002',
          actual_value: '2.0 hrs',
          expected_value: '3.0 hrs',
          reason: 'shift below minimum hours',
          variance: '1.0 hr short',
          breakdown:
            'Shift on Mon 2 Feb (10:00–12:00) is 2 hours, below the 3-hour minimum under Clerks Award clause 13.4.',
        },
        {
          id: 2,
          name: 'Lisa Park',
          emp_id: 'CLK005',
          actual_value: '2.5 hrs',
          expected_value: '3.0 hrs',
          reason: 'shift below minimum hours',
          variance: '0.5 hr short',
          breakdown:
            'Shift on Wed 4 Feb (14:00–16:30) is 2.5 hours, below the 3-hour minimum.',
        },
      ],
    },
    {
      id: 'meal-breaks',
      title: 'Meal Break Requirements',
      icon: 'restaurant',
      color: '#f97316',
      employee_count: 2,
      total_underpayment: '3 violations',
      issues: [
        {
          id: 3,
          name: 'Rachel Green',
          emp_id: 'CLK001',
          actual_value: 'No break',
          expected_value: '30 min break',
          reason: 'meal break not scheduled',
          variance: 'Missing',
          breakdown:
            'Shift on Mon 2 Feb (09:00–17:00) is 8 hours with no meal break. Award requires unpaid meal break of 30–60 min for shifts over 5 hours.',
        },
        {
          id: 4,
          name: 'Rachel Green',
          emp_id: 'CLK001',
          actual_value: 'No break',
          expected_value: '30 min break',
          reason: 'meal break not scheduled',
          variance: 'Missing',
          breakdown:
            'Shift on Tue 3 Feb (09:00–17:00) is 8 hours with no meal break.',
        },
        {
          id: 5,
          name: 'David Wong',
          emp_id: 'CLK003',
          actual_value: 'No break',
          expected_value: '30 min break',
          reason: 'meal break not scheduled',
          variance: 'Missing',
          breakdown:
            'Shift on Thu 5 Feb (08:00–16:00) is 8 hours with no meal break.',
        },
      ],
    },
    {
      id: 'rest-periods',
      title: 'Rest Period Between Shifts',
      icon: 'bedtime',
      color: '#eab308',
      employee_count: 1,
      total_underpayment: '1 violation',
      issues: [
        {
          id: 6,
          name: 'James Miller',
          emp_id: 'CLK004',
          actual_value: '8 hrs gap',
          expected_value: '10 hrs gap',
          reason: 'insufficient rest between shifts',
          variance: '2 hrs short',
          breakdown:
            'Shift ended Fri 6 Feb at 22:00, next shift starts Sat 7 Feb at 06:00. Only 8 hours rest; Award requires minimum 10 hours between shifts.',
        },
      ],
    },
    {
      id: 'weekly-hours',
      title: 'Weekly Hours Limit',
      icon: 'timer',
      color: '#3b82f6',
      employee_count: 1,
      total_underpayment: '1 violation',
      issues: [
        {
          id: 7,
          name: 'Rachel Green',
          emp_id: 'CLK001',
          actual_value: '48.0 hrs',
          expected_value: '38.0 hrs max',
          reason: 'exceeds maximum ordinary hours',
          variance: '10 hrs over',
          breakdown:
            'Rostered for 48 ordinary hours this week. Maximum ordinary hours under Clerks Award is 38 per week. Excess hours require overtime rates.',
        },
      ],
    },
  ],
}

export function RosterResults() {
  const navigate = useNavigate()
  const { metadata, summary, categories } =
    mapBackendToComplianceResults(mockBackendPayload)

  return (
    <ComplianceResults
      metadata={metadata}
      summary={summary}
      categories={categories}
      onNewValidation={() => navigate('/roster/upload')}
      onNavigateBack={() => navigate('/roster/upload')}
      breadcrumbLabel="Roster"
      periodLabel="Roster period"
      resultType="roster"
    />
  )
}

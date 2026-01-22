import { useNavigate } from 'react-router-dom'
import {
  ComplianceResults,
  mapBackendToComplianceResults,
} from '@/shared/compliance-check'
import type { ComplianceApiResponse } from '@/shared/compliance-check'

// TODO: [Backend Integration] Replace mock data with real API call.
// When integrating with backend:
// 1. Get validation ID from route params (e.g., useParams<{ id: string }>())
// 2. Fetch results from API (e.g., GET /api/roster/results/:id)
// 3. Add loading and error states
// 4. Remove mockBackendPayload after integration

const mockBackendPayload: ComplianceApiResponse = {
  metadata: {
    award: 'Hospitality Industry Award',
    pay_period: {
      start: '2026-01-13',
      end: '2026-01-19',
    },
    validated_at: 'Jan 12, 2026',
    validation_id: 'RST-001',
  },
  summary: {
    employees_compliant: 18,
    total_issues: 14,
    critical_issues_count: 3,
    total_variance: '14 issues',
    employees_affected: 9,
  },
  categories: [
    {
      id: 'minimum-hours',
      title: 'Minimum Hours',
      icon: 'timer',
      color: '#ef4444',
      employee_count: 3,
      total_variance: '3 issues',
      issues: [
        {
          id: 1,
          name: 'Lisa Chen',
          emp_id: 'EMP-101',
          actual_value: '2 hrs',
          expected_value: '3 hrs',
          reason: 'shift below minimum',
          variance: '-1 hr',
          breakdown:
            'Monday shift is 2 hours, below the 3-hour minimum engagement.',
        },
        {
          id: 2,
          name: 'Tom Anderson',
          emp_id: 'EMP-102',
          actual_value: '2.5 hrs',
          expected_value: '3 hrs',
          reason: 'shift below minimum',
          variance: '-0.5 hr',
          breakdown:
            'Wednesday shift is 2.5 hours, below the 3-hour minimum engagement.',
        },
        {
          id: 3,
          name: 'Maria Garcia',
          emp_id: 'EMP-103',
          actual_value: '2 hrs',
          expected_value: '3 hrs',
          reason: 'shift below minimum',
          variance: '-1 hr',
          breakdown:
            'Saturday shift is 2 hours, below the 3-hour minimum engagement.',
        },
      ],
    },
    {
      id: 'break-requirements',
      title: 'Break Requirements',
      icon: 'free_breakfast',
      color: '#f97316',
      employee_count: 2,
      total_variance: '2 issues',
      issues: [
        {
          id: 4,
          name: 'James Wilson',
          emp_id: 'EMP-104',
          actual_value: '0 min',
          expected_value: '30 min',
          reason: 'meal break missing',
          variance: '-30 min',
          breakdown:
            'Thursday 7-hour shift has no scheduled meal break (required after 5 hours).',
        },
        {
          id: 5,
          name: 'Sophie Brown',
          emp_id: 'EMP-105',
          actual_value: '0 min',
          expected_value: '30 min',
          reason: 'meal break missing',
          variance: '-30 min',
          breakdown:
            'Sunday 8-hour shift has no scheduled meal break (required after 5 hours).',
        },
      ],
    },
    {
      id: 'rest-between-shifts',
      title: 'Rest Between Shifts',
      icon: 'hotel',
      color: '#eab308',
      employee_count: 2,
      total_variance: '2 issues',
      issues: [
        {
          id: 6,
          name: 'Kevin Lee',
          emp_id: 'EMP-106',
          actual_value: '8 hrs',
          expected_value: '10 hrs',
          reason: 'insufficient rest period',
          variance: '-2 hrs',
          breakdown:
            'Only 8 hours between Friday close (11pm) and Saturday open (7am). Minimum 10 hours required.',
        },
        {
          id: 7,
          name: 'Emma Davis',
          emp_id: 'EMP-107',
          actual_value: '9 hrs',
          expected_value: '10 hrs',
          reason: 'insufficient rest period',
          variance: '-1 hr',
          breakdown:
            'Only 9 hours between Saturday close (10pm) and Sunday open (7am). Minimum 10 hours required.',
        },
      ],
    },
    {
      id: 'maximum-hours',
      title: 'Maximum Hours',
      icon: 'warning',
      color: '#dc2626',
      employee_count: 2,
      total_variance: '2 issues',
      issues: [
        {
          id: 8,
          name: 'Ryan Taylor',
          emp_id: 'EMP-108',
          actual_value: '52 hrs',
          expected_value: '38 hrs max',
          reason: 'exceeds maximum ordinary hours',
          variance: '+14 hrs',
          breakdown:
            'Scheduled for 52 hours this week, exceeds 38-hour maximum for full-time employees.',
        },
        {
          id: 9,
          name: 'Amy Johnson',
          emp_id: 'EMP-109',
          actual_value: '45 hrs',
          expected_value: '38 hrs max',
          reason: 'exceeds maximum ordinary hours',
          variance: '+7 hrs',
          breakdown:
            'Scheduled for 45 hours this week, exceeds 38-hour maximum for full-time employees.',
        },
      ],
    },
    {
      id: 'consecutive-days',
      title: 'Consecutive Days',
      icon: 'date_range',
      color: '#3b82f6',
      employee_count: 2,
      total_variance: '2 issues',
      issues: [
        {
          id: 10,
          name: 'Chris Martin',
          emp_id: 'EMP-110',
          actual_value: '8 days',
          expected_value: '6 days max',
          reason: 'exceeds consecutive days',
          variance: '+2 days',
          breakdown:
            'Scheduled for 8 consecutive days (Jan 12-19). Maximum 6 consecutive days allowed.',
        },
        {
          id: 11,
          name: 'Sarah White',
          emp_id: 'EMP-111',
          actual_value: '7 days',
          expected_value: '6 days max',
          reason: 'exceeds consecutive days',
          variance: '+1 day',
          breakdown:
            'Scheduled for 7 consecutive days (Jan 13-19). Maximum 6 consecutive days allowed.',
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
      periodLabel="Week"
      resultType="roster"
    />
  )
}

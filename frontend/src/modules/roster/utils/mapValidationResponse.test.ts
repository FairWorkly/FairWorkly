import { describe, it, expect } from 'vitest'
import { mapValidationToComplianceResults } from './mapValidationResponse'
import type { ValidateRosterResponse } from '@/services/rosterApi'

function createBaseResponse(
  overrides: Partial<ValidateRosterResponse> = {}
): ValidateRosterResponse {
  return {
    validationId: 'aaaa-bbbb-cccc-dddd',
    status: 'Failed',
    totalShifts: 10,
    passedShifts: 8,
    failedShifts: 2,
    totalIssues: 3,
    criticalIssues: 2,
    affectedEmployees: 2,
    weekStartDate: '2026-02-02',
    weekEndDate: '2026-02-08',
    totalEmployees: 5,
    validatedAt: '2026-02-03T10:30:00Z',
    failureType: null,
    retriable: null,
    issues: [],
    ...overrides,
  }
}

describe('mapValidationToComplianceResults', () => {
  it('maps metadata correctly', () => {
    const response = createBaseResponse()
    const result = mapValidationToComplianceResults(response)

    expect(result.metadata).toBeDefined()
    expect(result.metadata.award).toBe('Australian Workplace Award')
    expect(result.metadata.weekStarting).toBe('2026-02-02')
    expect(result.metadata.weekEnding).toBe('2026-02-08')
    expect(result.metadata.validationId).toBe('AAAA-BBB')
  })

  it('maps summary correctly', () => {
    const response = createBaseResponse({
      totalEmployees: 10,
      totalIssues: 5,
      criticalIssues: 3,
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'MinimumShiftHours',
          severity: 'Error',
          description: 'Shift too short',
          expectedValue: 3,
          actualValue: 2,
          affectedDates: null,
        },
        {
          id: '2',
          shiftId: null,
          employeeId: 'emp-2',
          employeeName: 'Bob',
          employeeNumber: 'E002',
          checkType: 'MealBreak',
          severity: 'Error',
          description: 'No meal break',
          expectedValue: null,
          actualValue: null,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)

    expect(result.summary).toBeDefined()
    // Both counts are derived from the filtered compliance issues only,
    // so they stay consistent (DataQuality issues excluded from both).
    expect(result.summary.totalIssues).toBe(2)
    expect(result.summary.criticalIssuesCount).toBe(2)
    expect(result.summary.employeesAffected).toBe(2)
    // 10 total - 2 affected = 8 compliant
    expect(result.summary.employeesCompliant).toBe(8)
  })

  it('groups issues by checkType into categories', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: 's1',
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'MinimumShiftHours',
          severity: 'Error',
          description: 'Shift too short',
          expectedValue: 3,
          actualValue: 2,
          affectedDates: null,
        },
        {
          id: '2',
          shiftId: 's2',
          employeeId: 'emp-2',
          employeeName: 'Bob',
          employeeNumber: 'E002',
          checkType: 'MinimumShiftHours',
          severity: 'Error',
          description: 'Another short shift',
          expectedValue: 3,
          actualValue: 1.5,
          affectedDates: null,
        },
        {
          id: '3',
          shiftId: 's3',
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'MealBreak',
          severity: 'Error',
          description: 'No meal break',
          expectedValue: null,
          actualValue: 0,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)

    expect(result.categories).toHaveLength(2)

    const minHoursCategory = result.categories.find(
      c => c.id === 'minimum-hours'
    )
    expect(minHoursCategory).toBeDefined()
    expect(minHoursCategory!.title).toBe('Minimum Shift Hours')
    expect(minHoursCategory!.icon).toBe('schedule')
    expect(minHoursCategory!.color).toBe('#ef4444')
    expect(minHoursCategory!.employeeCount).toBe(2) // 2 unique employees
    expect(minHoursCategory!.issues).toHaveLength(2)
    expect(minHoursCategory!.totalUnderpayment).toBe('2 violations')

    const mealBreakCategory = result.categories.find(
      c => c.id === 'meal-breaks'
    )
    expect(mealBreakCategory).toBeDefined()
    expect(mealBreakCategory!.title).toBe('Meal Break Requirements')
    expect(mealBreakCategory!.employeeCount).toBe(1)
    expect(mealBreakCategory!.totalUnderpayment).toBe('1 violation')

    const allMappedIds = result.categories.flatMap(c => c.issues.map(i => i.id))
    expect(new Set(allMappedIds).size).toBe(allMappedIds.length)
  })

  it('maps issue details correctly', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: 's1',
          employeeId: 'emp-1',
          employeeName: 'Alice Smith',
          employeeNumber: 'E001',
          checkType: 'MinimumShiftHours',
          severity: 'Error',
          description: 'Shift only 2 hours, minimum is 3 hours',
          expectedValue: 3,
          actualValue: 2,
          affectedDates: '2026-02-02',
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)
    const issue = result.categories[0].issues[0]

    expect(issue.id).toBe(1)
    expect(issue.name).toBe('Alice Smith')
    expect(issue.empId).toBe('E001')
    expect(issue.expectedValue).toBe('3 hrs')
    expect(issue.actualValue).toBe('2 hrs')
    expect(issue.reason).toBe('Shift only 2 hours, minimum is 3 hours')
    expect(issue.variance).toBe('1 hr short')
  })

  it('handles empty issues list', () => {
    const response = createBaseResponse({
      totalEmployees: 5,
      issues: [],
    })

    const result = mapValidationToComplianceResults(response)

    expect(result.categories).toHaveLength(0)
    expect(result.summary.employeesCompliant).toBe(5)
    expect(result.summary.employeesAffected).toBe(0)
  })

  it('handles unknown checkType with fallback display', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: null,
          checkType: 'UnknownCheck',
          severity: 'Warning',
          description: 'Some unknown check',
          expectedValue: null,
          actualValue: null,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)

    expect(result.categories).toHaveLength(1)
    const category = result.categories[0]
    expect(category.title).toBe('UnknownCheck')
    expect(category.icon).toBe('report_problem')
    expect(category.color).toBe('#ef4444')
  })

  it('handles null employeeName by using "Unknown"', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1-uuid-value',
          employeeName: null,
          employeeNumber: null,
          checkType: 'MinimumShiftHours',
          severity: 'Warning',
          description: 'Shift too short',
          expectedValue: 3,
          actualValue: 2,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)
    const issue = result.categories[0].issues[0]

    expect(issue.name).toBe('Unknown')
    // Falls back to first 8 chars of employeeId
    expect(issue.empId).toBe('emp-1-uu')
  })

  it('formats MealBreak values correctly', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'MealBreak',
          severity: 'Error',
          description: 'No break',
          expectedValue: 30,
          actualValue: 0,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)
    const issue = result.categories[0].issues[0]

    expect(issue.expectedValue).toBe('30 min break')
    expect(issue.actualValue).toBe('No break')
    expect(issue.variance).toBe('30 mins short')
  })

  it('formats RestPeriodBetweenShifts values correctly', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'RestPeriodBetweenShifts',
          severity: 'Error',
          description: 'Insufficient rest',
          expectedValue: 10,
          actualValue: 8,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)
    const issue = result.categories[0].issues[0]

    expect(issue.expectedValue).toBe('10 hrs gap')
    expect(issue.actualValue).toBe('8 hrs gap')
    expect(issue.variance).toBe('2 hrs short')
  })

  it('formats MaximumConsecutiveDays values correctly', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'MaximumConsecutiveDays',
          severity: 'Error',
          description: 'Too many days',
          expectedValue: 6,
          actualValue: 8,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)
    const issue = result.categories[0].issues[0]

    expect(issue.expectedValue).toBe('6 days')
    expect(issue.actualValue).toBe('8 days')
    expect(issue.variance).toBe('2 days over')
  })

  it('handles null values with N/A', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'WeeklyHoursLimit',
          severity: 'Warning',
          description: 'Hours issue',
          expectedValue: null,
          actualValue: null,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)
    const issue = result.categories[0].issues[0]

    expect(issue.expectedValue).toBe('N/A')
    expect(issue.actualValue).toBe('N/A')
    expect(issue.variance).toBe('N/A')
  })

  it('filters out DataQuality issues from results', () => {
    const response = createBaseResponse({
      totalEmployees: 5,
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'DataQuality',
          severity: 'Warning',
          description: 'Data issue',
          expectedValue: null,
          actualValue: null,
          affectedDates: null,
        },
        {
          id: '2',
          shiftId: null,
          employeeId: 'emp-2',
          employeeName: 'Bob',
          employeeNumber: 'E002',
          checkType: 'MinimumShiftHours',
          severity: 'Error',
          description: 'Shift too short',
          expectedValue: 3,
          actualValue: 2,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)

    // DataQuality category should not appear
    expect(result.categories).toHaveLength(1)
    expect(result.categories[0].id).toBe('minimum-hours')

    // Stats should exclude DataQuality issues
    expect(result.summary.totalIssues).toBe(1)
    expect(result.summary.employeesAffected).toBe(1) // only emp-2
    expect(result.summary.employeesCompliant).toBe(4) // 5 - 1
  })

  it('validatedAt is undefined when null', () => {
    const response = createBaseResponse({ validatedAt: null })
    const result = mapValidationToComplianceResults(response)

    expect(result.metadata.validatedAt).toBeUndefined()
  })

  it('singular violation text for single issue', () => {
    const response = createBaseResponse({
      issues: [
        {
          id: '1',
          shiftId: null,
          employeeId: 'emp-1',
          employeeName: 'Alice',
          employeeNumber: 'E001',
          checkType: 'MealBreak',
          severity: 'Error',
          description: 'No break',
          expectedValue: null,
          actualValue: null,
          affectedDates: null,
        },
      ],
    })

    const result = mapValidationToComplianceResults(response)
    expect(result.categories[0].totalUnderpayment).toBe('1 violation')
  })
})

import { describe, it, expect, vi, beforeEach } from 'vitest'
import type { ValidateRosterResponse } from '@/services/rosterApi'

// Mock React's useMemo to run the factory directly (no React dispatcher in Node)
vi.mock('react', async () => {
  const actual = await vi.importActual<typeof import('react')>('react')
  return {
    ...actual,
    useMemo: (fn: () => unknown) => fn(),
  }
})

vi.mock('@/services/rosterApi', () => ({
  getValidationResults: vi.fn(),
  validateRoster: vi.fn(),
}))

vi.mock('@/shared/hooks/useApiQuery', () => ({
  useApiQuery: vi.fn(),
}))

vi.mock('../utils/mapValidationResponse', () => ({
  mapValidationToComplianceResults: vi.fn(),
}))

import { useApiQuery } from '@/shared/hooks/useApiQuery'
import { getValidationResults, validateRoster } from '@/services/rosterApi'
import { mapValidationToComplianceResults } from '../utils/mapValidationResponse'
import { useValidateRoster } from './useValidateRoster'

const mockUseApiQuery = useApiQuery as ReturnType<typeof vi.fn>
const mockGetValidation = getValidationResults as ReturnType<typeof vi.fn>
const mockValidateRoster = validateRoster as ReturnType<typeof vi.fn>
const mockMapValidation = mapValidationToComplianceResults as ReturnType<
  typeof vi.fn
>

describe('useValidateRoster', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('passes correct queryKey and enabled=true when rosterId is provided', () => {
    mockUseApiQuery.mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
      error: null,
    })

    useValidateRoster('test-roster-id')

    expect(mockUseApiQuery).toHaveBeenCalledWith(
      expect.objectContaining({
        queryKey: ['roster', 'validate', 'test-roster-id'],
        enabled: true,
      })
    )
  })

  it('sets enabled=false when rosterId is undefined', () => {
    mockUseApiQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: false,
      error: null,
    })

    useValidateRoster(undefined)

    expect(mockUseApiQuery).toHaveBeenCalledWith(
      expect.objectContaining({
        enabled: false,
      })
    )
  })

  it('returns isError=true and message when rosterId is undefined', () => {
    mockUseApiQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: false,
      error: null,
    })

    const result = useValidateRoster(undefined)

    expect(result.isError).toBe(true)
    expect(result.errorMessage).toBe(
      'No roster ID provided. Please upload a roster first.'
    )
    expect(result.complianceData).toBeNull()
  })

  it('returns loading state from query', () => {
    mockUseApiQuery.mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
      error: null,
    })

    const result = useValidateRoster('some-id')

    expect(result.isLoading).toBe(true)
    expect(result.complianceData).toBeNull()
  })

  it('transforms data with mapValidationToComplianceResults when query succeeds', () => {
    const mockResponse: ValidateRosterResponse = {
      validationId: 'val-id',
      status: 'Passed',
      totalShifts: 10,
      passedShifts: 10,
      failedShifts: 0,
      totalIssues: 0,
      criticalIssues: 0,
      affectedEmployees: 0,
      weekStartDate: '2026-02-02',
      weekEndDate: '2026-02-08',
      totalEmployees: 5,
      validatedAt: null,
      failureType: null,
      retriable: null,
      issues: [],
    }

    const mockComplianceData = {
      metadata: { award: 'test' },
      summary: { totalIssues: 0 },
      categories: [],
    }

    mockUseApiQuery.mockReturnValue({
      data: mockResponse,
      isLoading: false,
      isError: false,
      error: null,
    })
    mockMapValidation.mockReturnValue(mockComplianceData)

    const result = useValidateRoster('test-id')

    expect(mockMapValidation).toHaveBeenCalledWith(mockResponse)
    expect(result.complianceData).toBe(mockComplianceData)
    expect(result.isLoading).toBe(false)
    expect(result.isError).toBe(false)
  })

  it('returns error message from query error', () => {
    mockUseApiQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: true,
      error: { message: 'Roster not found', status: 404 },
    })

    const result = useValidateRoster('bad-id')

    expect(result.isError).toBe(true)
    expect(result.errorMessage).toBe('Roster not found')
    expect(result.complianceData).toBeNull()
  })

  it('returns fallback error message when error has no message', () => {
    mockUseApiQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: true,
      error: null,
    })

    const result = useValidateRoster('bad-id')

    expect(result.isError).toBe(true)
    expect(result.errorMessage).toBe('Validation failed. Please try again.')
  })

  describe('queryFn (getOrValidate)', () => {
    it('uses GET first when validation already exists', async () => {
      const mockResponse = { validationId: 'existing' }
      mockGetValidation.mockResolvedValue(mockResponse)

      mockUseApiQuery.mockImplementation(
        ({ queryFn }: { queryFn: () => Promise<unknown> }) => {
          // Capture and invoke the queryFn
          queryFn()
          return {
            data: undefined,
            isLoading: true,
            isError: false,
            error: null,
          }
        }
      )

      useValidateRoster('roster-123')

      expect(mockGetValidation).toHaveBeenCalledWith('roster-123')
      expect(mockValidateRoster).not.toHaveBeenCalled()
    })

    it('falls back to POST when GET returns 404', async () => {
      const mockResponse = { validationId: 'new' }
      mockGetValidation.mockRejectedValue({ status: 404, message: 'Not found' })
      mockValidateRoster.mockResolvedValue(mockResponse)

      let capturedQueryFn: (() => Promise<unknown>) | undefined
      mockUseApiQuery.mockImplementation(
        ({ queryFn }: { queryFn: () => Promise<unknown> }) => {
          capturedQueryFn = queryFn
          return {
            data: undefined,
            isLoading: true,
            isError: false,
            error: null,
          }
        }
      )

      useValidateRoster('roster-456')

      const result = await capturedQueryFn!()

      expect(mockGetValidation).toHaveBeenCalledWith('roster-456')
      expect(mockValidateRoster).toHaveBeenCalledWith('roster-456')
      expect(result).toBe(mockResponse)
    })

    it('re-throws non-404 errors from GET', async () => {
      const serverError = { status: 500, message: 'Server error' }
      mockGetValidation.mockRejectedValue(serverError)

      let capturedQueryFn: (() => Promise<unknown>) | undefined
      mockUseApiQuery.mockImplementation(
        ({ queryFn }: { queryFn: () => Promise<unknown> }) => {
          capturedQueryFn = queryFn
          return {
            data: undefined,
            isLoading: true,
            isError: false,
            error: null,
          }
        }
      )

      useValidateRoster('roster-789')

      await expect(capturedQueryFn!()).rejects.toEqual(serverError)
      expect(mockValidateRoster).not.toHaveBeenCalled()
    })
  })
})

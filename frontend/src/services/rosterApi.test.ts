import { describe, it, expect, vi, beforeEach } from 'vitest'
import type { AxiosResponse } from 'axios'

// Mock httpClient module before imports
vi.mock('./httpClient', () => {
  return {
    default: {
      get: vi.fn(),
      post: vi.fn(),
    },
  }
})

import httpClient from './httpClient'
import {
  uploadRoster,
  getRosterDetails,
  validateRoster,
  getValidationResults,
} from './rosterApi'

const mockClient = httpClient as unknown as {
  get: ReturnType<typeof vi.fn>
  post: ReturnType<typeof vi.fn>
}

function mockAxiosResponse<T>(data: T): AxiosResponse<T> {
  return {
    data,
    status: 200,
    statusText: 'OK',
    headers: {},
    config: {} as AxiosResponse['config'],
  }
}

describe('rosterApi', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('uploadRoster', () => {
    it('sends file as multipart/form-data and returns response', async () => {
      const mockResponse = {
        rosterId: 'test-roster-id',
        weekStartDate: '2026-02-02',
        weekEndDate: '2026-02-08',
        totalShifts: 10,
        totalHours: 80,
        totalEmployees: 5,
        warnings: [],
      }

      mockClient.post.mockResolvedValue(mockAxiosResponse(mockResponse))

      const file = new File(['content'], 'roster.xlsx', {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      })

      const result = await uploadRoster(file)

      expect(mockClient.post).toHaveBeenCalledWith(
        '/roster/upload',
        expect.any(FormData),
        expect.objectContaining({
          headers: { 'Content-Type': 'multipart/form-data' },
        }),
      )
      expect(result).toEqual(mockResponse)
    })

    it('throws normalized error on failure', async () => {
      const axiosError = {
        isAxiosError: true,
        response: {
          status: 422,
          data: { message: 'Invalid file format' },
        },
        message: 'Request failed',
      }
      mockClient.post.mockRejectedValue(axiosError)

      const file = new File(['content'], 'bad.xlsx')

      await expect(uploadRoster(file)).rejects.toMatchObject({
        status: 422,
        message: 'Invalid file format',
      })
    })
  })

  describe('getRosterDetails', () => {
    it('calls GET /roster/:id and returns response', async () => {
      const mockResponse = {
        rosterId: 'test-id',
        weekStartDate: '2026-02-02',
        weekEndDate: '2026-02-08',
        weekNumber: 6,
        year: 2026,
        totalShifts: 10,
        totalHours: 80,
        totalEmployees: 5,
        isFinalized: false,
        originalFileName: null,
        createdAt: '2026-02-01T10:00:00Z',
        hasValidation: false,
        validationId: null,
        employees: [],
      }

      mockClient.get.mockResolvedValue(mockAxiosResponse(mockResponse))

      const result = await getRosterDetails('test-id')

      expect(mockClient.get).toHaveBeenCalledWith('/roster/test-id')
      expect(result).toEqual(mockResponse)
    })
  })

  describe('validateRoster', () => {
    it('calls POST /roster/:id/validate and returns response', async () => {
      const mockResponse = {
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
        validatedAt: '2026-02-03T10:00:00Z',
        issues: [],
      }

      mockClient.post.mockResolvedValue(mockAxiosResponse(mockResponse))

      const result = await validateRoster('test-roster-id')

      expect(mockClient.post).toHaveBeenCalledWith(
        '/roster/test-roster-id/validate',
      )
      expect(result).toEqual(mockResponse)
    })

    it('throws normalized error on failure', async () => {
      const axiosError = {
        isAxiosError: true,
        response: {
          status: 404,
          data: { message: 'Roster not found' },
        },
        message: 'Request failed',
      }
      mockClient.post.mockRejectedValue(axiosError)

      await expect(validateRoster('nonexistent')).rejects.toMatchObject({
        status: 404,
        message: 'Roster not found',
      })
    })
  })

  describe('getValidationResults', () => {
    it('calls GET /roster/:id/validation and returns response', async () => {
      const mockResponse = {
        validationId: 'val-id',
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
        validatedAt: '2026-02-03T10:00:00Z',
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
            affectedDates: '2026-02-02',
          },
        ],
      }

      mockClient.get.mockResolvedValue(mockAxiosResponse(mockResponse))

      const result = await getValidationResults('test-roster-id')

      expect(mockClient.get).toHaveBeenCalledWith(
        '/roster/test-roster-id/validation',
      )
      expect(result).toEqual(mockResponse)
    })

    it('throws normalized error when no validation exists', async () => {
      const axiosError = {
        isAxiosError: true,
        response: {
          status: 404,
          data: { message: 'No validation found for this roster' },
        },
        message: 'Request failed',
      }
      mockClient.get.mockRejectedValue(axiosError)

      await expect(
        getValidationResults('no-validation'),
      ).rejects.toMatchObject({
        status: 404,
        message: 'No validation found for this roster',
      })
    })
  })
})

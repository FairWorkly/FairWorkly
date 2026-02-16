import httpClient from './httpClient'
import { normalizeApiError } from '@/shared/types/api.types'
import type {
  PayrollValidationRequest,
  PayrollValidationResult,
} from '@/modules/payroll/types'

/**
 * Upload a payroll CSV file for validation against award rules.
 * Sends multipart/form-data to POST /api/payroll/validation.
 *
 * @param file - Payroll CSV file
 * @param options - Validation parameters (awardType, state, enable flags)
 * @returns Validation result with summary, categories, and issues
 * @throws ApiError with normalized error structure
 */
export async function uploadPayrollValidation(
  file: File,
  options: PayrollValidationRequest
): Promise<PayrollValidationResult> {
  const formData = new FormData()
  formData.append('file', file)
  formData.append('awardType', options.awardType)
  formData.append('state', options.state)
  formData.append('enableBaseRateCheck', String(options.enableBaseRateCheck))
  formData.append('enablePenaltyCheck', String(options.enablePenaltyCheck))
  formData.append(
    'enableCasualLoadingCheck',
    String(options.enableCasualLoadingCheck)
  )
  formData.append('enableSuperCheck', String(options.enableSuperCheck))

  try {
    const response = await httpClient.post<PayrollValidationResult>(
      '/payroll/validation',
      formData,
      {
        headers: { 'Content-Type': undefined },
      }
    )
    return response.data
  } catch (err) {
    throw normalizeApiError(err)
  }
}

import { useMemo } from 'react'
import { useApiQuery } from '@/shared/hooks/useApiQuery'
import {
  getValidationResults,
  validateRoster,
  type ValidateRosterResponse,
} from '@/services/rosterApi'
import type { ApiError } from '@/shared/types/api.types'
import {
  mapValidationToComplianceResults,
  type RosterComplianceResults,
} from '../utils/mapValidationResponse'

/**
 * Try GET first (cheap read); fall back to POST (triggers validation) on 404.
 */
async function getOrValidate(
  rosterId: string
): Promise<ValidateRosterResponse> {
  try {
    return await getValidationResults(rosterId)
  } catch (err) {
    if ((err as ApiError).status === 404) {
      return validateRoster(rosterId)
    }
    throw err
  }
}

export function useValidateRoster(rosterId: string | undefined) {
  const query = useApiQuery<
    ValidateRosterResponse,
    readonly [string, string, string | undefined]
  >({
    queryKey: ['roster', 'validate', rosterId] as const,
    queryFn: () => getOrValidate(rosterId!),
    enabled: !!rosterId,
  })

  const complianceData: RosterComplianceResults | null = useMemo(
    () => (query.data ? mapValidationToComplianceResults(query.data) : null),
    [query.data]
  )

  const errorMessage = !rosterId
    ? 'No roster ID provided. Please upload a roster first.'
    : (query.error?.message ?? 'Validation failed. Please try again.')

  return {
    complianceData,
    validationId: query.data?.validationId ?? null,
    isLoading: query.isLoading,
    isError: query.isError || !rosterId,
    errorMessage,
  }
}

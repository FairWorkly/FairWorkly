import httpClient from './httpClient'
import type {
  ShiftScenario,
  DebateResult,
} from '@/modules/fairbot/types/debate.types'

export async function runDebate(
  scenario: ShiftScenario
): Promise<DebateResult> {
  // Backend proxies to agent service and returns { code, msg, data }.
  // setupInterceptors unwraps the envelope, so response.data is the inner
  // DebateResult directly (the agent service's data field).
  const response = await httpClient.post<DebateResult>(
    '/fairbot/debate',
    { scenario },
    { timeout: 120_000 }
  )
  return response.data
}

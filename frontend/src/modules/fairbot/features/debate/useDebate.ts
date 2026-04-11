import { useState } from 'react'
import { runDebate } from '@/services/debateApi'
import type { ShiftScenario, DebateResult } from '../../types/debate.types'

export const useDebate = () => {
  const [result, setResult] = useState<DebateResult | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const startDebate = async (scenario: ShiftScenario) => {
    setIsLoading(true)
    setError(null)
    setResult(null)
    try {
      const data = await runDebate(scenario)
      setResult(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred')
    } finally {
      setIsLoading(false)
    }
  }

  return { result, isLoading, error, startDebate }
}

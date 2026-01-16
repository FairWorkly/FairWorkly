import { useCallback, useEffect, useMemo, useState } from 'react'
import {
  FAIRBOT_ENV,
  FAIRBOT_SESSION_KEYS,
} from '../constants/fairbot.constants'
import type { FairBotResult } from '../types/fairbot.types'

// Persist and hydrate the latest results panel summary in session storage.
interface UseResultsPanelResult {
  currentResult: FairBotResult | null
  setCurrentResult: (result: FairBotResult | null) => void
  clearResult: () => void
}

const canUseSessionStorage = (): boolean =>
  typeof window !== FAIRBOT_ENV.TYPEOF_UNDEFINED && Boolean(window.sessionStorage)

const readResultFromSession = (): FairBotResult | null => {
  if (!canUseSessionStorage()) {
    return null
  }

  try {
    const stored = window.sessionStorage.getItem(FAIRBOT_SESSION_KEYS.RESULTS)
    if (!stored) {
      return null
    }

    return JSON.parse(stored) as FairBotResult
  } catch {
    return null
  }
}

const persistResultToSession = (result: FairBotResult | null) => {
  if (!canUseSessionStorage()) {
    return
  }

  try {
    if (!result) {
      window.sessionStorage.removeItem(FAIRBOT_SESSION_KEYS.RESULTS)
      return
    }

    window.sessionStorage.setItem(
      FAIRBOT_SESSION_KEYS.RESULTS,
      JSON.stringify(result),
    )
  } catch {
    return
  }
}

export const useResultsPanel = (): UseResultsPanelResult => {
  const [currentResult, setCurrentResult] = useState<FairBotResult | null>(
    readResultFromSession,
  )

  useEffect(() => {
    persistResultToSession(currentResult)
  }, [currentResult])

  const clearResult = useCallback(() => {
    setCurrentResult(null)
  }, [])

  return useMemo(
    () => ({
      currentResult,
      setCurrentResult,
      clearResult,
    }),
    [clearResult, currentResult],
  )
}

export interface FairBotRecentResultEntry {
  id: string
  kind: 'roster' | 'payroll'
  title: string
  subtitle?: string
  resultRoute: string
  fairbotRoute: string
  timestamp: string
}

export interface FairBotHistoryEntry {
  id: string
  question: string
  answer: string
  requestId?: string | null
  timestamp: string
}

const RECENT_RESULTS_KEY = 'fairbot_recent_results'
const HISTORY_KEY = 'fairbot_history'
const MAX_RECENT_RESULTS = 8
const MAX_HISTORY = 20

const safeParse = <T>(raw: string | null, fallback: T): T => {
  if (!raw) {
    return fallback
  }

  try {
    const parsed = JSON.parse(raw)
    return parsed as T
  } catch {
    return fallback
  }
}

const readArray = <T>(key: string): T[] => {
  if (typeof window === 'undefined') {
    return []
  }
  return safeParse<T[]>(window.localStorage.getItem(key), [])
}

const writeArray = <T>(key: string, data: T[]) => {
  if (typeof window === 'undefined') {
    return
  }
  try {
    window.localStorage.setItem(key, JSON.stringify(data))
  } catch {
    // Ignore quota/private-mode failures.
  }
}

export const readRecentResults = (): FairBotRecentResultEntry[] =>
  readArray<FairBotRecentResultEntry>(RECENT_RESULTS_KEY)

export const recordRecentResult = (entry: FairBotRecentResultEntry) => {
  const next = [entry, ...readRecentResults().filter(item => item.id !== entry.id)]
  writeArray(RECENT_RESULTS_KEY, next.slice(0, MAX_RECENT_RESULTS))
}

export const readHistoryEntries = (): FairBotHistoryEntry[] =>
  readArray<FairBotHistoryEntry>(HISTORY_KEY)

export const recordHistoryEntry = (entry: FairBotHistoryEntry) => {
  const next = [entry, ...readHistoryEntries().filter(item => item.id !== entry.id)]
  writeArray(HISTORY_KEY, next.slice(0, MAX_HISTORY))
}

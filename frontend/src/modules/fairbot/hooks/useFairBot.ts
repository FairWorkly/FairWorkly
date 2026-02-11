import { useCallback, useEffect, useMemo, useState } from 'react'
import {
  FAIRBOT_ENV,
  FAIRBOT_KEYWORDS,
  FAIRBOT_LABELS,
  FAIRBOT_MESSAGES,
  FAIRBOT_RESULTS,
  FAIRBOT_ROLES,
  FAIRBOT_ROUTES,
  FAIRBOT_SESSION_KEYS,
  FAIRBOT_MOCK_DATA,
} from '../constants/fairbot.constants'
import type {
  FairBotAgentResponse,
  FairBotConversationState,
  FairBotError,
  FairBotFileMeta,
  FairBotMessage,
  FairBotResult,
  FairBotResultType,
  PayrollSummaryData,
  RosterSummaryData,
  EmployeeSummaryData,
  DocumentSummaryData,
} from '../types/fairbot.types'
import { useResultsPanel } from './useResultsPanel'

// Manages FairBot conversation state, mock agent replies, and persistence.
interface UseFairBotResult extends FairBotConversationState {
  sendMessage: (text: string, file?: File) => Promise<void>
}

// Initial welcome message from assistant shown when conversation is empty.
const createWelcomeMessage = (): FairBotMessage => ({
  id: 'welcome',
  role: FAIRBOT_ROLES.ASSISTANT,
  text: FAIRBOT_LABELS.WELCOME_MESSAGE,
  timestamp: new Date().toISOString(),
})

const INITIAL_MESSAGES: FairBotMessage[] = [createWelcomeMessage()]

const canUseSessionStorage = (): boolean =>
  typeof window !== FAIRBOT_ENV.TYPEOF_UNDEFINED && Boolean(window.sessionStorage)

const readMessagesFromSession = (): FairBotMessage[] => {
  if (!canUseSessionStorage()) {
    return INITIAL_MESSAGES
  }

  try {
    const stored = window.sessionStorage.getItem(FAIRBOT_SESSION_KEYS.CONVERSATION)
    if (!stored) {
      return INITIAL_MESSAGES
    }

    const parsed = JSON.parse(stored) as unknown
    if (Array.isArray(parsed) && parsed.length > 0) {
      return parsed as FairBotMessage[]
    }
    return INITIAL_MESSAGES
  } catch {
    return INITIAL_MESSAGES
  }
}

const persistMessagesToSession = (messages: FairBotMessage[]) => {
  if (!canUseSessionStorage()) {
    return
  }

  try {
    // Strip File objects before persistence to keep session storage serializable.
    const serialized = messages.map((message) => ({
      ...message,
      file: undefined,
    }))
    window.sessionStorage.setItem(
      FAIRBOT_SESSION_KEYS.CONVERSATION,
      JSON.stringify(serialized),
    )
  } catch {
    return
  }
}

const createMessageId = (): string =>
  globalThis.crypto?.randomUUID?.() ?? String(Date.now())

const createFileMeta = (file: File): FairBotFileMeta => ({
  name: file.name,
  size: file.size,
  type: file.type,
})

const createMessage = (
  role: FairBotMessage['role'],
  text: string,
  file?: File,
): FairBotMessage => ({
  id: createMessageId(),
  role,
  text,
  timestamp: new Date().toISOString(),
  file,
  fileMeta: file ? createFileMeta(file) : undefined,
})

const createError = (error: unknown): FairBotError => {
  if (error instanceof Error && error.message) {
    return { message: error.message }
  }

  return { message: FAIRBOT_LABELS.ERROR_GENERIC }
}

const getResultTypeFromText = (text: string): FairBotResultType | null => {
  const normalized = text.toLowerCase()

  if (normalized.includes(FAIRBOT_KEYWORDS.PAYROLL)) {
    return FAIRBOT_RESULTS.TYPES.PAYROLL
  }

  if (normalized.includes(FAIRBOT_KEYWORDS.ROSTER)) {
    return FAIRBOT_RESULTS.TYPES.ROSTER
  }

  if (normalized.includes(FAIRBOT_KEYWORDS.EMPLOYEE)) {
    return FAIRBOT_RESULTS.TYPES.EMPLOYEE
  }

  if (
    normalized.includes(FAIRBOT_KEYWORDS.DOCUMENT) ||
    normalized.includes(FAIRBOT_KEYWORDS.CONTRACT)
  ) {
    return FAIRBOT_RESULTS.TYPES.DOCUMENT
  }

  return null
}

const buildQuickSummary = (resultType: FairBotResultType): FairBotResult => {
  switch (resultType) {
    case FAIRBOT_RESULTS.TYPES.PAYROLL:
      return {
        type: FAIRBOT_RESULTS.TYPES.PAYROLL,
        data: FAIRBOT_MOCK_DATA.PAYROLL as unknown as PayrollSummaryData,
        detailsUrl: FAIRBOT_ROUTES.PAYROLL,
      }
    case FAIRBOT_RESULTS.TYPES.ROSTER:
      return {
        type: FAIRBOT_RESULTS.TYPES.ROSTER,
        data: FAIRBOT_MOCK_DATA.ROSTER as unknown as RosterSummaryData,
        detailsUrl: FAIRBOT_ROUTES.ROSTER,
      }
    case FAIRBOT_RESULTS.TYPES.EMPLOYEE:
      return {
        type: FAIRBOT_RESULTS.TYPES.EMPLOYEE,
        data: FAIRBOT_MOCK_DATA.EMPLOYEE as unknown as EmployeeSummaryData,
        detailsUrl: FAIRBOT_ROUTES.EMPLOYEE,
      }
    case FAIRBOT_RESULTS.TYPES.DOCUMENT:
      return {
        type: FAIRBOT_RESULTS.TYPES.DOCUMENT,
        data: FAIRBOT_MOCK_DATA.DOCUMENT as unknown as DocumentSummaryData,
        detailsUrl: FAIRBOT_ROUTES.DOCUMENT,
      }
  }
}

const buildMockResponse = (text: string, file?: File): FairBotAgentResponse => {
  // Mock response for UI scaffolding; replace with agent-service integration.
  const detectedType =
    getResultTypeFromText(text) ?? (file ? getResultTypeFromText(file.name) : null)

  return {
    textResponse: file
      ? FAIRBOT_MESSAGES.ASSISTANT_FILE_RECEIVED
      : FAIRBOT_MESSAGES.ASSISTANT_DEFAULT,
    quickSummary: detectedType ? buildQuickSummary(detectedType) : undefined,
  }
}

export const useFairBot = (): UseFairBotResult => {
  const { setCurrentResult } = useResultsPanel()

  const [messages, setMessages] = useState<FairBotMessage[]>(
    readMessagesFromSession,
  )
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<FairBotError | null>(null)

  useEffect(() => {
    persistMessagesToSession(messages)
  }, [messages])

  const sendMessage = useCallback(
    async (text: string, file?: File) => {
      const trimmedText = text.trim()
      const hasContent = Boolean(trimmedText) || Boolean(file)

      if (!hasContent) {
        return
      }

      setError(null)

      const userMessage = createMessage(
        FAIRBOT_ROLES.USER,
        trimmedText,
        file,
      )
      setMessages((prev) => [...prev, userMessage])
      setIsLoading(true)

      try {
        await Promise.resolve()
        const response = buildMockResponse(trimmedText, file)
        const assistantMessage = createMessage(
          FAIRBOT_ROLES.ASSISTANT,
          response.textResponse,
        )

        setMessages((prev) => [...prev, assistantMessage])

        if (response.quickSummary) {
          setCurrentResult(response.quickSummary)
        }
      } catch (caughtError) {
        setError(createError(caughtError))
      } finally {
        setIsLoading(false)
      }
    },
    [setCurrentResult],
  )

  return useMemo(
    () => ({
      messages,
      sendMessage,
      isLoading,
      error,
    }),
    [messages, sendMessage, isLoading, error],
  )
}

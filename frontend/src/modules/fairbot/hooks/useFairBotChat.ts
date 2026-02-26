import { useCallback, useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { FAIRBOT_ROLES } from '../constants/fairbot.constants'
import { FAIRBOT_TIMEOUT_SECONDS, sendChatMessage } from '@/services/fairbotApi'
import { useAuth } from '@/modules/auth/hooks/useAuth'
import { getValidationResults } from '@/services/rosterApi'
import type {
  FairBotConversationState,
  FairBotError,
  FairBotMessage,
  FairBotMessageMetadata,
} from '../types/fairbot.types'

interface UseFairBotChatResult extends FairBotConversationState {
  sendMessage: (text: string) => Promise<void>
  contextLabel: string | null
  isContextLoading: boolean
}

const createWelcomeMessage = (): FairBotMessage => ({
  id: 'welcome',
  role: FAIRBOT_ROLES.ASSISTANT,
  text: "Hi! I'm FairBot, your AI-powered Fair Work assistant.",
  timestamp: new Date().toISOString(),
})

const createInitialMessages = (): FairBotMessage[] => [createWelcomeMessage()]

const createMessageId = (): string =>
  globalThis.crypto?.randomUUID?.() ?? String(Date.now())

const createMessage = (
  role: FairBotMessage['role'],
  text: string,
  metadata?: FairBotMessageMetadata
): FairBotMessage => ({
  id: createMessageId(),
  role,
  text,
  timestamp: new Date().toISOString(),
  metadata,
})

interface FairBotTransportError {
  code?: string
  status?: number
  message?: string
  requestId?: string
}

const createTimeoutFallbackMessage = (requestId?: string): string => {
  const requestIdSuffix = requestId ? ` Request ID: ${requestId}.` : ''
  return `This analysis timed out (${FAIRBOT_TIMEOUT_SECONDS}s), so we couldn't get a complete result. Please try again. If it still fails, narrow your question or contact support with the Request ID.${requestIdSuffix}`
}

const withRequestId = (baseMessage: string, requestId?: string): string => {
  if (!requestId) {
    return baseMessage
  }
  return `${baseMessage} Request ID: ${requestId}.`
}

const isTimeoutError = (error: FairBotTransportError): boolean => {
  if (error.code === 'ECONNABORTED') {
    return true
  }
  if (error.status === 504) {
    return true
  }

  const message = (error.message ?? '').toLowerCase()
  return message.includes('timeout') || message.includes('timed out')
}

const createError = (error: unknown): FairBotError => {
  if (typeof error === 'object' && error !== null) {
    const typed = error as FairBotTransportError
    if (isTimeoutError(typed)) {
      return { message: createTimeoutFallbackMessage(typed.requestId) }
    }

    if (typed.status === 401) {
      return {
        message:
          'Your session expired or you do not have permission to use FairBot. Please sign in again.',
      }
    }

    if (typed.status === 413) {
      return {
        message:
          'Your request is too large. Please shorten your message and try again.',
      }
    }

    if (typed.status === 429) {
      return {
        message:
          'FairBot is handling too many requests right now. Please wait a moment and try again.',
      }
    }

    if (typeof typed.status === 'number' && typed.status >= 500) {
      return {
        message: withRequestId(
          "FairBot couldn't complete your request due to a service error. Please try again.",
          typed.requestId
        ),
      }
    }
  }

  if (error instanceof Error && error.message) {
    return {
      message: error.message,
    }
  }

  return { message: 'Something went wrong. Please try again.' }
}

const toAssistantMetadata = (
  model?: string,
  note?: string | null,
  sources?: Array<{ source: string; page: number; content: string }>
): FairBotMessageMetadata | undefined => {
  const normalizedSources = (sources ?? [])
    .filter(item => Boolean(item?.source))
    .map(item => ({
      source: item.source,
      page: item.page,
      content: item.content,
    }))

  const hasData =
    Boolean(model) || Boolean(note) || normalizedSources.length > 0
  if (!hasData) {
    return undefined
  }

  return {
    model,
    note: note ?? null,
    sources: normalizedSources,
  }
}

interface RosterExplainContext {
  intentHint: 'roster' | 'roster_explain'
  rosterId: string
  payload: Record<string, unknown>
}

export const useFairBotChat = (): UseFairBotChatResult => {
  const { user } = useAuth()
  const [searchParams] = useSearchParams()
  const [messages, setMessages] = useState<FairBotMessage[]>(
    createInitialMessages
  )
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<FairBotError | null>(null)
  const [context, setContext] = useState<RosterExplainContext | null>(null)
  const [isContextLoading, setIsContextLoading] = useState(false)

  const intent = searchParams.get('intent')
  const rosterId = searchParams.get('rosterId')
  const validationId = searchParams.get('validationId')
  const isRosterExplainMode = intent === 'roster'

  // Cleanup legacy persisted chat so users always start fresh.
  useEffect(() => {
    if (typeof window !== 'undefined') {
      window.sessionStorage.removeItem('fairbot_conversation')
    }
  }, [])

  // Each entry/task starts a fresh conversation to avoid stale context bleed.
  useEffect(() => {
    setMessages(createInitialMessages())
    setError(null)
  }, [intent, rosterId, validationId])

  // Defensive reset on account change (logout/switch user).
  useEffect(() => {
    setMessages(createInitialMessages())
    setError(null)
  }, [user?.id])

  // Load roster validation context for explain mode.
  // Prefer full validation payload (works even when backend resolver is stale).
  // Fall back to reference payload for compatibility.
  useEffect(() => {
    if (!isRosterExplainMode || !rosterId) {
      setContext(null)
      setIsContextLoading(false)
      return
    }

    let cancelled = false
    setIsContextLoading(true)

    void (async () => {
      try {
        const validation = await getValidationResults(rosterId)
        if (cancelled) {
          return
        }

        setContext({
          intentHint: 'roster_explain',
          rosterId,
          payload: {
            kind: 'roster_validation',
            rosterId,
            validation,
          },
        })
      } catch {
        if (cancelled) {
          return
        }

        setContext({
          intentHint: 'roster',
          rosterId,
          payload: {
            kind: 'roster_reference',
            rosterId,
            validationId,
          },
        })
      } finally {
        if (!cancelled) {
          setIsContextLoading(false)
        }
      }
    })()

    return () => {
      cancelled = true
    }
  }, [isRosterExplainMode, rosterId, validationId, user?.id])

  const sendMessage = useCallback(
    async (text: string) => {
      const trimmedText = text.trim()
      if (!trimmedText) {
        return
      }

      setError(null)

      const userMessage = createMessage(FAIRBOT_ROLES.USER, trimmedText)
      setMessages(prev => [...prev, userMessage])
      setIsLoading(true)

      try {
        // Attach roster context when available; otherwise plain compliance Q&A.
        let response = await sendChatMessage(
          trimmedText,
          context
            ? {
                intentHint: context.intentHint,
                contextPayload: context.payload,
              }
            : undefined
        )

        // Safety net: if explain context is missing upstream, retry once as general Q&A.
        if (response.result?.note === 'ROSTER_CONTEXT_REQUIRED') {
          response = await sendChatMessage(trimmedText, {
            intentHint: 'compliance',
          })
        }

        const replyText =
          response.result?.message || 'Sorry, I could not process your request.'
        const assistantMessage = createMessage(
          FAIRBOT_ROLES.ASSISTANT,
          replyText,
          toAssistantMetadata(
            response.result?.model,
            response.result?.note,
            response.result?.sources
          )
        )

        setMessages(prev => [...prev, assistantMessage])
      } catch (caughtError) {
        const normalized = createError(caughtError)
        setError(normalized)
        throw new Error(normalized.message)
      } finally {
        setIsLoading(false)
      }
    },
    [context]
  )

  return useMemo(
    () => ({
      messages,
      sendMessage,
      isLoading,
      error,
      contextLabel: isRosterExplainMode
        ? isContextLoading
          ? 'Loading roster context...'
          : context
            ? 'Roster context loaded'
            : 'Roster context unavailable'
        : null,
      isContextLoading,
    }),
    [
      messages,
      sendMessage,
      isLoading,
      error,
      context,
      isRosterExplainMode,
      isContextLoading,
    ]
  )
}

import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { FAIRBOT_ROLES } from '../constants/fairbot.constants'
import {
  sendChatMessage,
  type AgentChatResponse,
  type AgentChatHistoryItem,
} from '@/services/fairbotApi'
import { useAuth } from '@/modules/auth/hooks/useAuth'
import { getValidationResults } from '@/services/rosterApi'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { useApiQuery } from '@/shared/hooks/useApiQuery'
import type {
  FairBotConversationState,
  FairBotError,
  FairBotMessage,
  FairBotMessageMetadata,
} from '../types/fairbot.types'
import {
  createChatError,
  toHistoryPayload,
  toAssistantMetadata,
} from '../utils'

interface UseFairBotChatResult extends FairBotConversationState {
  sendMessage: (text: string) => Promise<boolean>
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
  globalThis.crypto?.randomUUID?.() ??
  `msg-${Date.now()}-${Math.random().toString(36).slice(2)}`

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

interface RosterExplainContext {
  intentHint: 'roster' | 'roster_explain'
  rosterId: string
  payload: Record<string, unknown>
}

const createConversationId = (): string =>
  globalThis.crypto?.randomUUID?.() ??
  `conv-${Date.now()}-${Math.random().toString(36).slice(2)}`

const loadRosterExplainContext = async (
  rosterId: string,
  validationId: string | null
): Promise<RosterExplainContext> => {
  try {
    const validation = await getValidationResults(rosterId)
    return {
      intentHint: 'roster_explain',
      rosterId,
      payload: {
        kind: 'roster_validation',
        rosterId,
        validation,
      },
    }
  } catch {
    return {
      intentHint: 'roster',
      rosterId,
      payload: {
        kind: 'roster_reference',
        rosterId,
        validationId,
      },
    }
  }
}

interface SendChatMutationVariables {
  text: string
  context: RosterExplainContext | null
  historyPayload: AgentChatHistoryItem[]
  conversationId: string
}

export const useFairBotChat = (): UseFairBotChatResult => {
  const { user } = useAuth()
  const [searchParams] = useSearchParams()
  const [messages, setMessages] = useState<FairBotMessage[]>(
    createInitialMessages
  )
  const [error, setError] = useState<FairBotError | null>(null)
  const runtimeRef = useRef<{
    abortController: AbortController | null
    conversationId: string
  }>({
    abortController: null,
    conversationId: createConversationId(),
  })

  const intent = searchParams.get('intent')
  const rosterId = searchParams.get('rosterId')
  const validationId = searchParams.get('validationId')
  const isRosterExplainMode = intent === 'roster'
  const rosterContextQuery = useApiQuery<
    RosterExplainContext,
    readonly [
      'fairbot',
      'roster-context',
      string | undefined,
      string | null,
      string | null
    ]
  >({
    queryKey: [
      'fairbot',
      'roster-context',
      user?.id,
      rosterId,
      validationId,
    ] as const,
    queryFn: () => loadRosterExplainContext(rosterId!, validationId),
    enabled: isRosterExplainMode && Boolean(rosterId),
  })
  const context = rosterContextQuery.data ?? null
  const isContextLoading =
    isRosterExplainMode &&
    Boolean(rosterId) &&
    (rosterContextQuery.isLoading || rosterContextQuery.isFetching)
  const sendChatMutation = useApiMutation<
    AgentChatResponse,
    SendChatMutationVariables
  >({
    mutationFn: async ({
      text,
      context: chatContext,
      historyPayload,
      conversationId,
    }) => {
      // Attach roster context when available; otherwise plain compliance Q&A.
      let response = await sendChatMessage(
        text,
        chatContext
          ? {
              intentHint: chatContext.intentHint,
              contextPayload: chatContext.payload,
              historyPayload,
              conversationId,
            }
          : {
              historyPayload,
              conversationId,
            }
      )

      // Safety net: if explain context is missing upstream, retry once as general Q&A.
      if (response.result?.note === 'ROSTER_CONTEXT_REQUIRED') {
        response = await sendChatMessage(text, {
          intentHint: 'compliance',
          historyPayload,
          conversationId,
        })
      }

      return response
    },
  })
  const { mutateAsync: sendChat, isPending: isSending } = sendChatMutation

  // Cleanup legacy persisted chat so users always start fresh.
  useEffect(() => {
    if (typeof window !== 'undefined') {
      window.sessionStorage.removeItem('fairbot_conversation')
    }
  }, [])

  // Each entry/task starts a fresh conversation to avoid stale context bleed.
  // Also resets on account change (logout/switch user).
  useEffect(() => {
    runtimeRef.current.abortController?.abort()
    runtimeRef.current.abortController = null
    setMessages(createInitialMessages())
    setError(null)
    runtimeRef.current.conversationId = createConversationId()
  }, [intent, rosterId, validationId, user?.id])

  // Abort in-flight chat request on unmount.
  useEffect(() => {
    const runtime = runtimeRef.current
    return () => {
      runtime.abortController?.abort()
    }
  }, [])

  const sendMessage = useCallback(
    async (text: string): Promise<boolean> => {
      const trimmedText = text.trim()
      if (!trimmedText) {
        return true
      }

      setError(null)

      const historyPayload = toHistoryPayload(messages)
      const userMessage = createMessage(FAIRBOT_ROLES.USER, trimmedText)
      setMessages(prev => [...prev, userMessage])

      runtimeRef.current.abortController?.abort()
      const controller = new AbortController()
      runtimeRef.current.abortController = controller

      try {
        const response = await sendChat({
          text: trimmedText,
          context,
          historyPayload,
          conversationId: runtimeRef.current.conversationId,
        })

        if (controller.signal.aborted) {
          return false
        }

        const replyText =
          response.result?.message || 'Sorry, I could not process your request.'
        const assistantMessage = createMessage(
          FAIRBOT_ROLES.ASSISTANT,
          replyText,
          toAssistantMetadata(
            response.result?.model,
            response.result?.note,
            response.result?.sources,
            response.result?.data?.action_plan
          )
        )

        setMessages(prev => [...prev, assistantMessage])
        return true
      } catch (caughtError) {
        if (controller.signal.aborted) {
          return false
        }
        const normalized = createChatError(caughtError)
        setError(normalized)
        return false
      } finally {
        if (runtimeRef.current.abortController === controller) {
          runtimeRef.current.abortController = null
        }
      }
    },
    [context, messages, sendChat]
  )

  return useMemo(
    () => ({
      messages,
      sendMessage,
      isLoading: isSending,
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
      isSending,
      error,
      context,
      isRosterExplainMode,
      isContextLoading,
    ]
  )
}

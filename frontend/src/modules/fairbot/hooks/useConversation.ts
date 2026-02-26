import { useMemo } from 'react'
import { useFairBotChat } from './useFairBotChat'
import { useMessageStream } from './useMessageStream'
import type { FairBotMessage } from '../types/fairbot.types'

interface UseConversationResult {
  messages: FairBotMessage[]
  isLoading: boolean
  isContextLoading: boolean
  isTyping: boolean
  errorMessage: string | null
  contextLabel: string | null
  hasMessages: boolean
  sendMessage: (text: string) => Promise<void>
}

export const useConversation = (): UseConversationResult => {
  const {
    messages,
    sendMessage,
    isLoading,
    error,
    contextLabel,
    isContextLoading,
  } = useFairBotChat()
  const { isTyping } = useMessageStream(isLoading)

  return useMemo(
    () => ({
      messages,
      isLoading,
      isContextLoading,
      isTyping,
      errorMessage: error?.message ?? null,
      contextLabel,
      hasMessages: messages.length > 0,
      sendMessage,
    }),
    [
      error?.message,
      isLoading,
      isContextLoading,
      isTyping,
      messages,
      sendMessage,
      contextLabel,
    ]
  )
}

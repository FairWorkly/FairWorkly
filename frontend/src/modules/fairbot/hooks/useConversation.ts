import { useMemo } from 'react'
import { useMessageStream } from '@/shared/chat'
import type { ChatMessage } from '@/shared/chat'
import { useFairBot } from './useFairBot'

// View-model hook that combines conversation state with typing indicator timing.
interface UseConversationResult {
  messages: ChatMessage[]
  isLoading: boolean
  isTyping: boolean
  errorMessage: string | null
  hasMessages: boolean
  sendMessage: (text: string, file?: File) => Promise<void>
}

export const useConversation = (): UseConversationResult => {
  const { messages, sendMessage, isLoading, error } = useFairBot()
  const { isTyping } = useMessageStream(isLoading)

  return useMemo(
    () => ({
      messages,
      isLoading,
      isTyping,
      errorMessage: error?.message ?? null,
      hasMessages: messages.length > 0,
      sendMessage,
    }),
    [error?.message, isLoading, isTyping, messages, sendMessage],
  )
}

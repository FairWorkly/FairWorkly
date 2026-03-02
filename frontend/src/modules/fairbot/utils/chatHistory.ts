import type { AgentChatHistoryItem } from '@/services/fairbotApi'
import type { FairBotMessage } from '../types/fairbot.types'

const MAX_HISTORY_MESSAGES = 12
const MAX_HISTORY_CONTENT_CHARS = 1_200

export const toHistoryPayload = (
  messageList: FairBotMessage[]
): AgentChatHistoryItem[] =>
  messageList
    .filter(message => message.id !== 'welcome')
    .map(message => ({
      role: message.role,
      content: message.text.trim().slice(0, MAX_HISTORY_CONTENT_CHARS),
    }))
    .filter(message => message.content.length > 0)
    .slice(-MAX_HISTORY_MESSAGES)

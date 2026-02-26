// Chat participant roles used to render message styling and labels.
export type FairBotMessageRole = 'user' | 'assistant'

export interface FairBotMessageSource {
  source: string
  page?: number
  content?: string
}

export interface FairBotMessageMetadata {
  model?: string
  note?: string | null
  sources?: FairBotMessageSource[]
}

// Chat message model used across UI and session storage.
export interface FairBotMessage {
  id: string
  role: FairBotMessageRole
  text: string
  timestamp: string
  metadata?: FairBotMessageMetadata
}

// Standard error shape used by FairBot hooks.
export interface FairBotError {
  message: string
  code?: string
  status?: number
  requestId?: string
}

export interface FairBotTraceInfo {
  requestId: string | null
  status: 'success' | 'error'
  note?: string | null
  timestamp: string
}

// Conversation state owned by the main FairBot hook.
export interface FairBotConversationState {
  messages: FairBotMessage[]
  isLoading: boolean
  error: FairBotError | null
}

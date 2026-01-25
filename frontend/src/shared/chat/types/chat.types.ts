// Chat participant roles used to render message styling and labels.
export type ChatMessageRole = 'user' | 'assistant'

// Serializable metadata for uploaded files (stored alongside messages).
export interface ChatFileMeta {
  name: string
  size: number
  type: string
}

// Chat message model used across UI and session storage.
export interface ChatMessage {
  id: string
  role: ChatMessageRole
  text: string
  timestamp: string
  // Raw file object (not persisted to session storage).
  file?: File
  // Persisted metadata for file display in the chat.
  fileMeta?: ChatFileMeta
}

// Standard error shape used by chat hooks.
export interface ChatError {
  message: string
  code?: string
}

// Conversation state owned by the main chat hook.
export interface ChatConversationState {
  messages: ChatMessage[]
  isLoading: boolean
  error: ChatError | null
}

// Upload state used by the file drop zone and controls.
export interface ChatUploadState {
  isDragging: boolean
  isUploading: boolean
  error: ChatError | null
  acceptedFileTypes: string[]
}

// Configuration for chat UI customization.
export interface ChatConfig {
  title: string
  subtitle: string
  welcomeMessage: string
  inputPlaceholder: string
  userLabel: string
  assistantLabel: string
  loadingMessage: string
  emptyResultsTitle: string
  emptyResultsSubtitle: string
}

// File upload configuration.
export interface ChatFileConfig {
  acceptedTypes: string[]
  acceptedMime: string[]
  maxSizeBytes: number
  maxSizeLabel: string
  acceptAttribute: string
}

// Layout configuration for chat UI.
export interface ChatLayoutConfig {
  chatHeaderHeightPx: number
  resultsPanelWidth: number
  mobileBreakpoint: number
  gridTemplateColumns: string
}

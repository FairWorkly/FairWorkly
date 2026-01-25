// Components
export { MessageBubble } from './components/MessageBubble'
export type { MessageBubbleProps, MessageBubbleLabels } from './components/MessageBubble'

export { MessageList } from './components/MessageList'
export type { MessageListProps, MessageListLabels } from './components/MessageList'

export { MessageInput } from './components/MessageInput'
export type {
  MessageInputProps,
  MessageInputLabels,
  MessageInputControls,
} from './components/MessageInput'

export { TypingIndicator } from './components/TypingIndicator'
export type { TypingIndicatorProps } from './components/TypingIndicator'

export { FileUploadZone } from './components/FileUploadZone'
export type {
  FileUploadZoneProps,
  FileUploadControls as FileUploadZoneControls,
} from './components/FileUploadZone'

// Hooks
export { useMessageStream } from './hooks/useMessageStream'
export { useFileUpload } from './hooks/useFileUpload'
export type {
  UseFileUploadOptions,
  UseFileUploadResult,
  FileUploadControls,
  FileUploadErrorMessages,
} from './hooks/useFileUpload'

// Types
export type {
  ChatMessage,
  ChatMessageRole,
  ChatFileMeta,
  ChatError,
  ChatConversationState,
  ChatUploadState,
  ChatConfig,
  ChatFileConfig,
  ChatLayoutConfig,
} from './types/chat.types'

// Utils
export {
  formatTimestamp,
  formatFileSize,
  createMessageId,
} from './utils/formatters'

// Constants
export {
  CHAT_ROLES,
  CHAT_TEXT,
  CHAT_ENV,
  CHAT_NUMBERS,
  CHAT_TIME_FORMAT,
  CHAT_FILE_SIZE,
  CHAT_TIMING,
  CHAT_TYPING_UI,
  CHAT_MESSAGE_UI,
  CHAT_INPUT_UI,
  CHAT_UPLOAD_UI,
  CHAT_ARIA,
  CHAT_DEFAULT_LABELS,
  CHAT_INPUT_TYPES,
} from './constants/chat.constants'

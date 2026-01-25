// Common role identifiers for chat messages.
export const CHAT_ROLES = {
  USER: 'user',
  ASSISTANT: 'assistant',
} as const

// Common text constants.
export const CHAT_TEXT = {
  EMPTY: '',
} as const

// Environment detection constants.
export const CHAT_ENV = {
  TYPEOF_UNDEFINED: 'undefined',
} as const

// Common numeric constants.
export const CHAT_NUMBERS = {
  ZERO: 0,
  ONE: 1,
} as const

// Time format constants for timestamps.
export const CHAT_TIME_FORMAT = {
  HOUR: '2-digit',
  MINUTE: '2-digit',
} as const

// File size formatting constants.
export const CHAT_FILE_SIZE = {
  KILO_THRESHOLD: 1024,
  MEGA_THRESHOLD: 1024 * 1024,
  MEGA_DECIMALS: 1,
  KILO_SUFFIX: 'KB',
  MEGA_SUFFIX: 'MB',
  ZERO_KB: '0 KB',
} as const

// Timing constants.
export const CHAT_TIMING = {
  TYPING_INDICATOR_DELAY_MS: 200,
} as const

// Typing indicator UI constants.
export const CHAT_TYPING_UI = {
  DOT_SIZE: 6,
  DOT_GAP: 4,
  DOT_ANIMATION_MS: 1200,
  DELAY_SHORT_MS: 120,
  DELAY_LONG_MS: 240,
} as const

// Message UI constants.
export const CHAT_MESSAGE_UI = {
  BUBBLE_MAX_WIDTH: 520,
} as const

// Input UI constants.
export const CHAT_INPUT_UI = {
  BUTTON_SIZE: 44,
} as const

// File upload zone UI constants.
export const CHAT_UPLOAD_UI = {
  BORDER_NONE: 'none',
  HELPER_TEXT_OFFSET_PX: 10,
  TRANSITION_MS: 150,
  MIN_HEIGHT: 48,
  TRANSITION_EASING: 'ease',
  TRANSITION_PROPERTIES: 'background-color',
} as const

// ARIA labels for accessibility.
export const CHAT_ARIA = {
  CHAT_AREA: 'Chat area',
  MESSAGE_LIST: 'Conversation',
  MESSAGE_INPUT: 'Message input',
  FILE_UPLOAD: 'File upload',
} as const

// Default labels (can be overridden via config).
export const CHAT_DEFAULT_LABELS = {
  USER_LABEL: 'You',
  ASSISTANT_LABEL: 'Assistant',
  MESSAGE_TIME_PREFIX: 'Sent',
  SEND_BUTTON_LABEL: 'Send message',
  ATTACH_BUTTON_LABEL: 'Attach file',
  MESSAGE_INPUT_LABEL: 'Message',
  INPUT_PLACEHOLDER: 'Type your message...',
  LOADING_MESSAGE: 'Thinking...',
  ATTACHMENT_LABEL: 'Attachment',
} as const

// Input type constants.
export const CHAT_INPUT_TYPES = {
  FILE: 'file',
  BUTTON: 'button',
  SUBMIT: 'submit',
} as const

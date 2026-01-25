import {
  CHAT_FILE_SIZE,
  CHAT_NUMBERS,
  CHAT_TIME_FORMAT,
} from '../constants/chat.constants'

// Format message timestamps for display, falling back to raw input if invalid.
export const formatTimestamp = (timestamp: string): string => {
  const date = new Date(timestamp)
  if (Number.isNaN(date.getTime())) {
    return timestamp
  }

  return date.toLocaleTimeString([], {
    hour: CHAT_TIME_FORMAT.HOUR,
    minute: CHAT_TIME_FORMAT.MINUTE,
  })
}

// Format file size in bytes to human-readable string.
export const formatFileSize = (bytes: number): string => {
  if (!Number.isFinite(bytes) || bytes <= CHAT_NUMBERS.ZERO) {
    return CHAT_FILE_SIZE.ZERO_KB
  }

  if (bytes >= CHAT_FILE_SIZE.MEGA_THRESHOLD) {
    return `${(bytes / CHAT_FILE_SIZE.MEGA_THRESHOLD).toFixed(
      CHAT_FILE_SIZE.MEGA_DECIMALS,
    )} ${CHAT_FILE_SIZE.MEGA_SUFFIX}`
  }

  return `${Math.ceil(bytes / CHAT_FILE_SIZE.KILO_THRESHOLD)} ${
    CHAT_FILE_SIZE.KILO_SUFFIX
  }`
}

// Generate a unique message ID.
export const createMessageId = (): string =>
  globalThis.crypto?.randomUUID?.() ?? String(Date.now())

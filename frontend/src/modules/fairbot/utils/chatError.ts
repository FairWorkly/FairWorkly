import { FAIRBOT_TIMEOUT_SECONDS } from '@/services/fairbotApi'
import type { FairBotError } from '../types/fairbot.types'

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

export const createChatError = (error: unknown): FairBotError => {
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

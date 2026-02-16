import type { AxiosError } from 'axios'

// --- Unified API error model ---

/**
 * Normalized error for UI / hooks consumption.
 * Abstracts away Axios internals and backend response formats.
 */
export interface ApiError {
  status?: number
  message: string
  details?: unknown // envelope inner data (e.g. { errors: [...] })
  raw?: unknown
}

// --- Backend response format detection ---

/**
 * Backend unified response envelope returned by Result<T> + ResultMappingFilter.
 * All intentional responses (2xx, 4xx, 5xx via Handler) use this format.
 */
export interface BackendEnvelope<T = unknown> {
  code: number
  msg: string
  data?: T
}

/**
 * Type guard: checks if data matches the { code, msg } envelope shape.
 * Used by both the success interceptor (unwrapping) and normalizeApiError.
 */
export function isBackendEnvelope(data: unknown): data is BackendEnvelope {
  return (
    typeof data === 'object' && data !== null && 'code' in data && 'msg' in data
  )
}

/**
 * RFC 7807 ProblemDetails — only produced by GlobalExceptionHandler
 * for truly unexpected exceptions (bugs). Frontend shows a generic message.
 */
interface ProblemDetails {
  status?: number
  title?: string
  detail?: string
  instance?: string
}

function isProblemDetails(data: unknown): data is ProblemDetails {
  return typeof data === 'object' && data !== null && 'title' in data
}

// --- Axios type guard ---

export function isAxiosError(error: unknown): error is AxiosError {
  return typeof error === 'object' && error !== null && 'isAxiosError' in error
}

// --- Error normalizer ---

/**
 * Converts any thrown error into a normalized ApiError.
 *
 * Handles three backend response formats:
 * 1. Envelope { code, msg, data } — normal business responses via Result<T>
 * 2. ProblemDetails { status, title, detail } — GlobalExceptionHandler (bugs)
 * 3. Unknown format — falls back to Axios error message
 */
export function normalizeApiError(error: unknown): ApiError {
  if (isAxiosError(error)) {
    const status = error.response?.status
    const data = error.response?.data

    // Backend envelope: { code, msg, data }
    if (isBackendEnvelope(data)) {
      return {
        status,
        message: data.msg,
        details: data.data,
        raw: error,
      }
    }

    // ProblemDetails (GlobalExceptionHandler — unexpected bugs)
    if (isProblemDetails(data)) {
      return {
        status,
        message: data.detail || data.title || 'Server error',
        raw: error,
      }
    }

    // Unknown format
    return {
      status,
      message: error.message || 'Request failed. Please try again.',
      raw: error,
    }
  }

  // Native Error
  if (error instanceof Error) {
    return { message: error.message || 'Unexpected error.', raw: error }
  }

  // Catch-all
  return { message: 'Network error. Please try again.', raw: error }
}

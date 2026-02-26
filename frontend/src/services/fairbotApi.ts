import { isAxiosError } from 'axios'
import httpClient from './httpClient'

export interface AgentChatSource {
  source: string
  page: number
  content: string
}

export interface AgentChatResult {
  type: string
  message: string
  file_name: string | null
  model?: string
  sources?: AgentChatSource[]
  note?: string | null
}

export interface AgentChatResponse {
  status: string
  message: string
  file_name: string | null
  routed_to: string
  request_id?: string
  result: AgentChatResult
}

export interface SendChatMessageOptions {
  intentHint?:
    | 'roster'
    | 'roster_explain'
    | 'payroll'
    | 'payroll_explain'
    | 'compliance'
  contextPayload?: unknown
}

const parseFairBotTimeoutMs = (): number => {
  const raw = import.meta.env.VITE_FAIRBOT_TIMEOUT_MS
  const parsed = Number(raw)
  if (!Number.isFinite(parsed) || parsed <= 0) {
    return 120000
  }
  return parsed
}

export const FAIRBOT_TIMEOUT_MS = parseFairBotTimeoutMs()
export const FAIRBOT_TIMEOUT_SECONDS = Math.round(FAIRBOT_TIMEOUT_MS / 1000)

const createRequestId = (): string =>
  globalThis.crypto?.randomUUID?.() ??
  `fairbot-${Date.now()}-${Math.random().toString(16).slice(2)}`

const readRequestIdFromHeaders = (headers: unknown): string | null => {
  if (!headers || typeof headers !== 'object') {
    return null
  }
  const source = headers as Record<string, unknown>
  const raw = source['x-request-id'] ?? source['X-Request-Id']

  if (typeof raw === 'string' && raw.trim()) {
    return raw.trim()
  }
  if (Array.isArray(raw) && raw.length > 0 && typeof raw[0] === 'string') {
    return raw[0].trim()
  }

  return null
}

export async function sendChatMessage(
  message: string,
  options?: SendChatMessageOptions
): Promise<AgentChatResponse> {
  const requestId = createRequestId()
  const startedAt = globalThis.performance?.now?.() ?? Date.now()
  const formData = new FormData()
  formData.append('message', message)
  if (options?.intentHint) {
    formData.append('intentHint', options.intentHint)
  }
  if (options?.contextPayload) {
    formData.append('contextPayload', JSON.stringify(options.contextPayload))
  }

  try {
    const response = await httpClient.post<AgentChatResponse>(
      '/fairbot/chat',
      formData,
      {
        headers: {
          'Content-Type': undefined,
          'X-Request-Id': requestId,
        },
        timeout: FAIRBOT_TIMEOUT_MS,
      }
    )

    const responseRequestId = readRequestIdFromHeaders(response.headers)
    if (!response.data.request_id && responseRequestId) {
      response.data.request_id = responseRequestId
    }

    const elapsedMs =
      (globalThis.performance?.now?.() ?? Date.now()) - startedAt
    console.info('[FairBot]', {
      requestId,
      status: 'success',
      elapsedMs: Math.round(elapsedMs),
      routedTo: response.data.routed_to,
      note: response.data.result?.note ?? null,
      intentHint: options?.intentHint ?? null,
    })

    return response.data
  } catch (error) {
    if (isAxiosError(error)) {
      const responseRequestId = readRequestIdFromHeaders(
        error.response?.headers
      )
      ;(error as { requestId?: string; status?: number }).requestId =
        responseRequestId ?? requestId
      ;(error as { status?: number }).status = error.response?.status
    }

    const elapsedMs =
      (globalThis.performance?.now?.() ?? Date.now()) - startedAt
    console.error('[FairBot]', {
      requestId,
      status: 'error',
      elapsedMs: Math.round(elapsedMs),
      intentHint: options?.intentHint ?? null,
      message: error instanceof Error ? error.message : 'Unknown error',
    })
    throw error
  }
}

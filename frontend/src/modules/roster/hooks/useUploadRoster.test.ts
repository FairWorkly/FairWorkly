import { describe, it, expect, vi, beforeEach } from 'vitest'

const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom')
  return { ...actual, useNavigate: () => mockNavigate }
})

vi.mock('@/services/rosterApi', () => ({
  uploadRoster: vi.fn(),
}))

const mockMutate = vi.fn()
const mockReset = vi.fn()
let mockIsPending = false
let mockError: { message: string } | null = null

vi.mock('@/shared/hooks/useApiMutation', () => ({
  useApiMutation: () => ({
    mutate: mockMutate,
    isPending: mockIsPending,
    error: mockError,
    reset: mockReset,
  }),
}))

// Mock React hooks to run outside React dispatcher
vi.mock('react', async () => {
  const actual = await vi.importActual<typeof import('react')>('react')
  let stateStore: Record<string, unknown> = {}
  let callIndex = 0
  return {
    ...actual,
    useState: (init: unknown) => {
      const key = `state-${callIndex++}`
      if (!(key in stateStore)) stateStore[key] = init
      return [stateStore[key], (val: unknown) => {
        stateStore[key] = typeof val === 'function' ? (val as (prev: unknown) => unknown)(stateStore[key]) : val
      }]
    },
    useRef: (init: unknown) => ({ current: init }),
    useCallback: (fn: unknown) => fn,
    // Reset state between tests
    __resetStateStore: () => { stateStore = {}; callIndex = 0 },
  }
})

import { useUploadRoster } from './useUploadRoster'

// Access the reset helper
const { __resetStateStore } = await import('react') as unknown as { __resetStateStore: () => void }

describe('useUploadRoster', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockIsPending = false
    mockError = null
    __resetStateStore()
  })

  it('returns initial state with empty files and no errors', () => {
    const result = useUploadRoster()

    expect(result.uploadedFiles).toEqual([])
    expect(result.isPending).toBe(false)
    expect(result.uploadError).toBeNull()
  })

  it('exposes isPending from mutation', () => {
    mockIsPending = true

    const result = useUploadRoster()

    expect(result.isPending).toBe(true)
  })

  it('formats uploadError as string from mutation error message', () => {
    mockError = { message: 'Upload failed' }

    const result = useUploadRoster()

    expect(result.uploadError).toBe('Upload failed')
  })

  it('handleStartAnalysis does nothing when no file is selected', () => {
    const result = useUploadRoster()

    result.handleStartAnalysis()

    expect(mockMutate).not.toHaveBeenCalled()
  })

  it('handleStartAnalysis calls reset and mutate with the file', () => {
    const result = useUploadRoster()

    // Simulate file selection by calling handleFileUpload
    const file = new File(['data'], 'roster.xlsx')
    const event = { target: { files: [file] } } as unknown as React.ChangeEvent<HTMLInputElement>
    result.handleFileUpload(event)

    result.handleStartAnalysis()

    expect(mockReset).toHaveBeenCalled()
    expect(mockMutate).toHaveBeenCalledWith(file, expect.objectContaining({
      onSuccess: expect.any(Function),
    }))
  })

  it('onSuccess navigates to results page with warnings', () => {
    const result = useUploadRoster()

    const file = new File(['data'], 'roster.xlsx')
    const event = { target: { files: [file] } } as unknown as React.ChangeEvent<HTMLInputElement>
    result.handleFileUpload(event)
    result.handleStartAnalysis()

    // Extract and invoke the onSuccess callback
    const onSuccess = mockMutate.mock.calls[0][1].onSuccess
    onSuccess({ rosterId: 'abc-123', warnings: [{ row: 1, message: 'test' }] })

    expect(mockNavigate).toHaveBeenCalledWith('/roster/results/abc-123', {
      state: { warnings: [{ row: 1, message: 'test' }] },
    })
  })

  it('handleCancel resets all state', () => {
    const result = useUploadRoster()

    result.handleCancel()

    expect(mockReset).toHaveBeenCalled()
  })

  it('handleFileUpload ignores empty file input', () => {
    const result = useUploadRoster()

    const event = { target: { files: [] } } as unknown as React.ChangeEvent<HTMLInputElement>
    result.handleFileUpload(event)

    expect(mockReset).not.toHaveBeenCalled()
  })

  it('handleRemoveFile calls reset', () => {
    const result = useUploadRoster()

    result.handleRemoveFile(123)

    expect(mockReset).toHaveBeenCalled()
  })
})

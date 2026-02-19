import { describe, it, expect, vi } from 'vitest'

vi.mock('@/services/rosterApi', () => ({
  uploadRoster: vi.fn(),
}))

vi.mock('@/shared/hooks/useApiMutation', () => ({
  useApiMutation: vi.fn(),
}))

import { uploadRoster } from '@/services/rosterApi'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { useUploadRoster } from './useUploadRoster'

const mockUseApiMutation = useApiMutation as ReturnType<typeof vi.fn>

describe('useUploadRoster', () => {
  it('calls useApiMutation with uploadRoster as mutationFn', () => {
    const mockReturn = {
      mutate: vi.fn(),
      isPending: false,
      error: null,
      reset: vi.fn(),
    }
    mockUseApiMutation.mockReturnValue(mockReturn)

    const result = useUploadRoster()

    expect(mockUseApiMutation).toHaveBeenCalledWith({
      mutationFn: expect.any(Function),
    })
    expect(result).toBe(mockReturn)
  })

  it('mutationFn delegates to uploadRoster service', () => {
    mockUseApiMutation.mockImplementation((options: { mutationFn: (file: File) => unknown }) => {
      // Capture and expose the mutationFn for testing
      return { mutationFn: options.mutationFn }
    })

    const { mutationFn } = useUploadRoster() as unknown as { mutationFn: (file: File) => unknown }
    const mockFile = new File(['content'], 'roster.xlsx')

    mutationFn(mockFile)

    expect(uploadRoster).toHaveBeenCalledWith(mockFile)
  })
})

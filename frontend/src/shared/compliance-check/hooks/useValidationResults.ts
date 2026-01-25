import { useCallback, useState } from 'react'
import type { ValidationStatus } from '../types/complianceCheck.type'

// TODO: [Backend Integration] This hook currently uses setTimeout for auto-completion.
// When integrating with backend:
// 1. Replace autoCompleteMs with polling interval or WebSocket connection
// 2. Poll backend for validation status (e.g., GET /api/compliance/status/:id)
// 3. Or subscribe to WebSocket events for real-time status updates
// 4. Handle long-running validations with progress indicators

type StartOptions = {
  autoCompleteMs?: number
  onComplete?: () => void
}

export function useValidationLifecycle(initialStatus: ValidationStatus = 'idle') {
  const [status, setStatus] = useState<ValidationStatus>(initialStatus)

  const startProcessing = useCallback((options: StartOptions = {}) => {
    setStatus('processing')

    if (options.autoCompleteMs) {
      window.setTimeout(() => {
        setStatus('completed')
        options.onComplete?.()
      }, options.autoCompleteMs)
    }
  }, [])

  const completeProcessing = useCallback(() => {
    setStatus('completed')
  }, [])

  const resetProcessing = useCallback(() => {
    setStatus('idle')
  }, [])

  return {
    status,
    setStatus,
    startProcessing,
    completeProcessing,
    resetProcessing,
  }
}

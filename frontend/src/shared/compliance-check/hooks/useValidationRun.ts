import { useState, useCallback, useRef } from 'react'
import type { UploadedFile, ValidationStatus } from '../types/complianceCheck.type'

// TODO: [Backend Integration] This hook currently uses mock data and setTimeout.
// When integrating with backend:
// 1. handleFileUpload: Upload file to backend API (e.g., POST /api/compliance/upload)
// 2. startProcessing: Call backend validation API (e.g., POST /api/compliance/validate)
// 3. Replace setTimeout with actual API response or polling mechanism
// 4. Handle upload progress, validation status polling, and error states

interface UseValidationRunOptions {
  onUploadComplete?: () => void
  onProcessingComplete?: () => void
  processingDurationMs?: number
}

interface UseValidationRunReturn {
  status: ValidationStatus
  uploadedFiles: UploadedFile[]
  handleFileUpload: (e: React.ChangeEvent<HTMLInputElement>) => void
  removeFile: (id: number) => void
  clearFiles: () => void
  startProcessing: () => void
  resetValidation: () => void
  showResults: () => void
}

export function useValidationRun(
  options: UseValidationRunOptions = {}
): UseValidationRunReturn {
  const {
    onUploadComplete,
    onProcessingComplete,
    processingDurationMs = 2500,
  } = options

  const [status, setStatus] = useState<ValidationStatus>('idle')
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([])
  const timeoutRef = useRef<ReturnType<typeof setTimeout>>()

  const handleFileUpload = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      if (e.target.files && e.target.files.length > 0) {
        const file = e.target.files[0]
        const newFile: UploadedFile = {
          id: Date.now(),
          name: file.name,
          size: `${Math.round(file.size / 1024)} KB`,
          date: new Date().toLocaleDateString(),
          status: 'ready',
        }
        setUploadedFiles(prev => [...prev, newFile])
        onUploadComplete?.()
      }
    },
    [onUploadComplete]
  )

  const removeFile = useCallback((id: number) => {
    setUploadedFiles(prev => prev.filter(f => f.id !== id))
  }, [])

  const clearFiles = useCallback(() => {
    setUploadedFiles([])
  }, [])

  const startProcessing = useCallback(() => {
    setStatus('processing')

    timeoutRef.current = setTimeout(() => {
      setStatus('completed')
      onProcessingComplete?.()
    }, processingDurationMs)
  }, [processingDurationMs, onProcessingComplete])

  const showResults = useCallback(() => {
    setStatus('idle')
  }, [])

  const resetValidation = useCallback(() => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current)
    }
    setUploadedFiles([])
    setStatus('idle')
  }, [])

  return {
    status,
    uploadedFiles,
    handleFileUpload,
    removeFile,
    clearFiles,
    startProcessing,
    resetValidation,
    showResults,
  }
}

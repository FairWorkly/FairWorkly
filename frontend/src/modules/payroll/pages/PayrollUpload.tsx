import React, { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ComplianceUpload } from '@/shared/compliance-check'
import type { ComplianceConfig, UploadedFile } from '@/shared/compliance-check'

// TODO: [Backend Integration] Replace mock timer with real API call.
// When integrating with backend:
// 1. Create payrollApi.ts with uploadPayroll() (similar to rosterApi.ts uploadRoster())
// 2. Replace handleStartAnalysis mock timer with actual API call
// 3. Extract error via err?.response?.data?.message and pass to ComplianceUpload error prop
//    - ComplianceUpload already supports `error` prop with whiteSpace: 'pre-line' for multi-line errors
// 4. See RosterUpload.tsx for reference implementation

const mockConfig: ComplianceConfig = {
  title: 'Upload Payroll',
  fileTypes: ['CSV'],
  maxFileSize: '50MB',
  coverageAreas: ['Awards', 'Classifications', 'Allowances'],
}

const payrollValidationItems = [
  'Base rates & award classifications',
  'Penalty rates (weekends & public holidays)',
  'Casual loading (25%)',
  'Superannuation guarantee',
  'Single Touch Payroll (STP) compliance',
]

export function PayrollUpload() {
  const navigate = useNavigate()
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([])
  const [isProcessing, setIsProcessing] = useState(false)
  const processingStartRef = useRef<number | null>(null)
  const pollingTimerRef = useRef<number | null>(null)

  const minProcessingMs = 2000
  const mockProcessingMs = 1200
  const pollIntervalMs = 300

  useEffect(() => {
    return () => {
      if (pollingTimerRef.current !== null) {
        window.clearInterval(pollingTimerRef.current)
      }
    }
  }, [])

  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0]
    if (!file) {
      return
    }

    const newFile: UploadedFile = {
      id: Date.now(),
      name: file.name,
      size: `${Math.ceil(file.size / 1024)} KB`,
      date: new Date().toISOString(),
      status: 'ready',
    }

    setUploadedFiles([newFile])
  }

  const handleRemoveFile = (id: number) => {
    setUploadedFiles(prev => prev.filter(file => file.id !== id))
  }

  const handleStartAnalysis = () => {
    setIsProcessing(true)
    processingStartRef.current = Date.now()

    const pollStatus = () => {
      if (processingStartRef.current === null) {
        return
      }

      const elapsedMs = Date.now() - processingStartRef.current
      const isComplete = elapsedMs >= mockProcessingMs
      const minTimeReached = elapsedMs >= minProcessingMs

      if (isComplete && minTimeReached) {
        if (pollingTimerRef.current !== null) {
          window.clearInterval(pollingTimerRef.current)
          pollingTimerRef.current = null
        }
        navigate('/payroll/results')
      }
    }

    pollStatus()
    pollingTimerRef.current = window.setInterval(pollStatus, pollIntervalMs)
  }

  const handleCancel = () => {
    if (pollingTimerRef.current !== null) {
      window.clearInterval(pollingTimerRef.current)
      pollingTimerRef.current = null
    }
    processingStartRef.current = null
    setIsProcessing(false)
    setUploadedFiles([])
  }

  return (
    <ComplianceUpload
      config={mockConfig}
      uploadedFiles={uploadedFiles}
      onFileUpload={handleFileUpload}
      onRemoveFile={handleRemoveFile}
      onStartAnalysis={handleStartAnalysis}
      onCancel={handleCancel}
      validationItems={payrollValidationItems}
      isLoading={isProcessing}
    />
  )
}

import React, { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  ComplianceUpload,
  ComplianceProcessing,
  AwardSelector,
} from '@/shared/compliance-check'
import type {
  ComplianceConfig,
  UploadedFile,
  AwardType,
} from '@/shared/compliance-check'

const mockConfig: ComplianceConfig = {
  title: 'Upload Roster',
  fileTypes: ['CSV'],
  maxFileSize: '50MB',
  coverageAreas: ['Shifts', 'Breaks', 'Hours'],
}

const rosterValidationItems = [
  'Minimum shift hours compliance',
  'Maximum consecutive days worked',
  'Meal break requirements',
  'Rest period between shifts',
  'Weekly hours limit',
]

export function RosterUpload() {
  const navigate = useNavigate()
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([])
  const [isProcessing, setIsProcessing] = useState(false)
  const [selectedAward, setSelectedAward] = useState<AwardType>('retail')
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
        navigate('/roster/results')
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

  if (isProcessing) {
    return (
      <ComplianceProcessing
        uploadedFiles={uploadedFiles}
        awardName="General Retail Industry Award"
      />
    )
  }

  return (
    <ComplianceUpload
      config={mockConfig}
      uploadedFiles={uploadedFiles}
      onFileUpload={handleFileUpload}
      onRemoveFile={handleRemoveFile}
      onStartAnalysis={handleStartAnalysis}
      onCancel={handleCancel}
      validationItems={rosterValidationItems}
      configSection={
        <AwardSelector
          selectedAward={selectedAward}
          onAwardChange={setSelectedAward}
        />
      }
    />
  )
}

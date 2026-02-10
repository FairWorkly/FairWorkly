import React, { useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Alert, AlertTitle, Box, Typography } from '@mui/material'
import { ComplianceUpload } from '@/shared/compliance-check'
import type {
  ComplianceConfig,
  UploadedFile,
} from '@/shared/compliance-check'
import { uploadRoster, type ParserWarning } from '@/services/rosterApi'

const mockConfig: ComplianceConfig = {
  title: 'Upload Roster',
  fileTypes: ['XLSX'],
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
  const [error, setError] = useState<string | null>(null)
  const [warnings, setWarnings] = useState<ParserWarning[]>([])
  const actualFileRef = useRef<File | null>(null)

  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0]
    if (!file) {
      return
    }

    // Store actual File object for upload
    actualFileRef.current = file

    const newFile: UploadedFile = {
      id: Date.now(),
      name: file.name,
      size: `${Math.ceil(file.size / 1024)} KB`,
      date: new Date().toISOString(),
      status: 'ready',
    }

    setUploadedFiles([newFile])
    setError(null)
    setWarnings([])
  }

  const handleRemoveFile = (id: number) => {
    setUploadedFiles(prev => prev.filter(file => file.id !== id))
    actualFileRef.current = null
    setError(null)
    setWarnings([])
  }

  const handleStartAnalysis = async () => {
    if (!actualFileRef.current) {
      setError('No file selected')
      return
    }

    setIsProcessing(true)
    setError(null)
    setWarnings([])

    try {
      const response = await uploadRoster(actualFileRef.current)

      // Store warnings if present
      if (response.warnings && response.warnings.length > 0) {
        setWarnings(response.warnings)
        console.warn('Roster uploaded with warnings:', response.warnings)
      }

      navigate('/roster/results')
    } catch (err: any) {
      const errorMessage =
        err?.response?.data?.message || err?.message || 'Failed to upload roster. Please try again.'
      setError(errorMessage)
      setIsProcessing(false)
    }
  }

  const handleCancel = () => {
    setIsProcessing(false)
    setUploadedFiles([])
    actualFileRef.current = null
    setError(null)
    setWarnings([])
  }

  return (
    <>
      {warnings.length > 0 && (
        <Alert severity="warning" sx={{ mb: 3 }}>
          <AlertTitle>Warnings</AlertTitle>
          <Box component="ul" sx={{ pl: 2, mt: 1, mb: 0 }}>
            {warnings.map((w, idx) => (
              <Typography component="li" variant="body2" key={idx}>
                Row {w.row}: {w.message}
                {w.hint && (
                  <Typography component="span" variant="caption" sx={{ ml: 0.5 }}>
                    ({w.hint})
                  </Typography>
                )}
              </Typography>
            ))}
          </Box>
        </Alert>
      )}

      <ComplianceUpload
        config={mockConfig}
        uploadedFiles={uploadedFiles}
        onFileUpload={handleFileUpload}
        onRemoveFile={handleRemoveFile}
        onStartAnalysis={handleStartAnalysis}
        onCancel={handleCancel}
        acceptFileTypes=".xlsx"
        validationItems={rosterValidationItems}
        isLoading={isProcessing}
        error={error}
      />
    </>
  )
}

import React, { useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Alert, AlertTitle, Typography } from '@mui/material'
import { styled } from '@/styles/styled'
import { ComplianceUpload } from '@/shared/compliance-check'
import type {
  ComplianceConfig,
  UploadedFile,
} from '@/shared/compliance-check'
import type { ApiError } from '@/shared/types/api.types'
import { uploadRoster, type ParserWarning } from '@/services/rosterApi'

const WarningAlert = styled(Alert)(({ theme }) => ({
  marginBottom: theme.spacing(3),
}))

const WarningList = styled('ul')(({ theme }) => ({
  paddingLeft: theme.spacing(2),
  marginTop: theme.spacing(1),
  marginBottom: 0,
}))

const HintText = styled('span')(({ theme }) => ({
  marginLeft: theme.spacing(0.5),
  ...theme.typography.caption,
}))

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
    } catch (err) {
      const errorMessage = (err as ApiError).message ?? 'Failed to upload roster. Please try again.'
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
        <WarningAlert severity="warning">
          <AlertTitle>Warnings</AlertTitle>
          <WarningList>
            {warnings.map((w, idx) => (
              <Typography component="li" variant="body2" key={idx}>
                Row {w.row}: {w.message}
                {w.hint && (
                  <HintText>
                    ({w.hint})
                  </HintText>
                )}
              </Typography>
            ))}
          </WarningList>
        </WarningAlert>
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

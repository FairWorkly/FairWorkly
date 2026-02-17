import React, { useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ComplianceUpload } from '@/shared/compliance-check'
import type { ComplianceConfig, UploadedFile } from '@/shared/compliance-check'
import { uploadPayrollValidation } from '@/services/payrollApi'
import type { ApiError } from '@/shared/types/api.types'

const payrollConfig: ComplianceConfig = {
  title: 'Upload Payroll',
  fileTypes: ['CSV'],
  maxFileSize: '2MB',
  coverageAreas: ['Awards', 'Classifications', 'Allowances'],
}

const payrollValidationItems = [
  'Base rates & award classifications',
  'Penalty rates (weekends & public holidays)',
  'Casual loading (25%)',
  'Superannuation guarantee',
  // Future: 'Single Touch Payroll(STP) compliance'
]

export function PayrollUpload() {
  const navigate = useNavigate()
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([])
  const [isProcessing, setIsProcessing] = useState(false)
  const [error, setError] = useState<string | null>(null)
  // ComplianceUpload exposes a synthetic ChangeEvent — keep the real File
  // object in a ref so we can send it via FormData on submit.
  const actualFileRef = useRef<File | null>(null)

  // Toggle state for each validation item, order matches payrollValidationItems:
  // [0] base rate, [1] penalty rate, [2] casual loading, [3] super
  const [enableChecks, setEnableChecks] = useState([true, true, true, true])

  // Store the real File and build an UploadedFile preview card for the UI.
  // ComplianceUpload only renders the card — it never touches the File itself.
  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0]
    if (!file) return

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
  }

  const handleRemoveFile = (id: number) => {
    setUploadedFiles(prev => prev.filter(file => file.id !== id))
    actualFileRef.current = null
    setError(null)
  }

  // Submit flow: validate file → POST /payroll/validation (multipart) →
  // persist result to sessionStorage → navigate to results page.
  // On failure: show error message, stay on this page.
  const handleStartAnalysis = async () => {
    if (!actualFileRef.current) {
      setError('No file selected')
      return
    }

    setIsProcessing(true)
    setError(null)

    try {
      // TODO: awardType and state are hardcoded for MVP; will be
      // user-selectable once multi-award support lands.
      const result = await uploadPayrollValidation(actualFileRef.current, {
        awardType: 'GeneralRetailIndustryAward2020',
        state: 'VIC',
        enableBaseRateCheck: enableChecks[0],
        enablePenaltyCheck: enableChecks[1],
        enableCasualLoadingCheck: enableChecks[2],
        enableSuperCheck: enableChecks[3],
      })

      // Persist to sessionStorage so the results page survives a
      // browser refresh (router state is lost on reload).
      sessionStorage.setItem(
        'payroll-validation-result',
        JSON.stringify(result)
      )

      navigate('/payroll/results', { state: { result } })
    } catch (err) {
      const apiError = err as ApiError
      console.log('Payroll validation error:', apiError)
      console.log('Error details:', apiError.details)
      setError(apiError.message)
      setIsProcessing(false)
    }
  }

  const handleCancel = () => {
    setIsProcessing(false)
    setUploadedFiles([])
    actualFileRef.current = null
    setError(null)
  }

  return (
    <ComplianceUpload
      config={payrollConfig}
      uploadedFiles={uploadedFiles}
      onFileUpload={handleFileUpload}
      onRemoveFile={handleRemoveFile}
      onStartAnalysis={handleStartAnalysis}
      onCancel={handleCancel}
      acceptFileTypes=".csv"
      validationItems={payrollValidationItems}
      validationItemStates={enableChecks}
      onToggleValidationItem={i =>
        setEnableChecks(prev => prev.map((v, j) => (j === i ? !v : v)))
      }
      isLoading={isProcessing}
      error={error}
    />
  )
}

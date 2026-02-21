import { ComplianceUpload } from '@/shared/compliance-check'
import type { ComplianceConfig } from '@/shared/compliance-check'
import { useUploadRoster } from '../hooks'

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
  const {
    uploadedFiles,
    isPending,
    uploadError,
    handleFileUpload,
    handleRemoveFile,
    handleStartAnalysis,
    handleCancel,
  } = useUploadRoster()

  return (
    <ComplianceUpload
      config={mockConfig}
      uploadedFiles={uploadedFiles}
      onFileUpload={handleFileUpload}
      onRemoveFile={handleRemoveFile}
      onStartAnalysis={handleStartAnalysis}
      onCancel={handleCancel}
      acceptFileTypes=".xlsx"
      validationItems={rosterValidationItems}
      isLoading={isPending}
      error={uploadError}
    />
  )
}

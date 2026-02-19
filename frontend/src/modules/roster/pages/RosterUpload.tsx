import { Alert, AlertTitle, Typography } from '@mui/material'
import { styled } from '@/styles/styled'
import { ComplianceUpload } from '@/shared/compliance-check'
import type { ComplianceConfig } from '@/shared/compliance-check'
import { useUploadRoster } from '../hooks'

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
  const {
    uploadedFiles,
    warnings,
    isPending,
    uploadError,
    handleFileUpload,
    handleRemoveFile,
    handleStartAnalysis,
    handleCancel,
  } = useUploadRoster()

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
        isLoading={isPending}
        error={uploadError}
      />
    </>
  )
}

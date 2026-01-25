import React from 'react'
import {
  Box,
  Typography,
  CircularProgress,
  Paper,
  styled,
  useTheme,
} from '@mui/material'
import CheckCircleOutlinedIcon from '@mui/icons-material/CheckCircleOutlined'
import type { UploadedFile } from '../types/complianceCheck.type'

const ProcessingCard = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(4),
  borderRadius: theme.fairworkly.radius.xl,
  border: `1px solid ${theme.palette.divider}`,
  backgroundColor: theme.palette.background.paper,
  boxShadow: 'none',
  marginTop: theme.spacing(4),
}))

const ProcessingBody = styled(Box)(({ theme }) => ({
  paddingTop: theme.spacing(8),
  paddingBottom: theme.spacing(8),
  textAlign: 'center',
}))

const ProcessingIndicator = styled(CircularProgress)(({ theme }) => ({
  marginBottom: theme.spacing(4),
  color: theme.palette.primary.main,
}))

const ProcessingHeading = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(1.5),
}))

const ProcessingMessage = styled(Typography)(({ theme }) => ({
  maxWidth: theme.spacing(57.5),
  marginLeft: 'auto',
  marginRight: 'auto',
}))

const UploadSuccessBadge = styled(Box)(({ theme }) => ({
  marginTop: theme.spacing(6),
  display: 'inline-flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  paddingLeft: theme.spacing(3),
  paddingRight: theme.spacing(3),
  paddingTop: theme.spacing(1.5),
  paddingBottom: theme.spacing(1.5),
  backgroundColor: theme.palette.background.default,
  borderRadius: theme.fairworkly.radius.md,
}))

const UploadSuccessIcon = styled(CheckCircleOutlinedIcon)(({ theme }) => ({
  color: theme.palette.success.main,
  fontSize: theme.typography.pxToRem(20),
}))

const UploadSuccessText = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.subtitle1.fontWeight,
  color: theme.palette.text.primary,
}))

interface ComplianceProcessingProps {
  uploadedFiles: UploadedFile[]
  awardName: string
  processingTitle?: string
  processingDescription?: string
}

export const ComplianceProcessing: React.FC<ComplianceProcessingProps> = ({
  uploadedFiles,
  awardName,
  processingTitle = 'Verifying...',
  processingDescription,
}) => {
  const theme = useTheme()
  const indicatorSize = parseFloat(theme.spacing(8))
  const indicatorThickness = parseFloat(theme.spacing(0.5))
  const defaultDescription = `Scanning ${uploadedFiles.length} file(s) against the ${awardName} and applicable compliance rules.`

  return (
    <ProcessingCard>
      <ProcessingBody>
        <ProcessingIndicator
          size={indicatorSize}
          thickness={indicatorThickness}
        />
        <ProcessingHeading variant="h5">{processingTitle}</ProcessingHeading>
        <ProcessingMessage variant="body1" color="text.secondary">
          {processingDescription || defaultDescription}
        </ProcessingMessage>

        <UploadSuccessBadge>
          <UploadSuccessIcon />
          <UploadSuccessText variant="body2">
            File uploaded successfully
          </UploadSuccessText>
        </UploadSuccessBadge>
      </ProcessingBody>
    </ProcessingCard>
  )
}

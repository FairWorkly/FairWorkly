import React, { useRef } from 'react'
import {
  Box,
  Typography,
  Button,
  IconButton,
  Paper,
  Divider,
  styled,
} from '@mui/material'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import InsertDriveFileOutlinedIcon from '@mui/icons-material/InsertDriveFileOutlined'
import CloudUploadOutlinedIcon from '@mui/icons-material/CloudUploadOutlined'
import FolderOpenOutlinedIcon from '@mui/icons-material/FolderOpenOutlined'
import CheckCircleOutlinedIcon from '@mui/icons-material/CheckCircleOutlined'
import CloseOutlinedIcon from '@mui/icons-material/CloseOutlined'
import TuneOutlinedIcon from '@mui/icons-material/TuneOutlined'
import type {
  UploadedFile,
  ComplianceConfig,
} from '../types/complianceCheck.type'

const UploadIconContainer = styled(Box)(({ theme }) => ({
  width: theme.spacing(8),
  height: theme.spacing(8),
  borderRadius: theme.fairworkly.radius.lg,
  backgroundColor: theme.palette.background.default,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.primary.main,
  marginBottom: theme.spacing(2.5),
}))

const DropzonePaper = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(8),
  border: `2px dashed ${theme.palette.divider}`,
  textAlign: 'center',
  backgroundColor: theme.palette.background.default,
  cursor: 'pointer',
  borderRadius: theme.fairworkly.radius.xl,
  boxShadow: 'none',
  transition: theme.transitions.create(['border-color', 'background-color'], {
    duration: theme.transitions.duration.short,
  }),
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  '&:hover': {
    borderColor: theme.palette.primary.main,
    backgroundColor: theme.palette.background.paper,
  },
}))

const PrimaryButton = styled(Button)(({ theme }) => ({
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  padding: theme.spacing(1.5, 3.5),
  borderRadius: theme.fairworkly.radius.md,
  fontWeight: theme.typography.button.fontWeight,
  '&:hover': {
    backgroundColor: theme.palette.primary.dark,
  },
}))

const FileCard = styled(Box)(({ theme }) => ({
  padding: theme.spacing(4),
  backgroundColor: theme.palette.background.paper,
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: theme.fairworkly.radius.lg,
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(3),
}))

const FileIconContainer = styled(Box)(({ theme }) => ({
  width: theme.spacing(7.5),
  height: theme.spacing(7.5),
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.fairworkly.radius.md,
  border: `1px solid ${theme.palette.divider}`,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.primary.main,
}))

const ConfigHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  marginBottom: theme.spacing(4),
}))

const ValidationListItem = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(2, 2.5),
  borderRadius: theme.fairworkly.radius.md,
  border: `1px solid ${theme.palette.background.default}`,
  marginBottom: theme.spacing(1.25),
  backgroundColor: theme.palette.background.paper,
  transition: theme.transitions.create(['border-color', 'background-color'], {
    duration: theme.transitions.duration.shortest,
  }),
  '&:hover': {
    borderColor: theme.palette.divider,
    backgroundColor: theme.palette.background.default,
  },
  '&:last-child': {
    marginBottom: 0,
  },
}))

const CoverageCheckIcon = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  marginRight: theme.spacing(2),
  color: theme.palette.success.main,
}))

const ConfigPaper = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(4),
  borderRadius: theme.fairworkly.radius.xl,
  border: `1px solid ${theme.palette.divider}`,
  backgroundColor: theme.palette.background.paper,
  boxShadow: 'none',
  marginTop: theme.spacing(4),
}))

const ActionButtonGroup = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'center',
  gap: theme.spacing(2),
  marginTop: theme.spacing(6),
}))

const PageTitle = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(4),
  color: theme.palette.text.primary,
}))

const FileName = styled(Typography)(({ theme }) => ({
  ...theme.typography.subtitle1,
  color: theme.palette.text.primary,
}))

const DeleteButton = styled(IconButton)(({ theme }) => ({
  color: theme.palette.text.disabled,
  '&:hover': {
    color: theme.palette.error.main,
  },
}))

const CancelButton = styled(Button)(({ theme }) => ({
  borderColor: theme.palette.divider,
  color: theme.palette.text.secondary,
  paddingLeft: theme.spacing(4),
  paddingRight: theme.spacing(4),
}))

const FileListContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(3),
}))

const CoverageSection = styled(Box)(({ theme }) => ({
  marginTop: theme.spacing(3),
}))

const DropzoneTitle = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(1),
}))

const DropzoneSubtitle = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(5),
}))

const DropzoneHint = styled(Typography)(({ theme }) => ({
  marginTop: theme.spacing(6),
}))

const IconLarge = styled(Box)(({ theme }) => ({
  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(4),
  },
}))

const IconMedium = styled(Box)(({ theme }) => ({
  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(3),
  },
}))

const IconSmall = styled(Box)(({ theme }) => ({
  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(2.5),
  },
}))

const ConfigDivider = styled(Divider)(({ theme }) => ({
  marginTop: theme.spacing(5),
  marginBottom: theme.spacing(5),
  borderStyle: 'dashed',
}))

const ConfigHeaderTitle = styled(Typography)(({ theme }) => ({
  ...theme.typography.h6,
  fontWeight: theme.typography.fontWeightBold,
}))

const ValidationCoverageTitle = styled(Typography)(({ theme }) => ({
  ...theme.typography.h6,
  fontWeight: theme.typography.fontWeightBold,
  marginBottom: theme.spacing(2),
}))

const ValidationItemText = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  fontWeight: theme.typography.fontWeightMedium,
}))

interface ComplianceUploadProps {
  config: ComplianceConfig
  uploadedFiles: UploadedFile[]
  onFileUpload: (e: React.ChangeEvent<HTMLInputElement>) => void
  onRemoveFile: (id: number) => void
  onStartAnalysis: () => void
  onCancel: () => void
  acceptFileTypes?: string
  configSection?: React.ReactNode
  validationItems?: string[]
}

export const ComplianceUpload: React.FC<ComplianceUploadProps> = ({
  config,
  uploadedFiles,
  onFileUpload,
  onRemoveFile,
  onStartAnalysis,
  onCancel,
  acceptFileTypes = '.csv',
  configSection,
  validationItems = [],
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null)
  const handleFileSelection = (files: FileList | null) => {
    if (!files?.length) {
      return
    }

    const event = {
      target: { files },
    } as React.ChangeEvent<HTMLInputElement>

    onFileUpload(event)
  }

  const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault()
    event.stopPropagation()
    handleFileSelection(event.dataTransfer.files)
  }

  const handleDragOver = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault()
  }

  return (
    <Box>
      <PageTitle variant="h4">{config.title}</PageTitle>

      {uploadedFiles.length > 0 ? (
        <FileListContainer>
          {uploadedFiles.map(file => (
            <FileCard key={file.id}>
              <FileIconContainer>
                <IconLarge>
                  <InsertDriveFileOutlinedIcon />
                </IconLarge>
              </FileIconContainer>
              <Box flex={1}>
                <FileName>{file.name}</FileName>
                <Typography variant="caption" color="text.secondary">
                  {file.size} - Ready for analysis
                </Typography>
              </Box>
              <DeleteButton onClick={() => onRemoveFile(file.id)}>
                <DeleteOutlinedIcon />
              </DeleteButton>
            </FileCard>
          ))}
        </FileListContainer>
      ) : (
        <Box component="label" display="block" style={{ cursor: 'pointer' }}>
          <DropzonePaper onDrop={handleDrop} onDragOver={handleDragOver}>
            <input
              type="file"
              ref={fileInputRef}
              hidden
              onChange={event => handleFileSelection(event.target.files)}
              accept={acceptFileTypes}
            />
            <UploadIconContainer>
              <IconLarge>
                <CloudUploadOutlinedIcon />
              </IconLarge>
            </UploadIconContainer>
            <DropzoneTitle variant="h5" color="text.primary">
              Drag and drop your{' '}
              {config.title.toLowerCase().replace('upload ', '')} file
            </DropzoneTitle>
            <DropzoneSubtitle variant="body1" color="text.secondary">
              or click to browse from your computer
            </DropzoneSubtitle>
            <PrimaryButton
              startIcon={
                <IconSmall>
                  <FolderOpenOutlinedIcon />
                </IconSmall>
              }
              onClick={() => fileInputRef.current?.click()}
            >
              Choose File
            </PrimaryButton>
            <DropzoneHint variant="caption" color="text.disabled">
              Supported: {config.fileTypes.join(', ')} - Max size:{' '}
              {config.maxFileSize}
            </DropzoneHint>
          </DropzonePaper>
        </Box>
      )}

      <ConfigPaper>
        <ConfigHeader>
          <IconMedium>
            <TuneOutlinedIcon />
          </IconMedium>
          <ConfigHeaderTitle>Configuration</ConfigHeaderTitle>
        </ConfigHeader>

        {configSection}

        <ConfigDivider />

        <CoverageSection>
          <ValidationCoverageTitle>Validation Coverage</ValidationCoverageTitle>
          {validationItems.length > 0 && (
            <Box>
              {validationItems.map((item, index) => (
                <ValidationListItem key={index}>
                  <CoverageCheckIcon>
                    <IconSmall>
                      <CheckCircleOutlinedIcon />
                    </IconSmall>
                  </CoverageCheckIcon>
                  <ValidationItemText>{item}</ValidationItemText>
                </ValidationListItem>
              ))}
            </Box>
          )}
        </CoverageSection>

        <ActionButtonGroup>
          <CancelButton
            variant="outlined"
            onClick={onCancel}
            startIcon={<CloseOutlinedIcon />}
          >
            Cancel
          </CancelButton>
          <PrimaryButton
            variant="contained"
            disabled={uploadedFiles.length === 0}
            startIcon={<CheckCircleOutlinedIcon />}
            onClick={onStartAnalysis}
          >
            Start Validation
          </PrimaryButton>
        </ActionButtonGroup>
      </ConfigPaper>
    </Box>
  )
}

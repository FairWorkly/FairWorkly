import React from 'react'
import { Button, styled } from '@mui/material'
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined'

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

const OutlinedButton = styled(Button)(({ theme }) => ({
  borderColor: theme.palette.divider,
  color: theme.palette.text.primary,
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.fairworkly.radius.sm,
  paddingLeft: theme.spacing(2),
  paddingRight: theme.spacing(2),
  height: theme.spacing(5),
}))

interface ExportButtonProps {
  onClick: () => void
  isExporting?: boolean
  label?: string
  variant?: 'solid' | 'outlined'
}

export const ExportButton: React.FC<ExportButtonProps> = ({
  onClick,
  isExporting = false,
  label = 'Export CSV',
  variant = 'solid',
}) => {
  if (variant === 'outlined') {
    return (
      <OutlinedButton
        variant="outlined"
        size="small"
        startIcon={<FileDownloadOutlinedIcon />}
        onClick={onClick}
        disabled={isExporting}
      >
        {isExporting ? 'Exporting...' : label}
      </OutlinedButton>
    )
  }

  return (
    <PrimaryButton
      variant="contained"
      startIcon={<FileDownloadOutlinedIcon />}
      onClick={onClick}
      disabled={isExporting}
    >
      {isExporting ? 'Exporting...' : label}
    </PrimaryButton>
  )
}

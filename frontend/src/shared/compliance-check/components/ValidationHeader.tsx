import React from 'react'
import { Box, Typography, Button, Stack, styled, alpha } from '@mui/material'
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined'
import DateRangeOutlinedIcon from '@mui/icons-material/DateRangeOutlined'
import EventAvailableOutlinedIcon from '@mui/icons-material/EventAvailableOutlined'
import FingerprintOutlinedIcon from '@mui/icons-material/FingerprintOutlined'
import AddOutlinedIcon from '@mui/icons-material/AddOutlined'
import type { ValidationMetadata } from '../types/complianceCheck.type'
import { formatDate } from '../utils/formatters'

const HeaderContainer = styled(Box)(({ theme }) => ({
  marginBottom: theme.spacing(3),
  [theme.breakpoints.up('sm')]: {
    marginBottom: theme.spacing(4),
  },
}))

const ValidationBreadcrumbs = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  marginBottom: theme.spacing(1),
}))

const BreadcrumbAction = styled('button')(({ theme }) => ({
  background: 'none',
  border: 'none',
  padding: 0,
  color: theme.palette.text.disabled,
  fontSize: theme.typography.body2.fontSize,
  fontWeight: theme.typography.caption.fontWeight,
  textDecoration: 'none',
  cursor: 'pointer',
  fontFamily: 'inherit',
  '&:hover': {
    color: theme.palette.primary.main,
  },
}))

const BreadcrumbDivider = styled(Typography)(({ theme }) => ({
  color: theme.palette.divider,
  fontWeight: theme.typography.subtitle1.fontWeight,
}))

const BreadcrumbCurrent = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.primary,
  fontWeight: theme.typography.subtitle1.fontWeight,
}))

const HeaderContent = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'flex-start',
  gap: theme.spacing(2),
  width: '100%',
  [theme.breakpoints.up('md')]: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    width: 'auto',
  },
}))

const HeaderActions = styled(Stack)(({ theme }) => ({
  width: '100%',
  flexDirection: 'column',
  gap: theme.spacing(1.5),
  [theme.breakpoints.up('md')]: {
    width: 'auto',
    flexDirection: 'row',
    gap: theme.spacing(2),
  },
}))

const PeriodInfo = styled(Stack)(({ theme }) => ({
  alignItems: 'center',
  flexWrap: 'wrap',
  rowGap: theme.spacing(0.5),
}))

const PeriodGlyph = styled(DateRangeOutlinedIcon)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.primary.main,
}))

const PeriodValue = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.subtitle1.fontWeight,
  color: theme.palette.text.primary,
  whiteSpace: 'normal',
  [theme.breakpoints.up('md')]: {
    whiteSpace: 'nowrap',
  },
}))

const MetadataStack = styled(Stack)(({ theme }) => ({
  alignItems: 'flex-start',
  color: theme.palette.text.disabled,
  marginTop: theme.spacing(0.5),
  flexWrap: 'wrap',
  rowGap: theme.spacing(1),
  [theme.breakpoints.up('md')]: {
    alignItems: 'center',
  },
}))

const MetadataItem = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  whiteSpace: 'normal',
  [theme.breakpoints.up('md')]: {
    whiteSpace: 'nowrap',
  },
}))

const MetadataIconGlyph = styled('span')({
  fontSize: 16,
  display: 'inline-flex',
  alignItems: 'center',
})

const ExportAction = styled(Button)(({ theme }) => ({
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  paddingLeft: theme.spacing(4),
  paddingRight: theme.spacing(4),
  borderRadius: theme.fairworkly.radius.md,
  fontWeight: theme.typography.button.fontWeight,
  width: '100%',
  [theme.breakpoints.up('md')]: {
    width: 'auto',
  },
  '&:hover': {
    backgroundColor: theme.palette.primary.dark,
  },
}))

const NewValidationAction = styled(Button)(({ theme }) => ({
  borderColor: theme.palette.divider,
  color: theme.palette.primary.main,
  backgroundColor: alpha(theme.palette.primary.main, 0.05),
  paddingLeft: theme.spacing(3),
  paddingRight: theme.spacing(3),
  width: '100%',
  [theme.breakpoints.up('md')]: {
    width: 'auto',
  },
  '&:hover': {
    borderColor: theme.palette.primary.main,
    backgroundColor: alpha(theme.palette.primary.main, 0.1),
  },
}))

interface ValidationHeaderProps {
  metadata: ValidationMetadata
  onNewValidation: () => void
  onExport: () => void
  onNavigateBack: () => void
  breadcrumbLabel?: string
  periodLabel?: string
}

export const ValidationHeader: React.FC<ValidationHeaderProps> = ({
  metadata,
  onNewValidation,
  onExport,
  onNavigateBack,
  breadcrumbLabel = 'Payroll',
  periodLabel = 'Pay period',
}) => {
  return (
    <HeaderContainer>
      <ValidationBreadcrumbs>
        <BreadcrumbAction onClick={onNavigateBack}>
          {breadcrumbLabel}
        </BreadcrumbAction>
        <BreadcrumbDivider variant="body2">&gt;</BreadcrumbDivider>
        <BreadcrumbCurrent variant="body2">Results</BreadcrumbCurrent>
      </ValidationBreadcrumbs>

      <HeaderContent>
        <Box>
          <Stack direction="column" spacing={1}>
            <PeriodInfo direction="row" spacing={1}>
              <PeriodGlyph />
              <PeriodValue variant="body1">
                {periodLabel}: {formatDate(metadata.weekStarting)} —{' '}
                {formatDate(metadata.weekEnding)}
              </PeriodValue>
            </PeriodInfo>

            <MetadataStack
              direction={{ xs: 'column', md: 'row' }}
              spacing={{ xs: 0.5, md: 3 }}
            >
              {metadata.validatedAt && (
                <MetadataItem>
                  <MetadataIconGlyph>
                    <EventAvailableOutlinedIcon fontSize="inherit" />
                  </MetadataIconGlyph>
                  <Typography variant="caption">
                    Validated on: {metadata.validatedAt}
                  </Typography>
                </MetadataItem>
              )}
              {metadata.validationId && (
                <MetadataItem>
                  <MetadataIconGlyph>
                    <FingerprintOutlinedIcon fontSize="inherit" />
                  </MetadataIconGlyph>
                  <Typography variant="caption">
                    Validation ID: {metadata.validationId}
                  </Typography>
                </MetadataItem>
              )}
            </MetadataStack>
          </Stack>
        </Box>

        <HeaderActions>
          <ExportAction
            variant="contained"
            startIcon={<FileDownloadOutlinedIcon />}
            onClick={onExport}
          >
            Export CSV
          </ExportAction>
          <NewValidationAction
            variant="outlined"
            onClick={onNewValidation}
            startIcon={<AddOutlinedIcon />}
          >
            New Validation
          </NewValidationAction>
        </HeaderActions>
      </HeaderContent>
    </HeaderContainer>
  )
}

// Payroll-specific issue row component. Renders a single ValidationIssue
// inside a CategoryAccordion body.
//   - warning: yellow Alert box (shown when a check was disabled)
//   - description: two-line structured text built from 4 category templates
// The backend guarantees these are mutually exclusive per issue.
// Left border color reflects severity (1-4).

import React from 'react'
import { Box, Typography, Chip, Alert, alpha } from '@mui/material'
import { useTheme } from '@mui/material/styles'
import { styled } from '@/styles/styled'
import { formatMoney } from '@/shared/compliance-check'
import type { ValidationIssue } from '../types/payrollValidation.types'
import { severityConfig } from './payrollCategoryConfig'
import { buildDescriptionLines } from './payrollDescriptionTemplates'

// --- Styled components ---

const RowContainer = styled(Box, {
  shouldForwardProp: prop => prop !== 'severityColor',
})<{ severityColor: string }>(({ theme, severityColor }) => ({
  padding: theme.spacing(2, 2.5),
  borderLeft: `3px solid ${severityColor}`,
  borderBottom: `1px solid ${theme.palette.background.default}`,
  backgroundColor: theme.palette.background.paper,
  '&:last-child': {
    borderBottom: 'none',
  },
}))

const HeaderRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  flexWrap: 'wrap',
  gap: theme.spacing(1),
  marginBottom: theme.spacing(1),
}))

const EmployeeName = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.subtitle2.fontWeight,
  color: theme.palette.text.primary,
}))

const EmployeeId = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
}))

const ImpactAmount = styled(Typography)(({ theme }) => ({
  marginLeft: 'auto',
  fontWeight: theme.typography.subtitle2.fontWeight,
  color: theme.palette.error.main,
}))

const DescriptionLine = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  lineHeight: 1.6,
}))

const SeverityChip = styled(Chip, {
  shouldForwardProp: prop => prop !== 'chipColor',
})<{ chipColor: string }>(({ chipColor }) => ({
  height: 22,
  fontSize: 11,
  fontWeight: 600,
  backgroundColor: alpha(chipColor, 0.1),
  borderColor: alpha(chipColor, 0.3),
  '& .MuiChip-label': { color: chipColor },
}))

// --- Component ---

interface PayrollIssueRowProps {
  issue: ValidationIssue
}

export const PayrollIssueRow: React.FC<PayrollIssueRowProps> = ({ issue }) => {
  const theme = useTheme()

  const sev = severityConfig[issue.severity]
  const severityColor = theme.palette[sev.paletteKey][sev.paletteTone]
  const descLines = buildDescriptionLines(issue)

  return (
    <RowContainer severityColor={severityColor}>
      <HeaderRow>
        <EmployeeName variant="body2">{issue.employeeName}</EmployeeName>
        <EmployeeId variant="caption">{issue.employeeId}</EmployeeId>
        <SeverityChip
          label={sev.label}
          size="small"
          variant="outlined"
          chipColor={severityColor}
        />
        {issue.impactAmount > 0 && (
          <ImpactAmount variant="body2">
            {formatMoney(issue.impactAmount)}
          </ImpactAmount>
        )}
      </HeaderRow>

      {issue.warning && (
        <Alert severity="warning" variant="outlined">
          {issue.warning}
        </Alert>
      )}

      {descLines && (
        <Box>
          <DescriptionLine variant="body2">{descLines[0]}</DescriptionLine>
          <DescriptionLine variant="body2">{descLines[1]}</DescriptionLine>
        </Box>
      )}
    </RowContainer>
  )
}

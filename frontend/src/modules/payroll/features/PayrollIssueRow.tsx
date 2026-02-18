// Payroll-specific issue row component. Renders a single ValidationIssue
// inside a CategoryAccordion body. Two rendering paths:
//   - warning !== null  → yellow Alert box with backend-provided message
//   - description !== null → two-line structured text built from 4 templates
//     (BaseRate / PenaltyRate / Superannuation / CasualLoading)
// Left border color reflects severity (1-4).

import React from 'react'
import { Box, Typography, Chip, Alert, alpha } from '@mui/material'
import { useTheme } from '@mui/material/styles'
import { styled } from '@/styles/styled'
import { formatMoney } from '@/shared/compliance-check'
import type { ValidationIssue } from '../types/payrollValidation.types'
import { severityConfig } from './payrollCategoryConfig'

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
  color: chipColor,
  backgroundColor: alpha(chipColor, 0.1),
  borderColor: alpha(chipColor, 0.3),
}))

// --- Description template builders ---

function fmt(n: number): string {
  return n.toFixed(2)
}

function buildDescriptionLines(
  issue: ValidationIssue
): [string, string] | null {
  const d = issue.description
  if (!d) return null

  const diff = fmt(d.expectedValue - d.actualValue)

  switch (issue.categoryType) {
    case 'BaseRate':
      return [
        `Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr (${d.contextLabel})`,
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`,
      ]
    case 'PenaltyRate':
      return [
        `${d.contextLabel}: Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr`,
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`,
      ]
    case 'Superannuation':
      return [
        `Super paid $${fmt(d.actualValue)}, should be $${fmt(d.expectedValue)} (${d.contextLabel} gross)`,
        `Underpayment: $${fmt(issue.impactAmount)}`,
      ]
    case 'CasualLoading':
      return [
        `Casual employee: Paid $${fmt(d.actualValue)}/hr, should be $${fmt(d.expectedValue)}/hr (${d.contextLabel})`,
        `Underpayment: $${fmt(issue.impactAmount)} (${fmt(d.affectedUnits)} hours @ $${diff}/hr shortfall)`,
      ]
  }
}

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

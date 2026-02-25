// Payroll-specific issue row component. Renders a single ValidationIssue
// inside a CategoryAccordion body.
//   - warning: yellow alert card (shown when a check was disabled)
//   - description: rich text line + alert card built from category templates
// The backend guarantees these are mutually exclusive per issue.

import React, { useState } from 'react'
import { Box, Typography, Chip, Button, alpha } from '@mui/material'
import { useTheme } from '@mui/material/styles'
import HelpOutlineOutlinedIcon from '@mui/icons-material/HelpOutlineOutlined'
import WarningAmberRounded from '@mui/icons-material/WarningAmberRounded'
import { styled } from '@/styles/styled'
import { GuidanceModal } from '@/shared/compliance-check'
import type { ValidationIssue } from '../types/payrollValidation.types'
import { severityConfig } from './payrollCategoryConfig'
import { buildDescriptionLines } from './payrollDescriptionTemplates'

// --- Styled components ---

const RowContainer = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2, 2.5),
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
  color: theme.palette.text.disabled,
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

const ActionButton = styled(Button)(({ theme }) => ({
  marginLeft: 'auto',
  color: theme.palette.text.primary,
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: theme.fairworkly.radius.sm,
  padding: theme.spacing(0.75, 2),
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.button.fontWeight,
  whiteSpace: 'nowrap',
  flexShrink: 0,
  [theme.breakpoints.between('sm', 'md')]: {
    padding: theme.spacing(0.5, 1),
    fontSize: '0.7rem',
  },
  [theme.breakpoints.down('sm')]: {
    width: '100%',
    marginLeft: 0,
  },
  '&:hover': {
    backgroundColor: theme.palette.background.default,
    borderColor: theme.palette.text.disabled,
  },
}))

const DescriptionLine = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  lineHeight: 1.6,
  marginBottom: theme.spacing(1.5),
}))

const BoldValue = styled('span')({
  fontWeight: 700,
})

const ExpectedValue = styled('span')(({ theme }) => ({
  fontWeight: 700,
  color: theme.palette.success.main,
}))

const IssueAlert = styled(Box, {
  shouldForwardProp: prop => prop !== 'alertColor',
})<{ alertColor: string }>(({ theme, alertColor }) => ({
  backgroundColor: alpha(alertColor, 0.08),
  borderLeft: `4px solid ${alertColor}`,
  borderRadius: theme.fairworkly.radius.sm,
  padding: theme.spacing(1.5),
  display: 'flex',
  alignItems: 'flex-start',
  gap: theme.spacing(1.5),
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(2),
  },
}))

const AlertIcon = styled(WarningAmberRounded, {
  shouldForwardProp: prop => prop !== 'alertColor',
})<{ alertColor: string }>(({ alertColor }) => ({
  color: alertColor,
}))

const AlertText = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.caption.fontWeight,
  color: theme.palette.text.secondary,
}))

const AlertBoldText = styled('span')(({ theme }) => ({
  fontWeight: theme.typography.subtitle2.fontWeight,
  color: theme.palette.text.primary,
  marginRight: theme.spacing(1),
}))

// --- Helpers ---

function renderRichLine(
  line: string,
  actualValue: number,
  expectedValue: number
): React.ReactNode {
  const actualStr = `$${actualValue.toFixed(2)}`
  const expectedStr = `$${expectedValue.toFixed(2)}`

  const actualIdx = line.indexOf(actualStr)
  if (actualIdx === -1) return line

  const expectedIdx = line.indexOf(expectedStr, actualIdx + actualStr.length)
  if (expectedIdx === -1) return line

  return (
    <>
      {line.slice(0, actualIdx)}
      <BoldValue>{actualStr}</BoldValue>
      {line.slice(actualIdx + actualStr.length, expectedIdx)}
      <ExpectedValue>{expectedStr}</ExpectedValue>
      {line.slice(expectedIdx + expectedStr.length)}
    </>
  )
}

function renderAlertLine(line: string): React.ReactNode {
  const match = line.match(/^(Underpayment: \$[\d.]+)(.*)/)
  if (!match) return line
  return (
    <>
      <AlertBoldText>{match[1]}</AlertBoldText>
      {match[2]}
    </>
  )
}

// --- Component ---

interface PayrollIssueRowProps {
  issue: ValidationIssue
}

export const PayrollIssueRow: React.FC<PayrollIssueRowProps> = ({ issue }) => {
  const theme = useTheme()
  const [fixModalOpen, setFixModalOpen] = useState(false)

  const sev = severityConfig[issue.severity]
  const severityColor =
    sev.color ?? theme.palette[sev.paletteKey][sev.paletteTone]
  const descLines = buildDescriptionLines(issue)
  const d = issue.description

  const warnSev = severityConfig[2]
  const warningColor =
    warnSev.color ?? theme.palette[warnSev.paletteKey][warnSev.paletteTone]

  return (
    <>
      <RowContainer>
        <HeaderRow>
          <EmployeeName variant="body2">{issue.employeeName}</EmployeeName>
          <EmployeeId variant="caption">ID: {issue.employeeId}</EmployeeId>
          <SeverityChip
            label={sev.label}
            size="small"
            variant="outlined"
            chipColor={severityColor}
          />
          <ActionButton
            startIcon={<HelpOutlineOutlinedIcon />}
            onClick={() => setFixModalOpen(true)}
          >
            How to Fix
          </ActionButton>
        </HeaderRow>

        {d && descLines && (
          <DescriptionLine variant="body2">
            {renderRichLine(descLines[0], d.actualValue, d.expectedValue)}
          </DescriptionLine>
        )}

        {d && descLines && (
          <IssueAlert alertColor={severityColor}>
            <AlertIcon alertColor={severityColor} />
            <AlertText variant="body2">
              {renderAlertLine(descLines[1])}
            </AlertText>
          </IssueAlert>
        )}

        {issue.warning && (
          <IssueAlert alertColor={warningColor}>
            <AlertIcon alertColor={warningColor} />
            <AlertText variant="body2">{issue.warning}</AlertText>
          </IssueAlert>
        )}
      </RowContainer>

      <GuidanceModal
        open={fixModalOpen}
        onClose={() => setFixModalOpen(false)}
      />
    </>
  )
}

import React, { useState } from 'react'
import {
  Box,
  Typography,
  Button,
  Checkbox,
  Stack,
  styled,
  alpha,
} from '@mui/material'
import ReportProblemOutlinedIcon from '@mui/icons-material/ReportProblemOutlined'
import HelpOutlineOutlinedIcon from '@mui/icons-material/HelpOutlineOutlined'
import type { IssueItem } from '../types/complianceCheck.type'
import { GuidanceModal } from './GuidanceModal'

export interface GuidanceContent {
  title?: string
  description?: string
  steps?: string[]
  afterStepsNote?: string
  footnote?: string
}

const IssueRowWrapper = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2),
  borderBottom: `1px solid ${theme.palette.background.default}`,
  display: 'flex',
  alignItems: 'flex-start',
  backgroundColor: theme.palette.background.paper,
  gap: theme.spacing(2),
  [theme.breakpoints.down('sm')]: {
    flexDirection: 'column',
  },
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(2.5),
  },
  [theme.breakpoints.up('md')]: {
    padding: theme.spacing(3),
  },
  '&:last-child': {
    borderBottom: 'none',
  },
}))

const IssueAlert = styled(Box)(({ theme }) => ({
  backgroundColor: alpha(theme.palette.error.main, 0.08),
  borderLeft: `4px solid ${theme.palette.error.main}`,
  borderRadius: theme.fairworkly.radius.sm,
  padding: theme.spacing(1.5),
  display: 'flex',
  alignItems: 'flex-start',
  gap: theme.spacing(1.5),
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(2),
  },
}))

const ActionButton = styled(Button)(({ theme }) => ({
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
  },
  '&:hover': {
    backgroundColor: theme.palette.background.default,
    borderColor: theme.palette.text.disabled,
  },
}))

const IssueCheckbox = styled(Checkbox)(({ theme }) => ({
  marginTop: theme.spacing(1),
  marginRight: theme.spacing(1),
  [theme.breakpoints.down('sm')]: {
    marginTop: 0,
  },
}))

const EmployeeName = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.h2.fontWeight,
  color: theme.palette.text.primary,
  whiteSpace: 'nowrap',
  overflow: 'hidden',
  textOverflow: 'ellipsis',
  minWidth: 0,
}))

const EmployeeId = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.body1.fontWeight,
  whiteSpace: 'nowrap',
  flexShrink: 0,
}))

const ExpectedValue = styled('span')(({ theme }) => ({
  color: theme.palette.success.main,
  fontWeight: theme.typography.h2.fontWeight,
}))

const VarianceLabel = styled('span')(({ theme }) => ({
  color: theme.palette.text.primary,
  fontWeight: theme.typography.h2.fontWeight,
  marginRight: theme.spacing(1),
}))

const IssueAlertIcon = styled(ReportProblemOutlinedIcon)(({ theme }) => ({
  color: theme.palette.error.main,
}))

const IssueAlertText = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.caption.fontWeight,
  color: theme.palette.text.secondary,
}))

const IssueDetails = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(2),
}))

const ActionsColumn = styled(Stack)(({ theme }) => ({
  marginLeft: theme.spacing(4),
  flexShrink: 0,
  [theme.breakpoints.between('sm', 'md')]: {
    marginLeft: theme.spacing(1),
  },
  [theme.breakpoints.down('sm')]: {
    marginLeft: 0,
    width: '100%',
  },
}))

const HeaderRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  marginBottom: theme.spacing(1),
  gap: theme.spacing(1),
}))

interface IssueRowProps {
  issue: IssueItem
  isSelected: boolean
  onToggleSelection: () => void
  guidance?: GuidanceContent
}

export const IssueRow: React.FC<IssueRowProps> = ({
  issue,
  isSelected,
  onToggleSelection,
  guidance,
}) => {
  const [fixModalOpen, setFixModalOpen] = useState(false)

  return (
    <>
      <IssueRowWrapper>
        <IssueCheckbox checked={isSelected} onChange={onToggleSelection} />
        <Box sx={{ flexGrow: 1, minWidth: 0 }}>
          <HeaderRow>
            <EmployeeName variant="subtitle1">{issue.name}</EmployeeName>
            <EmployeeId variant="body2" color="text.disabled">
              ID: {issue.empId}
            </EmployeeId>
          </HeaderRow>
          <IssueDetails variant="body2" color="text.primary">
            Actual: <strong>{issue.actualValue}</strong>, expected:{' '}
            <ExpectedValue>{issue.expectedValue}</ExpectedValue> — {issue.reason}
          </IssueDetails>

          <IssueAlert>
            <IssueAlertIcon />
            <IssueAlertText variant="body2">
              <VarianceLabel>Variance: {issue.variance}</VarianceLabel>
              {issue.breakdown}
            </IssueAlertText>
          </IssueAlert>
        </Box>
        <ActionsColumn direction="column" spacing={1}>
          <ActionButton
            startIcon={<HelpOutlineOutlinedIcon />}
            onClick={() => setFixModalOpen(true)}
          >
            How to Fix
          </ActionButton>
        </ActionsColumn>
      </IssueRowWrapper>

      <GuidanceModal
        open={fixModalOpen}
        onClose={() => setFixModalOpen(false)}
        {...(guidance ?? {})}
      />
    </>
  )
}

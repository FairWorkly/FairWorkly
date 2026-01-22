import React from 'react'
import { Box, Card, CardContent, styled } from '@mui/material'
import type { Theme } from '@mui/material/styles'
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline'
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline'
import WarningAmberIcon from '@mui/icons-material/WarningAmber'
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined'
import type {
  ValidationSummary,
  ValidationMetadata,
  IssueCategory,
} from '../types/complianceCheck.type'
import { ValidationHeader } from './ValidationHeader'
import { IssuesByCategory } from './IssuesByCategory'
import { exportComplianceCsv } from '../utils/formatters'
import { theme } from '@/styles/theme/theme'

const SummaryStatCard = styled(Card)(({ theme }) => ({
  borderRadius: theme.fairworkly.radius.xl,
  border: `1px solid ${theme.palette.background.default}`,
  boxShadow: theme.fairworkly.shadow.sm,
  transition: theme.transitions.create(['transform', 'box-shadow'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    borderColor: theme.palette.divider,
    transform: 'translateY(-2px)',
    boxShadow: theme.fairworkly.shadow.lg,
  },
}))

const StatContent = styled(CardContent)(({ theme }) => ({
  padding: theme.spacing(2),
  display: 'flex',
  flexDirection: 'column',
  height: '100%',
  '&:last-child': {
    paddingBottom: theme.spacing(2),
  },
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(2.5),
    '&:last-child': {
      paddingBottom: theme.spacing(2.5),
    },
  },
  [theme.breakpoints.up('md')]: {
    padding: theme.spacing(3),
    '&:last-child': {
      paddingBottom: theme.spacing(3),
    },
  },
}))

type StatusVariant = keyof Pick<
  Theme['palette'],
  'success' | 'error' | 'warning' | 'info'
>

const SummaryIconBox = styled(Box)(({ theme }) => ({
  width: theme.spacing(5),
  height: theme.spacing(5),
  borderRadius: theme.fairworkly.radius.md,
  backgroundColor: theme.palette.action.hover,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  marginBottom: theme.spacing(2),
  [theme.breakpoints.up('md')]: {
    width: theme.spacing(5.5),
    height: theme.spacing(5.5),
  },
}))

const StatValue = styled(Box, {
  shouldForwardProp: prop => prop !== 'valueColor',
})<{ valueColor: string }>(({ theme, valueColor }) => ({
  fontSize: theme.typography.h3.fontSize,
  fontWeight: theme.typography.h3.fontWeight,
  marginBottom: theme.spacing(1),
  color: valueColor,
}))

const StatLabel = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.uiLabel.fontSize,
  color: theme.palette.text.secondary,
  fontWeight: theme.typography.uiLabel.fontWeight,
}))

const StatsGrid = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '1fr',
  gap: theme.spacing(2),
  marginBottom: theme.spacing(4),
  [theme.breakpoints.up('sm')]: {
    gridTemplateColumns: 'repeat(2, 1fr)',
    gap: theme.spacing(3),
    marginBottom: theme.spacing(5),
  },
  [theme.breakpoints.up('md')]: {
    gridTemplateColumns: 'repeat(4, 1fr)',
    marginBottom: theme.spacing(6),
  },
}))

interface StatCardProps {
  value: string | number
  label: string
  variant: StatusVariant
  valueColor: string
}

const StatCard: React.FC<StatCardProps> = ({
  value,
  label,
  variant,
  valueColor,
}) => {
  const iconMap = {
    success: CheckCircleOutlineIcon,
    error: ErrorOutlineIcon,
    warning: WarningAmberIcon,
    info: InfoOutlinedIcon,
  }
  const Icon = iconMap[variant]

  return (
    <SummaryStatCard>
      <StatContent>
        <SummaryIconBox>
          <Icon fontSize="small" />
        </SummaryIconBox>
        <StatValue valueColor={valueColor}>{value}</StatValue>
        <StatLabel>{label}</StatLabel>
      </StatContent>
    </SummaryStatCard>
  )
}

interface ComplianceResultsProps {
  metadata: ValidationMetadata
  summary: ValidationSummary
  categories: IssueCategory[]
  onNewValidation: () => void
  onNavigateBack: () => void
  breadcrumbLabel?: string
  periodLabel?: string
  resultType?: 'payroll' | 'roster'
}

export const ComplianceResults: React.FC<ComplianceResultsProps> = ({
  metadata,
  summary,
  categories,
  onNewValidation,
  onNavigateBack,
  breadcrumbLabel = 'Payroll',
  periodLabel = 'Pay period',
  resultType = 'payroll',
}) => {
  const handleExport = () => {
    exportComplianceCsv(metadata, categories)
  }
  const stats =
    resultType === 'roster'
      ? [
          {
            value: summary.employeesCompliant,
            label: 'Employees Compliant',
            variant: 'success' as const,
            valueColor: theme.palette.success.main,
          },
          {
            value: summary.totalIssues,
            label: 'Total Issues Found',
            variant: 'error' as const,
            valueColor: theme.palette.error.main,
          },
          {
            value: summary.criticalIssuesCount,
            label: 'Critical Issues',
            variant: 'warning' as const,
            valueColor: theme.palette.warning.main,
          },
          {
            value: summary.employeesAffected,
            label: 'Employees Affected',
            variant: 'info' as const,
            valueColor: theme.palette.primary.main,
          },
        ]
      : [
          {
            value: summary.employeesCompliant,
            label: 'Employees Compliant',
            variant: 'success' as const,
            valueColor: theme.palette.success.main,
          },
          {
            value: summary.totalIssues,
            label: 'Total Issues Found',
            variant: 'error' as const,
            valueColor: theme.palette.error.main,
          },
          {
            value: summary.totalUnderpayment,
            label: 'Total Underpayment',
            variant: 'warning' as const,
            valueColor: theme.palette.warning.main,
          },
          {
            value: summary.employeesAffected,
            label: 'Employees Affected',
            variant: 'info' as const,
            valueColor: theme.palette.primary.main,
          },
        ]

  return (
    <Box>
      <ValidationHeader
        metadata={metadata}
        onNewValidation={onNewValidation}
        onExport={handleExport}
        onNavigateBack={onNavigateBack}
        breadcrumbLabel={breadcrumbLabel}
        periodLabel={periodLabel}
      />

      <StatsGrid>
        {stats.map(stat => (
          <StatCard
            key={stat.label}
            value={stat.value}
            label={stat.label}
            variant={stat.variant}
            valueColor={stat.valueColor}
          />
        ))}
      </StatsGrid>

      <IssuesByCategory
        categories={categories}
        onExport={handleExport}
        resultType={resultType}
      />
    </Box>
  )
}

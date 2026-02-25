// PayrollResults page — renders real PayrollValidationResult data.
//
// Data source priority:
//   1. Router state (navigate from PayrollUpload)
//   2. SessionStorage (browser refresh fallback)
//   3. Neither → redirect to /payroll/upload
//
// Composes shared building blocks (ValidationHeader, SummaryCards,
// CategoryAccordion) with payroll-specific issue rendering
// (PayrollIssueRow) and category/severity config.

import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { Box, Typography, Stack, Paper } from '@mui/material'
import { useTheme } from '@mui/material/styles'
import { styled } from '@/styles/styled'
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline'
import FactCheckOutlinedIcon from '@mui/icons-material/FactCheckOutlined'
import {
  ValidationHeader,
  SummaryCards,
  CategoryAccordion,
  formatMoney,
  formatDateTime,
} from '@/shared/compliance-check'
import type {
  ValidationMetadata,
  StatCardItem,
} from '@/shared/compliance-check'
import type { PayrollValidationResult } from '../types/payrollValidation.types'
import { categoryConfig } from '../features/payrollCategoryConfig'
import { PayrollIssueRow } from '../features/PayrollIssueRow'
import { exportPayrollCsv } from '../features/exportPayrollCsv'

// --- Styled components (page-level, no sx allowed in JSX) ---

const AllClearContainer = styled(Paper)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(6, 4),
  borderRadius: theme.fairworkly.radius.xl,
  backgroundColor: theme.palette.success.light,
  border: `1px solid ${theme.palette.success.main}`,
  textAlign: 'center',
  gap: theme.spacing(2),
}))

const AllClearIcon = styled(CheckCircleOutlineIcon)(({ theme }) => ({
  fontSize: 48,
  color: theme.palette.success.main,
}))

const AllClearTitle = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.h6.fontWeight,
  color: theme.palette.success.dark,
}))

const AllClearSubtitle = styled(Typography)(({ theme }) => ({
  color: theme.palette.success.dark,
}))

const IssuesWrapper = styled(Paper)(({ theme }) => ({
  marginTop: theme.spacing(2),
  padding: theme.spacing(2),
  borderRadius: theme.fairworkly.radius.xl,
  border: `1px solid ${theme.palette.divider}`,
  boxShadow: 'none',
  backgroundColor: theme.palette.background.paper,
  [theme.breakpoints.up('sm')]: {
    marginTop: theme.spacing(3),
    padding: theme.spacing(3),
  },
  [theme.breakpoints.up('md')]: {
    padding: theme.spacing(1),
  },
}))

const IssuesHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  marginBottom: theme.spacing(3),
  [theme.breakpoints.up('md')]: {
    marginBottom: theme.spacing(2),
  },
}))

const IssuesHeaderIcon = styled(FactCheckOutlinedIcon)(({ theme }) => ({
  color: theme.palette.text.primary,
}))

const CategoriesStack = styled(Stack)(({ theme }) => ({
  gap: theme.spacing(2),
}))

// --- Component ---

export function PayrollResults() {
  const location = useLocation()
  const navigate = useNavigate()
  const theme = useTheme()

  // Router state (from upload page) or sessionStorage (refresh fallback)
  const result: PayrollValidationResult | null =
    location.state?.result ??
    (() => {
      try {
        const cached = sessionStorage.getItem('payroll-validation-result')
        return cached ? JSON.parse(cached) : null
      } catch {
        sessionStorage.removeItem('payroll-validation-result')
        return null
      }
    })()

  // Expand/collapse state — first category open by default.
  // Must be declared before any early return to satisfy Rules of Hooks.
  const [expanded, setExpanded] = useState<Record<string, boolean>>(() => {
    const firstKey = result?.categories[0]?.key
    return firstKey ? { [firstKey]: true } : {}
  })

  const toggleCategory = (key: string) => {
    setExpanded(prev => ({ ...prev, [key]: !prev[key] }))
  }

  // No data → redirect to upload
  if (!result) {
    return <Navigate to="/payroll/upload" replace />
  }

  // ValidationHeader metadata
  const metadata: ValidationMetadata = {
    weekStarting: result.payPeriodStart,
    weekEnding: result.payPeriodEnd,
    validatedAt: formatDateTime(result.timestamp),
    validationId: result.validationId,
  }

  // SummaryCards stats
  const stats: StatCardItem[] = [
    {
      value: result.summary.passedCount,
      label: 'Employees Compliant',
      variant: 'success',
      valueColor: theme.palette.success.main,
    },
    {
      value: result.summary.totalIssues,
      label: 'Total Issues Found',
      variant: 'error',
      valueColor: theme.palette.error.main,
    },
    {
      value: formatMoney(result.summary.totalUnderpayment),
      label: 'Total Underpayment',
      variant: 'warning',
      valueColor: theme.palette.warning.main,
    },
    {
      value: result.summary.affectedEmployees,
      label: 'Employees Affected',
      variant: 'info',
      valueColor: theme.palette.primary.main,
    },
  ]

  const isPassed = result.status === 'Passed'

  return (
    <Box>
      <ValidationHeader
        metadata={metadata}
        onNewValidation={() => navigate('/payroll/upload')}
        onExport={() => exportPayrollCsv(result)}
        onNavigateBack={() => navigate('/payroll/upload')}
      />

      <SummaryCards items={stats} />

      {isPassed ? (
        <AllClearContainer elevation={0}>
          <AllClearIcon />
          <AllClearTitle variant="h6">All Clear</AllClearTitle>
          <AllClearSubtitle variant="body1">
            All employees are compliant. No issues found.
          </AllClearSubtitle>
        </AllClearContainer>
      ) : (
        <IssuesWrapper>
          <IssuesHeader>
            <IssuesHeaderIcon />
            <Typography variant="h6">Issues by Category</Typography>
          </IssuesHeader>

          <CategoriesStack>
            {result.categories
              .filter(cat => cat.affectedEmployeeCount > 0)
              .map(cat => {
                const config = categoryConfig[cat.key]
                if (!config) return null
                const Icon = config.icon
                const catIssues = result.issues.filter(
                  i => i.categoryType === cat.key
                )
                return (
                  <CategoryAccordion
                    key={cat.key}
                    title={config.title}
                    icon={<Icon fontSize="small" />}
                    iconColor={config.color}
                    employeeCount={cat.affectedEmployeeCount}
                    amountLabel={`${formatMoney(cat.totalUnderpayment)} underpayment`}
                    expanded={!!expanded[cat.key]}
                    onToggle={() => toggleCategory(cat.key)}
                  >
                    {catIssues.map(issue => (
                      <PayrollIssueRow key={issue.issueId} issue={issue} />
                    ))}
                  </CategoryAccordion>
                )
              })}
          </CategoriesStack>
        </IssuesWrapper>
      )}
    </Box>
  )
}

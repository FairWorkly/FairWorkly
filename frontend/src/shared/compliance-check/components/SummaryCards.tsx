// Shared building block: responsive stat card grid.
// Extracted from ComplianceResults — used by both payroll and roster
// results pages to display top-level summary numbers (e.g. compliant
// count, total issues, underpayment, affected employees).
// Consumer passes a StatCardItem[] — this component only handles layout
// and visual rendering, not data assembly.

import React from 'react'
import { Box, Card, CardContent } from '@mui/material'
import { styled, type Theme } from '@/styles/styled'
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline'
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline'
import WarningAmberIcon from '@mui/icons-material/WarningAmber'
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined'

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

type StatusVariant = keyof Pick<
  Theme['palette'],
  'success' | 'error' | 'warning' | 'info'
>

export interface StatCardItem {
  value: string | number
  label: string
  variant: StatusVariant
  valueColor: string
}

// Fixed mapping: variant → icon. Every stat card shows one of these
// four icons regardless of the domain (payroll / roster).
const iconMap = {
  success: CheckCircleOutlineIcon,
  error: ErrorOutlineIcon,
  warning: WarningAmberIcon,
  info: InfoOutlinedIcon,
}

const StatCard: React.FC<StatCardItem> = ({
  value,
  label,
  variant,
  valueColor,
}) => {
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

interface SummaryCardsProps {
  items: StatCardItem[]
}

export const SummaryCards: React.FC<SummaryCardsProps> = ({ items }) => (
  <StatsGrid>
    {items.map(item => (
      <StatCard key={item.label} {...item} />
    ))}
  </StatsGrid>
)

// Category and severity display config for payroll results.
// Maps backend enum values (CategoryType, Severity) to UI properties
// (title, color, icon, label) used by PayrollResults, PayrollIssueRow,
// and exportPayrollCsv.

import type React from 'react'
import AttachMoneyOutlined from '@mui/icons-material/AttachMoneyOutlined'
import GavelOutlined from '@mui/icons-material/GavelOutlined'
import PersonOutlined from '@mui/icons-material/PersonOutlined'
import AccountBalanceOutlined from '@mui/icons-material/AccountBalanceOutlined'
import type { CategoryType, Severity } from '../types/payrollValidation.types'

export interface CategoryDisplayConfig {
  title: string
  color: string
  icon: React.ElementType
}

export const categoryConfig: Record<CategoryType, CategoryDisplayConfig> = {
  BaseRate: {
    title: 'Base Rate Issues',
    color: '#ef4444',
    icon: AttachMoneyOutlined,
  },
  PenaltyRate: {
    title: 'Penalty Rate Issues',
    color: '#f97316',
    icon: GavelOutlined,
  },
  CasualLoading: {
    title: 'Casual Loading Issues',
    color: '#eab308',
    icon: PersonOutlined,
  },
  Superannuation: {
    title: 'Superannuation Issues',
    color: '#3b82f6',
    icon: AccountBalanceOutlined,
  },
}

export interface SeverityDisplayConfig {
  label: string
  paletteKey: 'info' | 'warning' | 'error'
  paletteTone: 'main' | 'dark'
}

export const severityConfig: Record<Severity, SeverityDisplayConfig> = {
  1: { label: 'Info', paletteKey: 'info', paletteTone: 'main' },
  2: { label: 'Warning', paletteKey: 'warning', paletteTone: 'main' },
  3: { label: 'Error', paletteKey: 'error', paletteTone: 'main' },
  4: { label: 'Critical', paletteKey: 'error', paletteTone: 'dark' },
}

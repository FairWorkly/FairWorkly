import {
  Box,
  Chip,
  Paper,
  Select,
  Switch,
  TableCell,
  TableRow,
  Typography,
} from '@mui/material'
import { styled } from '@/styles/styled'

// Card wrapper (same pattern as CompanyProfile)
export const TeamCard = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(3),
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.divider}`,
  transition: theme.transitions.create(['box-shadow', 'border-color'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    borderColor: theme.palette.primary.light,
    boxShadow: theme.fairworkly.shadow.md,
  },
}))

export const CardHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(3),
  paddingBottom: theme.spacing(2),
  borderBottom: `1px solid ${theme.palette.divider}`,
}))

export const CardTitle = styled(Typography)(({ theme }) => ({
  ...theme.typography.subtitle1,
  color: theme.palette.text.primary,
}))

export const MemberCount = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  color: theme.palette.text.secondary,
}))

// Table
export const StyledTableRow = styled(TableRow)(({ theme }) => ({
  '&:last-child td': { borderBottom: 0 },
  '&:hover': {
    backgroundColor: theme.palette.action.hover,
  },
}))

export const StyledTableCell = styled(TableCell)(({ theme }) => ({
  padding: theme.spacing(1.5, 2),
}))

export const HeaderCell = styled(TableCell)(({ theme }) => ({
  ...theme.typography.uiLabel,
  color: theme.palette.text.secondary,
  padding: theme.spacing(1.5, 2),
  borderBottom: `2px solid ${theme.palette.divider}`,
}))

export const RoleSelect = styled(Select)(({ theme }) => ({
  minWidth: theme.spacing(16),
  '& .MuiSelect-select': {
    padding: theme.spacing(0.75, 1.5),
  },
}))

export const StatusSwitch = styled(Switch)(() => ({}))

export const YouChip = styled(Chip)(({ theme }) => ({
  marginLeft: theme.spacing(1),
  height: theme.spacing(2.5),
  fontSize: theme.typography.caption.fontSize,
}))

export const LastLoginText = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  color: theme.palette.text.secondary,
}))

export const EmptyState = styled(Box)(({ theme }) => ({
  padding: theme.spacing(6),
  textAlign: 'center',
  color: theme.palette.text.secondary,
}))

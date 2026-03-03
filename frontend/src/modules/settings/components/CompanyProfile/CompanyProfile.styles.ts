import { Box, Button, Paper, Typography } from '@mui/material'
import { styled } from '@/styles/styled'

// Card
export const ProfileCard = styled(Paper)(({ theme }) => ({
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

export const CardContent = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2.5),
}))

// Form
export const FormRow = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: `${theme.spacing(22.5)} 1fr`,
  gap: theme.spacing(2),
  alignItems: 'start',

  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: '1fr',
    gap: theme.spacing(1),
  },
}))

export const FormField = styled(Box)(() => ({
  display: 'flex',
  flexDirection: 'column',
  width: '100%',
}))

export const FieldLabel = styled(Box)(({ theme }) => ({
  ...theme.typography.uiLabel,
  color: theme.palette.text.secondary,
  lineHeight: 1.6,
  paddingTop: theme.spacing(1.5),
}))

export const FieldValue = styled(Box)(({ theme }) => ({
  ...theme.typography.body1,
  color: theme.palette.text.primary,
  paddingTop: theme.spacing(1.5),
  minHeight: theme.spacing(5),
  '&:empty::before': {
    content: '"—"',
    color: theme.palette.text.disabled,
  },
}))

export const ErrorText = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.caption.fontSize,
  color: theme.palette.error.main,
  marginTop: theme.spacing(0.5),
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(0.5),
}))

// Buttons
export const ButtonContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(1.5),
  justifyContent: 'flex-end',
  marginTop: theme.spacing(1),
}))

const BaseActionButton = styled(Button)(({ theme }) => ({
  minWidth: theme.spacing(10),
  fontWeight: theme.typography.fontWeightMedium,
}))

export const EditButton = styled(BaseActionButton)(() => ({}))

export const CancelButton = styled(BaseActionButton)(({ theme }) => ({
  color: theme.palette.text.secondary,
  borderColor: theme.palette.divider,
  '&:hover': {
    borderColor: theme.palette.text.secondary,
    backgroundColor: theme.palette.action.hover,
  },
}))

export const SaveButton = styled(BaseActionButton)(({ theme }) => ({
  '&.Mui-disabled': {
    backgroundColor: theme.palette.action.disabledBackground,
    color: theme.palette.action.disabled,
  },
}))

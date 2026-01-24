import { Box, Button, Paper, Typography } from '@mui/material'
import { styled } from '@/styles/styled'


export const ProfileCard = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(3),
  borderRadius: `${theme.fairworkly.radius.lg}px`,
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

export const CardHeaderContent = styled(Box)(() => ({
  display: 'flex',
  flexDirection: 'column',
}))

export const CardTitle = styled(Typography)(({ theme }) => ({
  ...theme.typography.h6,
  color: theme.palette.text.primary,
  fontWeight: theme.typography.fontWeightBold,
}))

export const CardDescription = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  color: theme.palette.text.secondary,
  marginTop: theme.spacing(0.5),
}))


export const CardContent = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2.5),
}))


export const FormRow = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '180px 1fr',
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
  fontSize: theme.typography.body2.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  color: theme.palette.text.secondary,
  lineHeight: 1.6,
  paddingTop: theme.spacing(1.5),
}))


export const FieldValue = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.primary,
  lineHeight: 1.6,
  paddingTop: theme.spacing(1.5),
  minHeight: theme.spacing(5),

  // 空值显示占位符
  '&:empty::before': {
    content: '"—"',
    color: theme.palette.text.disabled,
  },
}))


export const LogoPlaceholder = styled(Box)(({ theme }) => ({
  width: 120,
  height: 120,
  borderRadius: `${theme.fairworkly.radius.md}px`,
  border: `2px dashed ${theme.palette.divider}`,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  backgroundColor: theme.palette.background.default,
  color: theme.palette.text.disabled,
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightMedium,
  transition: theme.transitions.create(['border-color', 'background-color'], {
    duration: theme.transitions.duration.short,
  }),

  '&:hover': {
    borderColor: theme.palette.primary.main,
    backgroundColor: theme.palette.action.hover,
    cursor: 'pointer',
  },
}))


export const ButtonContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(1.5),
  justifyContent: 'flex-end',
  marginTop: theme.spacing(1),
}))

export const EditButton = styled(Button)(({ theme }) => ({
  minWidth: theme.spacing(10),
  fontWeight: theme.typography.fontWeightMedium,
}))

export const CancelButton = styled(Button)(({ theme }) => ({
  minWidth: theme.spacing(10),
  color: theme.palette.text.secondary,
  borderColor: theme.palette.divider,
  fontWeight: theme.typography.fontWeightMedium,

  '&:hover': {
    borderColor: theme.palette.text.secondary,
    backgroundColor: theme.palette.action.hover,
  },
}))

export const SaveButton = styled(Button)(({ theme }) => ({
  minWidth: theme.spacing(10),
  fontWeight: theme.typography.fontWeightMedium,

  '&.Mui-disabled': {
    backgroundColor: theme.palette.action.disabledBackground,
    color: theme.palette.action.disabled,
  },
}))

export const BadgeContainer = styled(Box)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  padding: theme.spacing(0.5, 1.5),
  borderRadius: `${theme.fairworkly.radius.pill}px`,
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  textTransform: 'uppercase',
  letterSpacing: '0.05em',
}))


export const AwardItem = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  padding: theme.spacing(2),
  borderRadius: `${theme.fairworkly.radius.md}px`,
  backgroundColor: theme.palette.background.default,
  border: `1px solid ${theme.palette.divider}`,
  transition: theme.transitions.create(['background-color', 'border-color'], {
    duration: theme.transitions.duration.short,
  }),

  '&:hover': {
    backgroundColor: theme.palette.action.hover,
    borderColor: theme.palette.primary.light,
  },
}))


export const AwardInfo = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
}))


export const AwardMeta = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),
  alignItems: 'flex-end',
  fontSize: theme.typography.body2.fontSize,
  color: theme.palette.text.secondary,
}))


export const AddAwardButton = styled(Button)(({ theme }) => ({
  marginTop: theme.spacing(1),
  borderColor: theme.palette.divider,
  color: theme.palette.text.primary,
  fontWeight: theme.typography.fontWeightMedium,

  '&:hover': {
    borderColor: theme.palette.primary.main,
    backgroundColor: theme.palette.action.hover,
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
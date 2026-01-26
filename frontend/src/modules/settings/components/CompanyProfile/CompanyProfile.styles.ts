import { Box, Button, DialogActions, Paper, Typography } from '@mui/material'
import { styled } from '@/styles/styled'

// Card部分
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

const Column = styled(Box)(() => ({
  display: 'flex',
  flexDirection: 'column',
}))

export const CardHeaderContent = styled(Column)(() => ({}))

export const CardTitle = styled(Typography)(({ theme }) => ({
  ...theme.typography.h6,
  color: theme.palette.text.primary,
}))

export const CardDescription = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  color: theme.palette.text.secondary,
  marginTop: theme.spacing(0.5),
}))


export const CardContent = styled(Column)(({ theme }) => ({
  gap: theme.spacing(2.5),
}))



// Form部分
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

// Button部分
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

export const BadgeContainer = styled(Box)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  padding: theme.spacing(0.5, 1.5),
  borderRadius: theme.fairworkly.radius.lg,
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  textTransform: 'uppercase',
  letterSpacing: '0.05em',
}))

// Award部分
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

export const EmptyStateText = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  color: theme.palette.text.secondary,
  paddingTop: theme.spacing(2),
  paddingBottom: theme.spacing(2),
}))

export const AwardTitleText = styled(Typography)(({ theme }) => ({
  ...theme.typography.subtitle2,
  color: theme.palette.text.primary,
  overflow: 'hidden',
  textOverflow: 'ellipsis',
  whiteSpace: 'nowrap',
}))

export const AwardMetaText = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  color: theme.palette.text.primary,
}))

export const AwardDateText = styled(Typography)(({ theme }) => ({
  ...theme.typography.caption,
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


// Dialog部分
export const DialogHeader = styled(Box)(({ theme }) => ({
  padding: theme.spacing(3, 3, 0),
}))

export const DialogBody = styled(Column)(({ theme }) => ({
  gap: theme.spacing(3),
  paddingTop: theme.spacing(2),
  paddingBottom: theme.spacing(2),
}))

export const FieldBlock = styled(Column)(({ theme }) => ({
  gap: theme.spacing(1),
}))

export const DialogFooter = styled(DialogActions)(({ theme }) => ({
  paddingLeft: theme.spacing(3),
  paddingRight: theme.spacing(3),
  paddingBottom: theme.spacing(3),
}))
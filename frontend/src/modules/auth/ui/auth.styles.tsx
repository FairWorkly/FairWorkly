import Box from '@mui/material/Box'
import Typography from '@mui/material/Typography'
import IconButton from '@mui/material/IconButton'
import SvgIcon from '@mui/material/SvgIcon'
import { styled } from '@/styles/styled'

export const GoogleButton = styled('button')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  gap: theme.spacing(1),
  width: '100%',
  padding: theme.spacing(1.5),
  borderRadius: theme.shape.borderRadius,
  fontSize: theme.typography.body1.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  cursor: 'pointer',
  background: theme.palette.background.paper,
  color: theme.palette.text.primary,
  border: `1px solid ${theme.palette.divider}`,
  transition: theme.transitions.create(['border-color', 'background']),
  '&:hover': {
    borderColor: theme.palette.primary.main,
    background: theme.fairworkly.effect.primaryGlow,
  },
}))

export const FormDivider = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  margin: theme.spacing(3, 0),
  '&::before, &::after': {
    content: '""',
    flex: 1,
    height: 1,
    background: theme.palette.divider,
  },
}))

export const FormDividerText = styled(Typography)(({ theme }) => ({
  padding: theme.spacing(0, 2),
  fontSize: theme.typography.caption.fontSize,
  color: theme.palette.text.disabled,
}))

export const FormRow = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '1fr 1fr',
  gap: theme.spacing(2),
  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: '1fr',
  },
}))

export const FormOptions = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(3),
}))

export const FormLink = styled('button')(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  color: theme.palette.primary.main,
  background: 'none',
  border: 'none',
  cursor: 'pointer',
  fontWeight: theme.typography.fontWeightMedium,
  '&:hover': {
    textDecoration: 'underline',
  },
}))

export const SubmitButton = styled('button')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  gap: theme.spacing(1),
  width: '100%',
  padding: theme.spacing(1.5),
  borderRadius: theme.shape.borderRadius,
  fontSize: theme.typography.body1.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  cursor: 'pointer',
  background: theme.fairworkly.gradient.primary,
  color: theme.palette.common.white,
  border: 'none',
  boxShadow: theme.fairworkly.shadow.primaryButton,
  transition: theme.transitions.create(['box-shadow', 'transform']),
  '&:hover': {
    boxShadow: theme.fairworkly.shadow.primaryButtonHover,
    transform: 'translateY(-2px)',
  },
}))

export const FormTerms = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.caption.fontSize,
  color: theme.palette.text.disabled,
  textAlign: 'center',
  marginTop: theme.spacing(3),
  lineHeight: theme.typography.body2.lineHeight,
  '& a': {
    color: theme.palette.primary.main,
    textDecoration: 'none',
    '&:hover': {
      textDecoration: 'underline',
    },
  },
}))

export const StrengthBar = styled(Box)(({ theme }) => ({
  height: theme.spacing(0.5),
  background: theme.palette.divider,
  borderRadius: theme.spacing(0.25),
  overflow: 'hidden',
  marginTop: theme.spacing(1),
  marginBottom: theme.spacing(0.5),
}))

const strengthConfig = {
  weak: { width: 1 / 3, color: 'error' },
  medium: { width: 2 / 3, color: 'warning' },
  strong: { width: 1, color: 'success' },
  '': { width: 0, color: 'transparent' },
} as const

type StrengthLevel = keyof typeof strengthConfig

export const StrengthFill = styled(Box)<{ strength: StrengthLevel }>(({
  theme,
  strength,
}) => {
  const config = strengthConfig[strength]
  return {
    height: '100%',
    width: `${config.width * 100}%`,
    transition: theme.transitions.create('width'),
    background:
      config.color === 'transparent'
        ? 'transparent'
        : theme.palette[config.color].main,
  }
})

export const StrengthText = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.caption.fontSize,
  color: theme.palette.text.disabled,
}))

export const ModalContent = styled(Box)(({ theme }) => ({
  padding: theme.spacing(5),
  position: 'relative',
}))

export const ModalCloseButton = styled(IconButton)(({ theme }) => ({
  position: 'absolute',
  top: theme.spacing(2),
  right: theme.spacing(2),
  background: theme.palette.background.default,
  color: theme.palette.text.disabled,
  '&:hover': {
    background: theme.palette.divider,
    color: theme.palette.text.primary,
  },
}))

const ModalIconBase = styled(Box)(({ theme }) => ({
  width: theme.spacing(8),
  height: theme.spacing(8),
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  margin: '0 auto',
  marginBottom: theme.spacing(3),
  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(4),
  },
}))

export const ModalIcon = styled(ModalIconBase)(({ theme }) => ({
  borderRadius: theme.spacing(2),
  background: theme.fairworkly.effect.primaryGlow,
  color: theme.palette.primary.main,
}))

export const ModalSuccessIcon = styled(ModalIconBase)(({ theme }) => ({
  borderRadius: '50%',
  background: `${theme.palette.success.main}1A`,
  color: theme.palette.success.main,
}))

export const ModalTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h4.fontSize,
  fontWeight: theme.typography.h4.fontWeight,
  textAlign: 'center',
  marginBottom: theme.spacing(1),
}))

export const ModalSubtitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.secondary,
  textAlign: 'center',
  marginBottom: theme.spacing(3),
  lineHeight: theme.typography.body1.lineHeight,
}))

export const ModalFooter = styled('div')(({ theme }) => ({
  textAlign: 'center',
  marginTop: theme.spacing(3),
}))

export const ModalBody = styled('div')({
  width: '100%',
})

export const FormActions = styled('div')(({ theme }) => ({
  marginTop: theme.spacing(3),
}))

export const AuthFormContainer = styled('form')({
  width: '100%',
})

export const AuthFieldset = styled('fieldset')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2.5),
  border: 0,
  margin: 0,
  padding: 0,
  minInlineSize: 0,
}))

export const RememberMeLabel = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
}))

export const AuthHeader = styled('header')(({ theme }) => ({
  textAlign: 'center',
  marginBottom: theme.spacing(4),
}))

export const AuthTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h3.fontSize,
  fontWeight: theme.typography.h3.fontWeight,
  marginBottom: theme.spacing(0.5),
  color: theme.palette.text.primary,
}))

export const AuthSubtitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.secondary,
}))

export const AuthTabList = styled('nav')(({ theme }) => ({
  display: 'flex',
  background: theme.palette.background.paper,
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: theme.shape.borderRadius,
  padding: theme.spacing(0.5),
  marginBottom: theme.spacing(4),
}))

export const AuthTabButton = styled('button')<{ active: boolean }>(
  ({ theme, active }) => ({
    flex: 1,
    padding: theme.spacing(1.5),
    border: 'none',
    background: active ? theme.fairworkly.gradient.primary : 'transparent',
    color: active ? theme.palette.common.white : theme.palette.text.secondary,
    fontSize: theme.typography.body1.fontSize,
    fontWeight: theme.typography.fontWeightBold,
    cursor: 'pointer',
    borderRadius: theme.spacing(1),
    transition: theme.transitions.create(['background', 'color']),
    '&:not(:disabled):hover': {
      ...(!active && {
        color: theme.palette.text.primary,
        background: theme.palette.background.default,
      }),
    },
  })
)

export const GoogleIcon = () => (
  <SvgIcon viewBox="0 0 24 24" aria-hidden="true">
    <path
      fill="#4285F4"
      d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
    />
    <path
      fill="#34A853"
      d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
    />
    <path
      fill="#FBBC05"
      d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
    />
    <path
      fill="#EA4335"
      d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
    />
  </SvgIcon>
)

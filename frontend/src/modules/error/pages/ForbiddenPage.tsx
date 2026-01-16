import { Link } from 'react-router-dom'
import { Box, Typography } from '@mui/material'
import { Block as BlockIcon } from '@mui/icons-material'
import { styled } from '@/styles/styled'

const ErrorShell = styled(Box)(({ theme }) => ({
  minHeight: '100vh',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  backgroundColor: theme.palette.background.default,
}))

const ErrorContainer = styled(Box)(({ theme }) => ({
  textAlign: 'center',
  maxWidth: theme.spacing(62.5),
  padding: theme.spacing(0, 3),
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  gap: theme.spacing(3),
}))

const ErrorIcon = styled(BlockIcon)(({ theme }) => ({
  fontSize: theme.spacing(10),
  color: theme.palette.error.main,
}))

const ErrorCode = styled(Typography)(({ theme }) => ({
  fontSize: theme.spacing(12),
  fontWeight: theme.typography.h1.fontWeight,
  color: theme.palette.text.primary,
}))

const ErrorTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h5.fontSize,
  fontWeight: theme.typography.h5.fontWeight,
  color: theme.palette.text.secondary,
}))

const ErrorMessage = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.secondary,
}))

const ErrorButton = styled(Link)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(1.5, 3),
  fontSize: theme.typography.button.fontSize,
  fontWeight: theme.typography.button.fontWeight,
  textTransform: theme.typography.button.textTransform as 'uppercase',
  textDecoration: 'none',
  borderRadius: theme.shape.borderRadius,
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  boxShadow: theme.shadows[2],
  transition: theme.transitions.create(['background-color', 'box-shadow'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    backgroundColor: theme.palette.primary.dark,
    boxShadow: theme.shadows[4],
  },
}))

export function ForbiddenPage() {
  return (
    <ErrorShell>
      <ErrorContainer>
        <ErrorIcon />

        <ErrorCode variant="h1">403</ErrorCode>

        <ErrorTitle variant="h5">Access Forbidden</ErrorTitle>

        <ErrorMessage variant="body1">
          You don't have permission to access this page. Please contact your administrator if you believe this is an error.
        </ErrorMessage>

        <ErrorButton to="/">Back to Home</ErrorButton>
      </ErrorContainer>
    </ErrorShell>
  )
}

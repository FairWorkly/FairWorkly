import { Link } from 'react-router-dom'
import { Box, Typography } from '@mui/material'
import { SearchOff as SearchOffIcon } from '@mui/icons-material'
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
}))

const ErrorIcon = styled(SearchOffIcon)(({ theme }) => ({
  fontSize: theme.spacing(10),
  color: theme.palette.warning.main,
  marginBottom: theme.spacing(2),
}))

const ErrorCode = styled(Typography)(({ theme }) => ({
  fontSize: theme.spacing(12),
  fontWeight: theme.typography.h1.fontWeight,
  color: theme.palette.text.primary,
  marginBottom: theme.spacing(2),
}))

const ErrorTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h5.fontSize,
  fontWeight: theme.typography.h5.fontWeight,
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(1),
}))

const ErrorMessage = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(4),
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

export default function NotFoundPage() {
  return (
    <ErrorShell>
      <ErrorContainer>
        <ErrorIcon />

        <ErrorCode variant="h1">404</ErrorCode>

        <ErrorTitle variant="h5">Page Not Found</ErrorTitle>

        <ErrorMessage variant="body1">The page you're looking for doesn't exist.</ErrorMessage>

        <ErrorButton to="/">Back to Home</ErrorButton>
      </ErrorContainer>
    </ErrorShell>
  )
}

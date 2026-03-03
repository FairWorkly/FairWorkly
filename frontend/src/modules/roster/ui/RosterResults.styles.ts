import { Box } from '@mui/material'
import { styled } from '@/styles/styled'

export const LoadingContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  paddingTop: theme.spacing(8),
  paddingBottom: theme.spacing(8),
  gap: theme.spacing(2),
}))

export const ErrorContainer = styled(Box)(({ theme }) => ({
  paddingTop: theme.spacing(4),
  paddingBottom: theme.spacing(4),
}))

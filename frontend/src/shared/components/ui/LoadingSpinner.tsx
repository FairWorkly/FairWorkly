import { CircularProgress, Typography } from '@mui/material'
import { styled } from '@/styles/styled'

const SpinnerShell = styled('div')(({ theme }) => ({
  minHeight: '100vh',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  backgroundColor: theme.palette.background.default,
}))

const SpinnerContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  gap: theme.spacing(2),
}))

const SpinnerText = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.secondary,
}))

export function LoadingSpinner() {
  return (
    <SpinnerShell>
      <SpinnerContainer>
        <CircularProgress />
        <SpinnerText>Loading...</SpinnerText>
      </SpinnerContainer>
    </SpinnerShell>
  )
}

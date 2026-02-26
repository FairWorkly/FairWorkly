import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import { useAuth } from '@/modules/auth/hooks/useAuth'

const HeaderContainer = styled('header')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
}))

export const WelcomeHeader = () => {
  const { user } = useAuth()
  const displayName = user?.name ?? 'there'

  return (
    <HeaderContainer>
      <Typography variant="h5" component="h1">
        Welcome back, {displayName}!
      </Typography>
    </HeaderContainer>
  )
}

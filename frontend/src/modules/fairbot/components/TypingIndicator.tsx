import { styled } from '@/styles/styled'
import { keyframes } from '@mui/material/styles'
import Typography from '@mui/material/Typography'

interface TypingIndicatorProps {
  isVisible: boolean
}

const bounce = keyframes`
  0%, 80%, 100% { transform: scale(0.6); opacity: 0.4; }
  40% { transform: scale(1); opacity: 1; }
`

const TypingContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
}))

const Dots = styled('div')(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(0.5),
}))

const Dot = styled('span')<{ delay: number }>(({ theme, delay }) => ({
  width: theme.spacing(0.75),
  height: theme.spacing(0.75),
  borderRadius: '50%',
  backgroundColor: theme.palette.text.secondary,
  animation: `${bounce} 1200ms infinite`,
  animationDelay: `${delay}ms`,
}))

export const TypingIndicator = ({ isVisible }: TypingIndicatorProps) => {
  if (!isVisible) {
    return null
  }

  return (
    <TypingContainer aria-live="polite">
      <Typography variant="caption">FairBot is thinking...</Typography>
      <Dots>
        <Dot delay={0} />
        <Dot delay={120} />
        <Dot delay={240} />
      </Dots>
    </TypingContainer>
  )
}

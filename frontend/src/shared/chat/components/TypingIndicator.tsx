import { styled } from '@/styles/styled'
import { keyframes } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import {
  CHAT_DEFAULT_LABELS,
  CHAT_TYPING_UI,
} from '../constants/chat.constants'

export interface TypingIndicatorProps {
  isVisible: boolean
  loadingMessage?: string
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

const Dots = styled('div')({
  display: 'flex',
  gap: `${CHAT_TYPING_UI.DOT_GAP}px`,
})

const Dot = styled('span')<{
  delay: number
}>(({ theme, delay }) => ({
  width: `${CHAT_TYPING_UI.DOT_SIZE}px`,
  height: `${CHAT_TYPING_UI.DOT_SIZE}px`,
  borderRadius: '50%',
  backgroundColor: theme.palette.text.secondary,
  animation: `${bounce} ${CHAT_TYPING_UI.DOT_ANIMATION_MS}ms infinite`,
  animationDelay: `${delay}ms`,
}))

export const TypingIndicator = ({
  isVisible,
  loadingMessage = CHAT_DEFAULT_LABELS.LOADING_MESSAGE,
}: TypingIndicatorProps) => {
  if (!isVisible) {
    return null
  }

  return (
    <TypingContainer aria-live="polite">
      <Typography variant="caption">{loadingMessage}</Typography>
      <Dots>
        <Dot delay={0} />
        <Dot delay={CHAT_TYPING_UI.DELAY_SHORT_MS} />
        <Dot delay={CHAT_TYPING_UI.DELAY_LONG_MS} />
      </Dots>
    </TypingContainer>
  )
}

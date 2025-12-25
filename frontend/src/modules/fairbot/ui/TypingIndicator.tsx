import { styled, keyframes } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import {
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
  FAIRBOT_NUMBERS,
  FAIRBOT_TYPING_UI,
} from '../constants/fairbot.constants'

interface TypingIndicatorProps {
  isVisible: boolean
}

const bounce = keyframes`
  0%, 80%, 100% { transform: scale(0.6); opacity: 0.4; }
  40% { transform: scale(1); opacity: 1; }
`

const TypingContainer = styled('div')({
  display: 'flex',
  alignItems: 'center',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_STACK_GAP}px`,
})

const Dots = styled('div')({
  display: 'flex',
  gap: `${FAIRBOT_TYPING_UI.DOT_GAP}px`,
})

const Dot = styled('span')<{
  delay: number
}>(({ theme, delay }) => ({
  width: `${FAIRBOT_TYPING_UI.DOT_SIZE}px`,
  height: `${FAIRBOT_TYPING_UI.DOT_SIZE}px`,
  borderRadius: '50%',
  backgroundColor: theme.palette.text.secondary,
  animation: `${bounce} ${FAIRBOT_TYPING_UI.DOT_ANIMATION_MS}ms infinite`,
  animationDelay: `${delay}ms`,
}))

export const TypingIndicator = ({ isVisible }: TypingIndicatorProps) => {
  if (!isVisible) {
    return null
  }

  return (
    <TypingContainer aria-live="polite">
      <Typography variant="caption">{FAIRBOT_LABELS.LOADING_MESSAGE}</Typography>
      <Dots>
        <Dot delay={FAIRBOT_NUMBERS.ZERO} />
        <Dot delay={FAIRBOT_TYPING_UI.DELAY_SHORT_MS} />
        <Dot delay={FAIRBOT_TYPING_UI.DELAY_LONG_MS} />
      </Dots>
    </TypingContainer>
  )
}

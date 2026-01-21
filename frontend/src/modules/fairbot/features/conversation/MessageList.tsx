import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_NUMBERS,
} from '../../constants/fairbot.constants'
import type { FairBotMessage } from '../../types/fairbot.types'
import { MessageBubble } from '../../ui/MessageBubble'
import { TypingIndicator } from '../../ui/TypingIndicator'

interface MessageListProps {
  messages: FairBotMessage[]
  isTyping: boolean
}

const ListContainer = styled('section')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2),
  padding: theme.spacing(1),
}))

const ListHeader = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
}))

const MessageStack = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2),
}))

// Renders message bubbles and the typing indicator for active responses.
export const MessageList = ({ messages, isTyping }: MessageListProps) => {
  const hasMessages = messages.length > FAIRBOT_NUMBERS.ZERO

  return (
    <ListContainer aria-label={FAIRBOT_ARIA.MESSAGE_LIST}>
      <ListHeader variant="caption">{FAIRBOT_LABELS.MESSAGE_LIST_HEADING}</ListHeader>
      <MessageStack>
        {messages.map((message) => (
          <MessageBubble key={message.id} message={message} />
        ))}
        <TypingIndicator isVisible={isTyping && hasMessages} />
      </MessageStack>
    </ListContainer>
  )
}

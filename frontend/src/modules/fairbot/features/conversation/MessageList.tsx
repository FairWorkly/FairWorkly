import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
  FAIRBOT_NUMBERS,
} from '../../constants/fairbot.constants'
import type { FairBotMessage } from '../../types/fairbot.types'
import { MessageBubble } from '../../ui/MessageBubble'
import { TypingIndicator } from '../../ui/TypingIndicator'

interface MessageListProps {
  messages: FairBotMessage[]
  isTyping: boolean
}

const ListContainer = styled('section')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_LIST_GAP}px`,
  padding: `${FAIRBOT_LAYOUT.MESSAGE_LIST_PADDING}px`,
})

const ListHeader = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
}))

const MessageStack = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_LIST_GAP}px`,
})

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

import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import {
  CHAT_ARIA,
  CHAT_NUMBERS,
} from '../constants/chat.constants'
import type { ChatMessage } from '../types/chat.types'
import { MessageBubble, type MessageBubbleLabels } from './MessageBubble'
import { TypingIndicator } from './TypingIndicator'

export interface MessageListLabels extends MessageBubbleLabels {
  messageListHeading?: string
  loadingMessage?: string
}

export interface MessageListProps {
  messages: ChatMessage[]
  isTyping: boolean
  labels?: MessageListLabels
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
export const MessageList = ({
  messages,
  isTyping,
  labels = {},
}: MessageListProps) => {
  const {
    messageListHeading = 'Conversation',
    loadingMessage,
    ...bubbleLabels
  } = labels

  const hasMessages = messages.length > CHAT_NUMBERS.ZERO

  return (
    <ListContainer aria-label={CHAT_ARIA.MESSAGE_LIST}>
      <ListHeader variant="caption">{messageListHeading}</ListHeader>
      <MessageStack>
        {messages.map((message) => (
          <MessageBubble key={message.id} message={message} labels={bubbleLabels} />
        ))}
        <TypingIndicator isVisible={isTyping && hasMessages} loadingMessage={loadingMessage} />
      </MessageStack>
    </ListContainer>
  )
}

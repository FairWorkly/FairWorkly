import { styled } from '@/styles/styled'
import { FAIRBOT_ARIA } from '../constants/fairbot.constants'
import type { FairBotMessage } from '../types/fairbot.types'
import { MessageBubble } from './MessageBubble'
import { TypingIndicator } from './TypingIndicator'

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

const MessageStack = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2),
}))

export const MessageList = ({ messages, isTyping }: MessageListProps) => {
  const hasMessages = messages.length > 0

  return (
    <ListContainer aria-label={FAIRBOT_ARIA.MESSAGE_LIST}>
      <MessageStack>
        {messages.map((message) => (
          <MessageBubble key={message.id} message={message} />
        ))}
        <TypingIndicator isVisible={isTyping && hasMessages} />
      </MessageStack>
    </ListContainer>
  )
}

import { styled } from '@/styles/styled'
import { FAIRBOT_ARIA } from '../constants/fairbot.constants'
import type { FairBotMessage } from '../types/fairbot.types'
import { MessageBubble } from './MessageBubble'
import { TypingIndicator } from './TypingIndicator'

interface MessageListProps {
  messages: FairBotMessage[]
  isTyping: boolean
  onQuickFollowUp?: (prompt: string) => void
  quickFollowUpDisabled?: boolean
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

export const MessageList = ({
  messages,
  isTyping,
  onQuickFollowUp,
  quickFollowUpDisabled = false,
}: MessageListProps) => {
  const hasMessages = messages.length > 0
  const latestActionPlanIndex = [...messages]
    .reverse()
    .findIndex(message => {
      const actions = message.metadata?.actionPlan?.actions
      return Array.isArray(actions) && actions.length > 0
    })
  const latestActionPlanMessageIndex =
    latestActionPlanIndex === -1
      ? -1
      : messages.length - 1 - latestActionPlanIndex

  return (
    <ListContainer aria-label={FAIRBOT_ARIA.MESSAGE_LIST}>
      <MessageStack>
        {messages.map((message, index) => (
          <MessageBubble
            key={message.id}
            message={message}
            onQuickFollowUp={onQuickFollowUp}
            quickFollowUpDisabled={quickFollowUpDisabled}
            showActionPlan={index === latestActionPlanMessageIndex}
          />
        ))}
        <TypingIndicator isVisible={isTyping && hasMessages} />
      </MessageStack>
    </ListContainer>
  )
}

import { useRef, useEffect } from 'react'
import { styled } from '@/styles/styled'
import Alert from '@mui/material/Alert'
import Chip from '@mui/material/Chip'
import Divider from '@mui/material/Divider'
import AssignmentIcon from '@mui/icons-material/Assignment'
import { FAIRBOT_ARIA } from '../constants/fairbot.constants'
import { useConversation } from '../hooks/useConversation'
import { MessageList } from './MessageList'
import { MessageInput } from './MessageInput'

const SectionContainer = styled('section')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  flex: 1,
  minHeight: 0,
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: `${theme.fairworkly.radius.xl}px`,
  overflow: 'hidden',
  backgroundColor: theme.palette.background.paper,
}))

const ScrollArea = styled('div')(({ theme }) => ({
  flex: 1,
  minHeight: 0,
  overflowY: 'auto',
  padding: theme.spacing(0, 1),
}))

const InputArea = styled('div')(({ theme }) => ({
  padding: theme.spacing(2, 3),
}))

const StatusArea = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
  padding: theme.spacing(2, 3, 0),
}))

export const ChatSection = () => {
  const conversation = useConversation()
  const scrollRef = useRef<HTMLDivElement>(null)

  const handleQuickFollowUp = (prompt: string) => {
    if (!prompt.trim()) {
      return
    }
    console.info('[FairBot][action_follow_up_clicked]')
    void conversation.sendMessage(prompt)
  }

  useEffect(() => {
    const el = scrollRef.current
    if (!el) return
    const threshold = 80
    const isNearBottom = el.scrollHeight - el.scrollTop - el.clientHeight < threshold
    if (isNearBottom) {
      el.scrollTo({ top: el.scrollHeight, behavior: 'smooth' })
    }
  }, [conversation.messages, conversation.isTyping])

  return (
    <SectionContainer aria-label={FAIRBOT_ARIA.CHAT_AREA}>
      <StatusArea>
        {conversation.contextLabel && (
          <Chip
            icon={<AssignmentIcon />}
            label={conversation.contextLabel}
            size="small"
            color="info"
            variant="outlined"
          />
        )}
        {conversation.errorMessage && (
          <Alert severity="error">{conversation.errorMessage}</Alert>
        )}
      </StatusArea>
      <ScrollArea ref={scrollRef}>
        <MessageList
          messages={conversation.messages}
          isTyping={conversation.isTyping}
          onQuickFollowUp={handleQuickFollowUp}
          quickFollowUpDisabled={
            conversation.isLoading || conversation.isContextLoading
          }
        />
      </ScrollArea>
      <Divider />
      <InputArea>
        <MessageInput
          onSendMessage={conversation.sendMessage}
          disabled={conversation.isLoading || conversation.isContextLoading}
        />
      </InputArea>
    </SectionContainer>
  )
}

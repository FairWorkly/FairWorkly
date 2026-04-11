import { useCallback } from 'react'
import { styled } from '@/styles/styled'
import { WelcomeHeader } from '../components/WelcomeHeader'
import { ActionCards } from '../components/ActionCards'
import { ChatSection } from '../components/ChatSection'
import { useConversation } from '../hooks/useConversation'

const DEBATE_TRIGGER =
  '/debate Alice worked 10 hours on Saturday. She already worked 38 hours this week. Full-time employee under the Hospitality Industry (General) Award 2020. What is the correct pay rate?'

const PageContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(4),
  height: '100%',
}))

export const FairBotPage = () => {
  const conversation = useConversation()

  const handleDebateClick = useCallback(() => {
    void conversation.sendMessage(DEBATE_TRIGGER)
  }, [conversation.sendMessage])

  return (
    <PageContainer>
      <WelcomeHeader />
      <ActionCards onDebateClick={handleDebateClick} />
      <ChatSection conversationOverride={conversation} />
    </PageContainer>
  )
}

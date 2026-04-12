import { useCallback } from 'react'
import { styled } from '@/styles/styled'
import { WelcomeHeader } from '../components/WelcomeHeader'
import { ActionCards } from '../components/ActionCards'
import { ChatSection } from '../components/ChatSection'
import { useConversation } from '../hooks/useConversation'
import { SAMPLE_DEBATE_SCENARIO } from '../utils'

const DEBATE_TRIGGER = `/debate ${SAMPLE_DEBATE_SCENARIO.employee_name} worked ${SAMPLE_DEBATE_SCENARIO.shift_hours} hours on ${SAMPLE_DEBATE_SCENARIO.shift_date}. She already worked ${SAMPLE_DEBATE_SCENARIO.week_hours_before_shift} hours this week. ${SAMPLE_DEBATE_SCENARIO.extra_context} under the ${SAMPLE_DEBATE_SCENARIO.award_name}. What is the correct pay rate?`

const PageContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(4),
  height: '100%',
}))

export const FairBotPage = () => {
  const conversation = useConversation()
  const { sendMessage } = conversation

  const handleDebateClick = useCallback(() => {
    void sendMessage(DEBATE_TRIGGER)
  }, [sendMessage])

  return (
    <PageContainer>
      <WelcomeHeader />
      <ActionCards onDebateClick={handleDebateClick} />
      <ChatSection conversationOverride={conversation} />
    </PageContainer>
  )
}

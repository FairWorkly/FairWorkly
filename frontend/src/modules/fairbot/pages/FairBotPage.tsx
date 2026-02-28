import { styled } from '@/styles/styled'
import { WelcomeHeader } from '../components/WelcomeHeader'
import { ActionCards } from '../components/ActionCards'
import { ChatSection } from '../components/ChatSection'

const PageContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(4),
  height: '100%',
}))

export const FairBotPage = () => (
  <PageContainer>
    <WelcomeHeader />
    <ActionCards />
    <ChatSection />
  </PageContainer>
)

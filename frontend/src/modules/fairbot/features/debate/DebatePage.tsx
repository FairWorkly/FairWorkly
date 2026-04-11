import Typography from '@mui/material/Typography'
import Alert from '@mui/material/Alert'
import LinearProgress from '@mui/material/LinearProgress'
import { styled } from '@/styles/styled'
import { DebateScenarioForm } from './DebateScenarioForm'
import { DebateTimeline } from './DebateTimeline'
import { useDebate } from './useDebate'

const PageContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(4),
  maxWidth: 800,
  margin: '0 auto',
  padding: theme.spacing(4, 2),
}))

const Header = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
}))

const Section = styled('div')(({ theme }) => ({
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: theme.fairworkly.radius.lg,
  backgroundColor: theme.palette.background.paper,
  padding: theme.spacing(3),
  boxShadow: theme.fairworkly.shadow.sm,
}))

export const DebatePage = () => {
  const { result, isLoading, error, startDebate } = useDebate()

  return (
    <PageContainer>
      <Header>
        <Typography variant="h4">Multi-Agent Compliance Debate</Typography>
        <Typography variant="body2" color="text.secondary">
          Three AI agents review a shift scenario, challenge each other's
          analysis, and deliver a final ruling backed by Award legislation.
        </Typography>
      </Header>

      <Section>
        <Typography variant="subtitle2" sx={{ mb: 2 }}>
          Shift Scenario
        </Typography>
        <DebateScenarioForm onSubmit={startDebate} isLoading={isLoading} />
      </Section>

      {isLoading && <LinearProgress />}

      {error && <Alert severity="error">{error}</Alert>}

      {result && (
        <Section>
          <Typography variant="subtitle2" sx={{ mb: 2.5 }}>
            Agent Debate
          </Typography>
          <DebateTimeline result={result} />
        </Section>
      )}
    </PageContainer>
  )
}

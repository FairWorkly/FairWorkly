import Typography from '@mui/material/Typography'
import Chip from '@mui/material/Chip'
import GavelIcon from '@mui/icons-material/Gavel'
import { styled } from '@/styles/styled'
import type { DebateResult } from '../../types/debate.types'

interface DebateTimelineProps {
  result: DebateResult
}

/* ── Styled components ─────────────────────────────────── */

const Root = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: 0,
  position: 'relative',
})

const RoundRow = styled('div')(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2),
  position: 'relative',
}))

const TimelineTrack = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  width: 48,
  flexShrink: 0,
})

const IconCircle = styled('div')(({ theme }) => ({
  width: 40,
  height: 40,
  borderRadius: '50%',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  fontSize: 20,
  backgroundColor: theme.palette.background.paper,
  border: `2px solid ${theme.palette.divider}`,
  position: 'relative',
  zIndex: 1,
}))

const Connector = styled('div')(({ theme }) => ({
  width: 2,
  flex: 1,
  minHeight: 24,
  backgroundColor: theme.palette.divider,
}))

const RoundCard = styled('div')(({ theme }) => ({
  flex: 1,
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: theme.fairworkly.radius.lg,
  backgroundColor: theme.palette.background.paper,
  padding: theme.spacing(2),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1.25),
  marginBottom: theme.spacing(2),
  boxShadow: theme.fairworkly.shadow.sm,
}))

const AgentHeader = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
}))

const ChallengeBar = styled('div')(({ theme }) => ({
  borderLeft: `3px solid ${theme.palette.warning.main}`,
  paddingLeft: theme.spacing(1.5),
  backgroundColor: 'rgba(249, 115, 22, 0.06)',
  borderRadius: `0 ${theme.fairworkly.radius.sm}px ${theme.fairworkly.radius.sm}px 0`,
  padding: theme.spacing(1, 1.5),
}))

const SourcesRow = styled('div')(({ theme }) => ({
  display: 'flex',
  flexWrap: 'wrap',
  gap: theme.spacing(0.5),
}))

const VerdictCard = styled('div')(({ theme }) => ({
  border: `2px solid ${theme.palette.success.main}`,
  borderRadius: theme.fairworkly.radius.lg,
  backgroundColor: 'rgba(16, 185, 129, 0.06)',
  padding: theme.spacing(2.5),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1.25),
  boxShadow: theme.fairworkly.shadow.md,
}))

/* ── Component ─────────────────────────────────────────── */

export const DebateTimeline = ({ result }: DebateTimelineProps) => {
  const { rounds, final_ruling, cited_award_section, model } = result

  return (
    <Root>
      {rounds.map((round, idx) => (
        <RoundRow key={round.agent}>
          <TimelineTrack>
            <IconCircle>{round.icon}</IconCircle>
            {idx < rounds.length - 1 && <Connector />}
          </TimelineTrack>

          <RoundCard>
            <AgentHeader>
              <Typography variant="subtitle2">{round.agent}</Typography>
              <Chip
                size="small"
                label={round.role}
                variant="outlined"
                color={idx === 0 ? 'info' : idx === 1 ? 'warning' : 'success'}
              />
            </AgentHeader>

            <Typography variant="body2" fontWeight={700}>
              {round.stance}
            </Typography>

            {round.challenges && (
              <ChallengeBar>
                <Typography variant="caption" color="warning.dark">
                  {round.challenges}
                </Typography>
              </ChallengeBar>
            )}

            <Typography variant="body2" color="text.secondary">
              {round.reasoning}
            </Typography>

            {round.sources.length > 0 && (
              <SourcesRow>
                {round.sources.map((src, i) => (
                  <Chip
                    key={`${src.source}-${src.page}-${i}`}
                    size="small"
                    label={`${src.source} p.${src.page + 1}`}
                    variant="outlined"
                  />
                ))}
              </SourcesRow>
            )}
          </RoundCard>
        </RoundRow>
      ))}

      {/* Final ruling */}
      <RoundRow>
        <TimelineTrack>
          <IconCircle>
            <GavelIcon fontSize="small" color="success" />
          </IconCircle>
        </TimelineTrack>

        <VerdictCard>
          <Typography variant="subtitle1" color="success.dark">
            Final Ruling
          </Typography>
          <Typography variant="body1" fontWeight={700}>
            {final_ruling}
          </Typography>
          {cited_award_section && (
            <Typography variant="caption" color="text.secondary">
              Cited: {cited_award_section}
            </Typography>
          )}
          {model && (
            <Typography variant="caption" color="text.disabled">
              Model: {model}
            </Typography>
          )}
        </VerdictCard>
      </RoundRow>
    </Root>
  )
}

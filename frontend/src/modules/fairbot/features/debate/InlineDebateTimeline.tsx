import Typography from '@mui/material/Typography'
import Chip from '@mui/material/Chip'
import GavelIcon from '@mui/icons-material/Gavel'
import { styled } from '@/styles/styled'
import type { FairBotDebateResult } from '../../types/fairbot.types'

interface InlineDebateTimelineProps {
  result: FairBotDebateResult
}

const Root = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: 0,
})

const RoundRow = styled('div')(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(1.5),
}))

const Track = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  width: 36,
  flexShrink: 0,
})

const Dot = styled('div')(({ theme }) => ({
  width: 32,
  height: 32,
  borderRadius: '50%',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  fontSize: 16,
  backgroundColor: theme.palette.background.default,
  border: `2px solid ${theme.palette.divider}`,
  zIndex: 1,
}))

const Line = styled('div')(({ theme }) => ({
  width: 2,
  flex: 1,
  minHeight: 16,
  backgroundColor: theme.palette.divider,
}))

const RoundBody = styled('div')(({ theme }) => ({
  flex: 1,
  paddingBottom: theme.spacing(1.5),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),
}))

const ChallengeText = styled(Typography)(({ theme }) => ({
  borderLeft: `3px solid ${theme.palette.warning.main}`,
  paddingLeft: theme.spacing(1),
  marginTop: theme.spacing(0.25),
}))

const VerdictBox = styled('div')(({ theme }) => ({
  border: `2px solid ${theme.palette.success.main}`,
  borderRadius: theme.fairworkly.radius.md,
  backgroundColor: 'rgba(16, 185, 129, 0.06)',
  padding: theme.spacing(1.5),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),
}))

const SourceChips = styled('div')(({ theme }) => ({
  display: 'flex',
  flexWrap: 'wrap',
  gap: theme.spacing(0.5),
  marginTop: theme.spacing(0.25),
}))

export const InlineDebateTimeline = ({ result }: InlineDebateTimelineProps) => {
  const { rounds, final_ruling, cited_award_section } = result

  return (
    <Root>
      {rounds.map((round, idx) => (
        <RoundRow key={round.agent}>
          <Track>
            <Dot>{round.icon}</Dot>
            {idx < rounds.length && <Line />}
          </Track>
          <RoundBody>
            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <Typography variant="caption" fontWeight={700}>
                {round.agent}
              </Typography>
              <Chip
                size="small"
                label={round.role}
                variant="outlined"
                color={idx === 0 ? 'info' : idx === 1 ? 'warning' : 'success'}
                sx={{ height: 20, fontSize: 11 }}
              />
            </div>
            <Typography variant="body2" fontWeight={600}>
              {round.stance}
            </Typography>
            {round.challenges && (
              <ChallengeText variant="caption" color="warning.dark">
                {round.challenges}
              </ChallengeText>
            )}
            <Typography variant="caption" color="text.secondary">
              {round.reasoning}
            </Typography>
            {round.sources.length > 0 && (
              <SourceChips>
                {round.sources.map((src, i) => (
                  <Chip
                    key={`${src.source}-${src.page}-${i}`}
                    size="small"
                    label={`${src.source} p.${(src.page ?? 0) + 1}`}
                    variant="outlined"
                    sx={{ height: 20, fontSize: 10 }}
                  />
                ))}
              </SourceChips>
            )}
          </RoundBody>
        </RoundRow>
      ))}

      <RoundRow>
        <Track>
          <Dot>
            <GavelIcon sx={{ fontSize: 16 }} color="success" />
          </Dot>
        </Track>
        <VerdictBox>
          <Typography variant="caption" fontWeight={700} color="success.dark">
            Final Ruling
          </Typography>
          <Typography variant="body2" fontWeight={700}>
            {final_ruling}
          </Typography>
          {cited_award_section && (
            <Typography variant="caption" color="text.secondary">
              Cited: {cited_award_section}
            </Typography>
          )}
        </VerdictBox>
      </RoundRow>
    </Root>
  )
}

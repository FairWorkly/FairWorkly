import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import Button from '@mui/material/Button'
import Chip from '@mui/material/Chip'
import { FAIRBOT_ROLES } from '../constants/fairbot.constants'
import type { FairBotMessage } from '../types/fairbot.types'

interface MessageBubbleProps {
  message: FairBotMessage
  onQuickFollowUp?: (prompt: string) => void
  quickFollowUpDisabled?: boolean
  showActionPlan?: boolean
}

interface BubbleProps {
  isUser: boolean
}

const MessageRow = styled('div', {
  shouldForwardProp: (prop) => prop !== 'isUser',
})<BubbleProps>(({ theme, isUser }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: isUser ? 'flex-end' : 'flex-start',
  gap: theme.spacing(1),
}))

const Bubble = styled('div', {
  shouldForwardProp: (prop) => prop !== 'isUser',
})<BubbleProps>(({ theme, isUser }) => ({
  maxWidth: theme.spacing(65),
  borderRadius: theme.fairworkly.radius.lg,
  padding: theme.spacing(1.5),
  backgroundColor: isUser
    ? theme.palette.primary.main
    : theme.palette.action.hover,
  color: isUser ? theme.palette.primary.contrastText : theme.palette.text.primary,
}))

const MetaRow = styled('div', {
  shouldForwardProp: (prop) => prop !== 'isUser',
})<BubbleProps>(({ theme, isUser }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  color: isUser ? theme.palette.primary.main : theme.palette.text.secondary,
}))

const DetailSection = styled('div')(({ theme }) => ({
  marginTop: theme.spacing(1.25),
  paddingTop: theme.spacing(1),
  borderTop: `1px solid ${theme.palette.divider}`,
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.75),
}))

const SourceList = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),
}))

const SourceItem = styled('div')(({ theme }) => ({
  padding: theme.spacing(0.5, 0.75),
  borderRadius: theme.fairworkly.radius.sm,
  backgroundColor: theme.palette.action.hover,
}))

const ActionPlanSection = styled('div')(({ theme }) => ({
  marginTop: theme.spacing(1.25),
  paddingTop: theme.spacing(1),
  borderTop: `1px solid ${theme.palette.divider}`,
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
}))

const ActionCard = styled('div')(({ theme }) => ({
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: theme.fairworkly.radius.md,
  backgroundColor: theme.palette.background.paper,
  padding: theme.spacing(1),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.75),
}))

const FollowUpRow = styled('div')(({ theme }) => ({
  marginTop: theme.spacing(0.25),
  display: 'flex',
  flexWrap: 'wrap',
  gap: theme.spacing(0.75),
}))

const FollowUpButton = styled(Button)(({ theme }) => ({
  textTransform: 'none',
  borderRadius: theme.fairworkly.radius.sm,
}))

const formatTimestamp = (timestamp: string): string => {
  const date = new Date(timestamp)
  if (Number.isNaN(date.getTime())) {
    return timestamp
  }
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const formatPage = (page?: number): string => {
  if (typeof page !== 'number' || Number.isNaN(page)) {
    return ''
  }
  return `, p.${page + 1}`
}

const truncate = (text?: string, maxLength = 140): string | null => {
  if (!text) {
    return null
  }
  if (text.length <= maxLength) {
    return text
  }
  return `${text.slice(0, maxLength - 3)}...`
}

export const MessageBubble = ({
  message,
  onQuickFollowUp,
  quickFollowUpDisabled = false,
  showActionPlan = true,
}: MessageBubbleProps) => {
  const isUser = message.role === FAIRBOT_ROLES.USER
  const senderLabel = isUser ? 'You' : 'FairBot'
  const metadata = message.metadata
  const sources = metadata?.sources?.slice(0, 5) ?? []
  const actionPlan = showActionPlan ? metadata?.actionPlan : undefined
  const hasDetails =
    !isUser &&
    (metadata?.model || metadata?.note || sources.length > 0 || actionPlan)

  return (
    <MessageRow isUser={isUser}>
      <MetaRow isUser={isUser}>
        <Typography variant="caption">{senderLabel}</Typography>
        <Typography variant="caption">
          Sent {formatTimestamp(message.timestamp)}
        </Typography>
      </MetaRow>
      <Bubble isUser={isUser}>
        <Typography variant="body2" whiteSpace="pre-line">{message.text}</Typography>
        {hasDetails && (
          <DetailSection>
            {metadata?.model && (
              <Typography variant="caption" color="text.secondary">
                Model: {metadata.model}
              </Typography>
            )}
            {metadata?.note && (
              <Typography variant="caption" color="text.secondary">
                Note: {metadata.note}
              </Typography>
            )}
            {sources.length > 0 && (
              <SourceList>
                <Typography variant="caption" color="text.secondary">
                  Sources:
                </Typography>
                {sources.map((source, index) => {
                  const snippet = truncate(source.content)
                  return (
                    <SourceItem key={`${source.source}-${source.page ?? 'na'}-${index}`}>
                      <Typography variant="caption" color="text.secondary">
                        {index + 1}. {source.source}{formatPage(source.page)}
                      </Typography>
                      {snippet && (
                        <Typography variant="caption" display="block">
                          {snippet}
                        </Typography>
                      )}
                    </SourceItem>
                  )
                })}
              </SourceList>
            )}
            {actionPlan && Array.isArray(actionPlan.actions) && actionPlan.actions.length > 0 && (
              <ActionPlanSection>
                <Typography variant="caption" color="text.secondary">
                  {actionPlan.title}
                </Typography>
                {actionPlan.actions.slice(0, 3).map((action) => (
                  <ActionCard key={action.id}>
                    <div>
                      <Chip
                        size="small"
                        label={action.priority || 'Action'}
                        color="warning"
                        variant="outlined"
                      />
                    </div>
                    <Typography variant="body2" fontWeight={700}>
                      {action.title}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      What to change: {action.whatToChange}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      Why: {action.why}
                    </Typography>
                    {action.focusExamples && (
                      <Typography variant="caption" color="text.secondary">
                        Focus: {action.focusExamples}
                      </Typography>
                    )}
                  </ActionCard>
                ))}
                {Array.isArray(actionPlan.quickFollowUps) && actionPlan.quickFollowUps.length > 0 && onQuickFollowUp && (
                  <FollowUpRow>
                    {actionPlan.quickFollowUps.slice(0, 3).map((item) => (
                      <FollowUpButton
                        key={item.id}
                        size="small"
                        variant="outlined"
                        disabled={quickFollowUpDisabled}
                        onClick={() => onQuickFollowUp(item.prompt)}
                      >
                        {item.label}
                      </FollowUpButton>
                    ))}
                  </FollowUpRow>
                )}
              </ActionPlanSection>
            )}
          </DetailSection>
        )}
      </Bubble>
    </MessageRow>
  )
}

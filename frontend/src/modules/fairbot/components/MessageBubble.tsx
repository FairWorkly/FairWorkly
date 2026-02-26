import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import { FAIRBOT_ROLES } from '../constants/fairbot.constants'
import type { FairBotMessage } from '../types/fairbot.types'

interface MessageBubbleProps {
  message: FairBotMessage
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

export const MessageBubble = ({ message }: MessageBubbleProps) => {
  const isUser = message.role === FAIRBOT_ROLES.USER
  const senderLabel = isUser ? 'You' : 'FairBot'
  const metadata = message.metadata
  const sources = metadata?.sources?.slice(0, 5) ?? []
  const hasDetails = !isUser && (metadata?.model || metadata?.note || sources.length > 0)

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
          </DetailSection>
        )}
      </Bubble>
    </MessageRow>
  )
}

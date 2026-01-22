import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import Stack from '@mui/material/Stack'
import AttachFileOutlined from '@mui/icons-material/AttachFileOutlined'
import {
  FAIRBOT_LABELS,
  FAIRBOT_FILE_SIZE,
  FAIRBOT_MESSAGE_UI,
  FAIRBOT_NUMBERS,
  FAIRBOT_ROLES,
  FAIRBOT_TIME_FORMAT,
} from '../constants/fairbot.constants'
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
  maxWidth: `${FAIRBOT_MESSAGE_UI.BUBBLE_MAX_WIDTH}px`,
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

const FileBadge = styled('div')(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  borderRadius: theme.fairworkly.radius.md,
  padding: `${theme.spacing(0.5)} ${theme.spacing(1)}`,
  border: `1px solid ${theme.palette.divider}`,
  backgroundColor: theme.palette.background.paper,
  color: theme.palette.text.secondary,
}))

// Format message timestamps for display, falling back to raw input if invalid.
const formatTimestamp = (timestamp: string): string => {
  const date = new Date(timestamp)
  if (Number.isNaN(date.getTime())) {
    return timestamp
  }

  return date.toLocaleTimeString([], {
    hour: FAIRBOT_TIME_FORMAT.HOUR,
    minute: FAIRBOT_TIME_FORMAT.MINUTE,
  })
}

const formatFileSize = (bytes: number): string => {
  if (!Number.isFinite(bytes) || bytes <= FAIRBOT_NUMBERS.ZERO) {
    return FAIRBOT_FILE_SIZE.ZERO_KB
  }

  if (bytes >= FAIRBOT_FILE_SIZE.MEGA_THRESHOLD) {
    return `${(bytes / FAIRBOT_FILE_SIZE.MEGA_THRESHOLD).toFixed(
      FAIRBOT_FILE_SIZE.MEGA_DECIMALS,
    )} ${FAIRBOT_FILE_SIZE.MEGA_SUFFIX}`
  }

  return `${Math.ceil(bytes / FAIRBOT_FILE_SIZE.KILO_THRESHOLD)} ${
    FAIRBOT_FILE_SIZE.KILO_SUFFIX
  }`
}

export const MessageBubble = ({ message }: MessageBubbleProps) => {
  const isUser = message.role === FAIRBOT_ROLES.USER
  const senderLabel = isUser ? FAIRBOT_LABELS.USER_LABEL : FAIRBOT_LABELS.ASSISTANT_LABEL

  return (
    <MessageRow isUser={isUser}>
      <MetaRow isUser={isUser}>
        <Typography variant="caption">{senderLabel}</Typography>
        <Typography variant="caption">
          {FAIRBOT_LABELS.MESSAGE_TIME_PREFIX} {formatTimestamp(message.timestamp)}
        </Typography>
      </MetaRow>
      <Bubble isUser={isUser}>
        <Typography variant="body2">{message.text}</Typography>
      </Bubble>
      {message.fileMeta ? (
        <FileBadge>
          <AttachFileOutlined fontSize="small" />
          <Stack spacing={FAIRBOT_NUMBERS.ZERO}>
            <Typography variant="caption">{FAIRBOT_LABELS.ATTACHMENT_LABEL}</Typography>
            <Typography variant="caption">{message.fileMeta.name}</Typography>
            <Typography variant="caption">{formatFileSize(message.fileMeta.size)}</Typography>
          </Stack>
        </FileBadge>
      ) : null}
    </MessageRow>
  )
}

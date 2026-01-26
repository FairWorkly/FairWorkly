import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import Stack from '@mui/material/Stack'
import AttachFileOutlined from '@mui/icons-material/AttachFileOutlined'
import {
  CHAT_DEFAULT_LABELS,
  CHAT_MESSAGE_UI,
  CHAT_NUMBERS,
  CHAT_ROLES,
} from '../constants/chat.constants'
import { formatTimestamp, formatFileSize } from '../utils/formatters'
import type { ChatMessage } from '../types/chat.types'

export interface MessageBubbleLabels {
  userLabel?: string
  assistantLabel?: string
  messageTimePrefix?: string
  attachmentLabel?: string
}

export interface MessageBubbleProps {
  message: ChatMessage
  labels?: MessageBubbleLabels
}

interface BubbleStyleProps {
  isUser: boolean
}

const MessageRow = styled('div', {
  shouldForwardProp: (prop) => prop !== 'isUser',
})<BubbleStyleProps>(({ theme, isUser }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: isUser ? 'flex-end' : 'flex-start',
  gap: theme.spacing(1),
}))

const Bubble = styled('div', {
  shouldForwardProp: (prop) => prop !== 'isUser',
})<BubbleStyleProps>(({ theme, isUser }) => ({
  maxWidth: `${CHAT_MESSAGE_UI.BUBBLE_MAX_WIDTH}px`,
  borderRadius: theme.fairworkly.radius.lg,
  padding: theme.spacing(1.5),
  backgroundColor: isUser
    ? theme.palette.primary.main
    : theme.palette.action.hover,
  color: isUser ? theme.palette.primary.contrastText : theme.palette.text.primary,
}))

const MetaRow = styled('div', {
  shouldForwardProp: (prop) => prop !== 'isUser',
})<BubbleStyleProps>(({ theme, isUser }) => ({
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

export const MessageBubble = ({ message, labels = {} }: MessageBubbleProps) => {
  const {
    userLabel = CHAT_DEFAULT_LABELS.USER_LABEL,
    assistantLabel = CHAT_DEFAULT_LABELS.ASSISTANT_LABEL,
    messageTimePrefix = CHAT_DEFAULT_LABELS.MESSAGE_TIME_PREFIX,
    attachmentLabel = CHAT_DEFAULT_LABELS.ATTACHMENT_LABEL,
  } = labels

  const isUser = message.role === CHAT_ROLES.USER
  const senderLabel = isUser ? userLabel : assistantLabel

  return (
    <MessageRow isUser={isUser}>
      <MetaRow isUser={isUser}>
        <Typography variant="caption">{senderLabel}</Typography>
        <Typography variant="caption">
          {messageTimePrefix} {formatTimestamp(message.timestamp)}
        </Typography>
      </MetaRow>
      <Bubble isUser={isUser}>
        <Typography variant="body2">{message.text}</Typography>
      </Bubble>
      {message.fileMeta ? (
        <FileBadge>
          <AttachFileOutlined fontSize="small" />
          <Stack spacing={CHAT_NUMBERS.ZERO}>
            <Typography variant="caption">{attachmentLabel}</Typography>
            <Typography variant="caption">{message.fileMeta.name}</Typography>
            <Typography variant="caption">{formatFileSize(message.fileMeta.size)}</Typography>
          </Stack>
        </FileBadge>
      ) : null}
    </MessageRow>
  )
}

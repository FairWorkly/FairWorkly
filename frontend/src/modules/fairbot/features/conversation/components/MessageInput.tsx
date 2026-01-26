import { useState, type FormEvent } from 'react'
import { styled } from '@/styles/styled'
import IconButton from '@mui/material/IconButton'
import TextField from '@mui/material/TextField'
import SendRounded from '@mui/icons-material/SendRounded'
import AttachFileRounded from '@mui/icons-material/AttachFileRounded'
import {
  CHAT_ARIA,
  CHAT_DEFAULT_LABELS,
  CHAT_INPUT_TYPES,
  CHAT_INPUT_UI,
  CHAT_TEXT,
} from '../constants/chat.constants'

export interface MessageInputLabels {
  inputPlaceholder?: string
  sendButtonLabel?: string
  attachButtonLabel?: string
  messageInputLabel?: string
}

export interface MessageInputControls {
  openFileDialog: () => void
}

export interface MessageInputProps {
  onSendMessage: (message: string) => Promise<void>
  controls: MessageInputControls
  disabled?: boolean
  labels?: MessageInputLabels
  inputId?: string
}

const InputRow = styled('form')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
}))

const StyledTextField = styled(TextField)(({ theme }) => ({
  flex: 1,
  '& .MuiOutlinedInput-root': {
    borderRadius: theme.fairworkly.radius.md,
  },
}))

const InputButton = styled(IconButton)(({ theme }) => ({
  width: `${CHAT_INPUT_UI.BUTTON_SIZE}px`,
  height: `${CHAT_INPUT_UI.BUTTON_SIZE}px`,
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.divider}`,
}))

// Text entry plus attach/send actions for the chat footer.
export const MessageInput = ({
  onSendMessage,
  controls,
  disabled = false,
  labels = {},
  inputId = 'chat-message-input',
}: MessageInputProps) => {
  const {
    inputPlaceholder = CHAT_DEFAULT_LABELS.INPUT_PLACEHOLDER,
    sendButtonLabel = CHAT_DEFAULT_LABELS.SEND_BUTTON_LABEL,
    attachButtonLabel = CHAT_DEFAULT_LABELS.ATTACH_BUTTON_LABEL,
    messageInputLabel = CHAT_DEFAULT_LABELS.MESSAGE_INPUT_LABEL,
  } = labels

  const [value, setValue] = useState(CHAT_TEXT.EMPTY)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    // Ignore empty submissions; file uploads are handled via the attach button.
    if (!value.trim()) {
      return
    }

    await onSendMessage(value)
    setValue(CHAT_TEXT.EMPTY)
  }

  return (
    <InputRow onSubmit={handleSubmit} aria-label={CHAT_ARIA.MESSAGE_INPUT}>
      <InputButton
        type={CHAT_INPUT_TYPES.BUTTON}
        aria-label={attachButtonLabel}
        onClick={controls.openFileDialog}
        disabled={disabled}
      >
        <AttachFileRounded fontSize="small" />
      </InputButton>
      <StyledTextField
        id={inputId}
        placeholder={inputPlaceholder}
        value={value}
        onChange={(event) => setValue(event.target.value)}
        size="small"
        disabled={disabled}
        aria-label={messageInputLabel}
      />
      <InputButton
        type={CHAT_INPUT_TYPES.SUBMIT}
        aria-label={sendButtonLabel}
        disabled={disabled}
      >
        <SendRounded fontSize="small" />
      </InputButton>
    </InputRow>
  )
}

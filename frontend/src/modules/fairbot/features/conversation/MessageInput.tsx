import { useState, type FormEvent } from 'react'
import { styled } from '@/styles/styled'
import IconButton from '@mui/material/IconButton'
import TextField from '@mui/material/TextField'
import SendRounded from '@mui/icons-material/SendRounded'
import AttachFileRounded from '@mui/icons-material/AttachFileRounded'
import {
  FAIRBOT_ARIA,
  FAIRBOT_IDS,
  FAIRBOT_INPUT_TYPES,
  FAIRBOT_INPUT_UI,
  FAIRBOT_LABELS,
  FAIRBOT_TEXT,
} from '../../constants/fairbot.constants'
import type { FileUploadControls } from '../../hooks/useFileUpload'

interface MessageInputProps {
  onSendMessage: (message: string) => Promise<void>
  upload: FileUploadControls
  disabled?: boolean
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
  width: `${FAIRBOT_INPUT_UI.BUTTON_SIZE}px`,
  height: `${FAIRBOT_INPUT_UI.BUTTON_SIZE}px`,
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.divider}`,
}))

// Text entry plus attach/send actions for the chat footer.
export const MessageInput = ({
  onSendMessage,
  upload,
  disabled = false,
}: MessageInputProps) => {
  const [value, setValue] = useState(FAIRBOT_TEXT.EMPTY)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    // Ignore empty submissions; file uploads are handled via the attach button.
    if (!value.trim()) {
      return
    }

    await onSendMessage(value)
    setValue(FAIRBOT_TEXT.EMPTY)
  }

  return (
    <InputRow onSubmit={handleSubmit} aria-label={FAIRBOT_ARIA.MESSAGE_INPUT}>
      <InputButton
        type={FAIRBOT_INPUT_TYPES.BUTTON}
        aria-label={FAIRBOT_LABELS.ATTACH_BUTTON_LABEL}
        onClick={upload.openFileDialog}
        disabled={disabled}
      >
        <AttachFileRounded fontSize="small" />
      </InputButton>
      <StyledTextField
        id={FAIRBOT_IDS.MESSAGE_INPUT}
        placeholder={FAIRBOT_LABELS.INPUT_PLACEHOLDER}
        value={value}
        onChange={(event) => setValue(event.target.value)}
        size="small"
        disabled={disabled}
        aria-label={FAIRBOT_LABELS.MESSAGE_INPUT_LABEL}
      />
      <InputButton
        type={FAIRBOT_INPUT_TYPES.SUBMIT}
        aria-label={FAIRBOT_LABELS.SEND_BUTTON_LABEL}
        disabled={disabled}
      >
        <SendRounded fontSize="small" />
      </InputButton>
    </InputRow>
  )
}

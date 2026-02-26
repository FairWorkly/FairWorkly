import { useState, type FormEvent } from 'react'
import { styled } from '@/styles/styled'
import IconButton from '@mui/material/IconButton'
import TextField from '@mui/material/TextField'
import SendRounded from '@mui/icons-material/SendRounded'
import { FAIRBOT_ARIA } from '../constants/fairbot.constants'

interface MessageInputProps {
  onSendMessage: (message: string) => Promise<boolean>
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

const SendButton = styled(IconButton)(({ theme }) => ({
  width: theme.spacing(5.5),
  height: theme.spacing(5.5),
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.divider}`,
}))

export const MessageInput = ({
  onSendMessage,
  disabled = false,
}: MessageInputProps) => {
  const [value, setValue] = useState('')

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (!value.trim()) {
      return
    }

    const success = await onSendMessage(value)
    if (success) {
      setValue('')
    }
  }

  return (
    <InputRow onSubmit={handleSubmit} aria-label={FAIRBOT_ARIA.MESSAGE_INPUT}>
      <StyledTextField
        placeholder="Ask a Fair Work question..."
        value={value}
        onChange={(event) => setValue(event.target.value)}
        size="small"
        disabled={disabled}
        aria-label="Message"
      />
      <SendButton
        type="submit"
        aria-label="Send message"
        disabled={disabled}
      >
        <SendRounded fontSize="small" />
      </SendButton>
    </InputRow>
  )
}

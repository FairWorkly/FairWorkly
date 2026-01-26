import { styled } from '@/styles/styled'
import type { ReactNode, RefObject, ChangeEvent, DragEvent } from 'react'
import {
  CHAT_ARIA,
  CHAT_INPUT_TYPES,
  CHAT_UPLOAD_UI,
} from '../constants/chat.constants'
import type { ChatError } from '../types/chat.types'

export interface FileUploadControls {
  isDragging: boolean
  isUploading: boolean
  error: ChatError | null
  acceptAttribute: string
  handleDragEnter: (event: DragEvent<HTMLElement>) => void
  handleDragLeave: (event: DragEvent<HTMLElement>) => void
  handleDragOver: (event: DragEvent<HTMLElement>) => void
  handleDrop: (event: DragEvent<HTMLElement>) => void
  handleFileSelect: (event: ChangeEvent<HTMLInputElement>) => void
}

export interface FileUploadZoneProps {
  controls: FileUploadControls
  inputRef: RefObject<HTMLInputElement | null>
  children: ReactNode
  helperText?: string
  disabled?: boolean
  inputId?: string
}

interface UploadContainerProps {
  isDragging: boolean
  isDisabled: boolean
}

// Drop zone styling is driven by state props, so keep them off the DOM.
const UploadContainer = styled('div', {
  shouldForwardProp: (prop) =>
    prop !== 'isDragging' && prop !== 'isDisabled',
})<UploadContainerProps>(({ theme, isDragging, isDisabled }) => ({
  border: CHAT_UPLOAD_UI.BORDER_NONE,
  borderRadius: theme.fairworkly.radius.lg,
  padding: theme.spacing(1),
  minHeight: `${CHAT_UPLOAD_UI.MIN_HEIGHT}px`,
  backgroundColor: isDragging
    ? theme.palette.action.hover
    : theme.palette.background.default,
  transition: `${CHAT_UPLOAD_UI.TRANSITION_PROPERTIES} ${CHAT_UPLOAD_UI.TRANSITION_MS}ms ${CHAT_UPLOAD_UI.TRANSITION_EASING}`,
  opacity: isDisabled ? theme.palette.action.disabledOpacity : 1,
  pointerEvents: isDisabled ? 'none' : 'auto',
}))

const UploadSurface = styled('label')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  width: '100%',
  cursor: 'pointer',
}))

const HiddenInput = styled('input')({
  display: 'none',
})

const HelperText = styled('p')(({ theme }) => ({
  ...theme.typography.body2,
  marginTop: theme.spacing(1),
  marginLeft: `${CHAT_UPLOAD_UI.HELPER_TEXT_OFFSET_PX}px`,
  marginBottom: 0,
  color: theme.palette.text.secondary,
}))

const ErrorText = styled('p')(({ theme }) => ({
  ...theme.typography.body2,
  marginTop: theme.spacing(1),
  marginBottom: 0,
  color: theme.palette.error.main,
}))

// Drag-and-drop file zone that wires to the shared upload controls.
export const FileUploadZone = ({
  controls,
  inputRef,
  children,
  helperText,
  disabled = false,
  inputId = 'chat-file-input',
}: FileUploadZoneProps) => {
  const handleDragEnter = disabled ? undefined : controls.handleDragEnter
  const handleDragLeave = disabled ? undefined : controls.handleDragLeave
  const handleDragOver = disabled ? undefined : controls.handleDragOver
  const handleDrop = disabled ? undefined : controls.handleDrop
  const handleFileSelect = disabled ? undefined : controls.handleFileSelect

  return (
    <UploadContainer
      aria-label={CHAT_ARIA.FILE_UPLOAD}
      isDragging={controls.isDragging}
      isDisabled={disabled}
      onDragEnter={handleDragEnter}
      onDragLeave={handleDragLeave}
      onDragOver={handleDragOver}
      onDrop={handleDrop}
    >
      <HiddenInput
        id={inputId}
        ref={inputRef}
        type={CHAT_INPUT_TYPES.FILE}
        accept={controls.acceptAttribute}
        onChange={handleFileSelect}
        disabled={disabled}
      />
      <UploadSurface htmlFor={inputId}>{children}</UploadSurface>
      {helperText ? <HelperText>{helperText}</HelperText> : null}
      {controls.error ? <ErrorText role="alert">{controls.error.message}</ErrorText> : null}
    </UploadContainer>
  )
}

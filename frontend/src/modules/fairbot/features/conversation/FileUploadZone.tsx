import { styled } from '@mui/material/styles'
import type { ReactNode, RefObject } from 'react'
import {
  FAIRBOT_ARIA,
  FAIRBOT_IDS,
  FAIRBOT_INPUT_TYPES,
  FAIRBOT_LABELS,
  FAIRBOT_UPLOAD,
} from '../../constants/fairbot.constants'
import type { FileUploadControls } from '../../hooks/useFileUpload'

interface FileUploadZoneProps {
  upload: FileUploadControls
  inputRef: RefObject<HTMLInputElement>
  children: ReactNode
  helperText?: string
  disabled?: boolean
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
  border: FAIRBOT_UPLOAD.BORDER_NONE,
  borderRadius: `${FAIRBOT_UPLOAD.BORDER_RADIUS}px`,
  padding: `${FAIRBOT_UPLOAD.PADDING}px`,
  minHeight: `${FAIRBOT_UPLOAD.MIN_HEIGHT}px`,
  backgroundColor: isDragging
    ? theme.palette.action.hover
    : theme.palette.background.default,
  transition: `${FAIRBOT_UPLOAD.TRANSITION_PROPERTIES} ${FAIRBOT_UPLOAD.TRANSITION_MS}ms ${FAIRBOT_UPLOAD.TRANSITION_EASING}`,
  opacity: isDisabled ? theme.palette.action.disabledOpacity : 1,
  pointerEvents: isDisabled ? 'none' : 'auto',
}))

const UploadSurface = styled('label')({
  display: 'flex',
  alignItems: 'center',
  gap: `${FAIRBOT_UPLOAD.GAP}px`,
  width: '100%',
  cursor: 'pointer',
})

const HiddenInput = styled('input')({
  display: 'none',
})

const HelperText = styled('p')(({ theme }) => ({
  ...theme.typography.body2,
  marginTop: `${FAIRBOT_UPLOAD.GAP}px`,
  marginLeft: `${FAIRBOT_UPLOAD.HELPER_TEXT_OFFSET_PX}px`,
  marginBottom: 0,
  color: theme.palette.text.secondary,
}))

const ErrorText = styled('p')(({ theme }) => ({
  ...theme.typography.body2,
  marginTop: `${FAIRBOT_UPLOAD.GAP}px`,
  marginBottom: 0,
  color: theme.palette.error.main,
}))

// Drag-and-drop file zone that wires to the shared upload controls.
export const FileUploadZone = ({
  upload,
  inputRef,
  children,
  helperText = FAIRBOT_LABELS.UPLOAD_TIP,
  disabled = false,
}: FileUploadZoneProps) => {
  const handleDragEnter = disabled ? undefined : upload.handleDragEnter
  const handleDragLeave = disabled ? undefined : upload.handleDragLeave
  const handleDragOver = disabled ? undefined : upload.handleDragOver
  const handleDrop = disabled ? undefined : upload.handleDrop
  const handleFileSelect = disabled ? undefined : upload.handleFileSelect

  return (
    <UploadContainer
      aria-label={FAIRBOT_ARIA.FILE_UPLOAD}
      isDragging={upload.isDragging}
      isDisabled={disabled}
      onDragEnter={handleDragEnter}
      onDragLeave={handleDragLeave}
      onDragOver={handleDragOver}
      onDrop={handleDrop}
    >
      <HiddenInput
        id={FAIRBOT_IDS.FILE_INPUT}
        ref={inputRef}
        type={FAIRBOT_INPUT_TYPES.FILE}
        accept={upload.acceptAttribute}
        onChange={handleFileSelect}
        disabled={disabled}
      />
      <UploadSurface htmlFor={FAIRBOT_IDS.FILE_INPUT}>{children}</UploadSurface>
      {helperText ? <HelperText>{helperText}</HelperText> : null}
      {upload.error ? <ErrorText role="alert">{upload.error.message}</ErrorText> : null}
    </UploadContainer>
  )
}

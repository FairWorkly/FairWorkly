import {
  useCallback,
  useMemo,
  useRef,
  useState,
  type ChangeEvent,
  type DragEvent,
  type RefObject,
} from 'react'
import {
  FAIRBOT_ERRORS,
  FAIRBOT_FILE,
  FAIRBOT_NUMBERS,
  FAIRBOT_TEXT,
} from '../constants/fairbot.constants'
import type { FairBotError, FairBotUploadState } from '../types/fairbot.types'

// Encapsulates drag/drop + file picker logic and validation for FairBot uploads.
export interface UseFileUploadOptions {
  onFileAccepted?: (file: File) => void
}

export interface FileUploadControls extends FairBotUploadState {
  acceptAttribute: string
  openFileDialog: () => void
  handleDragEnter: (event: DragEvent<HTMLElement>) => void
  handleDragLeave: (event: DragEvent<HTMLElement>) => void
  handleDragOver: (event: DragEvent<HTMLElement>) => void
  handleDrop: (event: DragEvent<HTMLElement>) => void
  handleFileSelect: (event: ChangeEvent<HTMLInputElement>) => void
  reset: () => void
}

export interface UseFileUploadResult {
  inputRef: RefObject<HTMLInputElement>
  controls: FileUploadControls
}

const getFileExtension = (fileName: string): string => {
  const segments = fileName.split(FAIRBOT_FILE.NAME_SEPARATOR)
  if (segments.length < FAIRBOT_NUMBERS.ONE) {
    return FAIRBOT_TEXT.EMPTY
  }

  return segments[segments.length - FAIRBOT_NUMBERS.ONE].toLowerCase()
}

const isAcceptedFileType = (file: File): boolean => {
  const extension = getFileExtension(file.name)
  if ((FAIRBOT_FILE.ACCEPTED_TYPES as readonly string[]).includes(extension)) {
    return true
  }

  if ((FAIRBOT_FILE.ACCEPTED_MIME as readonly string[]).includes(file.type)) {
    return true
  }

  return false
}

const validateFile = (file: File): FairBotError | null => {
  if (!isAcceptedFileType(file)) {
    return { message: FAIRBOT_ERRORS.INVALID_FILE_TYPE }
  }

  if (file.size > FAIRBOT_FILE.MAX_SIZE_BYTES) {
    return { message: FAIRBOT_ERRORS.FILE_TOO_LARGE }
  }

  return null
}

export const useFileUpload = (
  options: UseFileUploadOptions = {},
): UseFileUploadResult => {
  const { onFileAccepted } = options
  const inputRef = useRef<HTMLInputElement>(null)
  const [state, setState] = useState<FairBotUploadState>({
    isDragging: false,
    isUploading: false,
    error: null,
    acceptedFileTypes: [...FAIRBOT_FILE.ACCEPTED_TYPES],
  })

  const reset = useCallback(() => {
    setState((prev) => ({
      ...prev,
      isDragging: false,
      isUploading: false,
      error: null,
    }))
  }, [])

  const handleAcceptedFile = useCallback(
    (file: File) => {
      const validationError = validateFile(file)
      if (validationError) {
        // Surface validation errors through the shared upload state.
        setState((prev) => ({ ...prev, error: validationError }))
        return
      }

      setState((prev) => ({ ...prev, error: null, isUploading: false }))
      onFileAccepted?.(file)
    },
    [onFileAccepted],
  )

  const handleDragEnter = useCallback((event: DragEvent<HTMLElement>) => {
    event.preventDefault()
    event.stopPropagation()
    setState((prev) => ({ ...prev, isDragging: true }))
  }, [])

  const handleDragLeave = useCallback((event: DragEvent<HTMLElement>) => {
    event.preventDefault()
    event.stopPropagation()
    setState((prev) => ({ ...prev, isDragging: false }))
  }, [])

  const handleDragOver = useCallback((event: DragEvent<HTMLElement>) => {
    event.preventDefault()
    event.stopPropagation()
  }, [])

  const handleDrop = useCallback(
    (event: DragEvent<HTMLElement>) => {
      event.preventDefault()
      event.stopPropagation()

      setState((prev) => ({ ...prev, isDragging: false }))
      const files = event.dataTransfer.files
      if (files.length < FAIRBOT_NUMBERS.ONE) {
        setState((prev) => ({
          ...prev,
          error: { message: FAIRBOT_ERRORS.FILE_REQUIRED },
        }))
        return
      }

      const file = files[FAIRBOT_NUMBERS.ZERO]
      handleAcceptedFile(file)
    },
    [handleAcceptedFile],
  )

  const handleFileSelect = useCallback(
    (event: ChangeEvent<HTMLInputElement>) => {
      const files = event.target.files
      if (!files || files.length < FAIRBOT_NUMBERS.ONE) {
        setState((prev) => ({
          ...prev,
          error: { message: FAIRBOT_ERRORS.FILE_REQUIRED },
        }))
        return
      }

      const file = files[FAIRBOT_NUMBERS.ZERO]
      handleAcceptedFile(file)
      event.target.value = FAIRBOT_TEXT.EMPTY
    },
    [handleAcceptedFile],
  )

  const openFileDialog = useCallback(() => {
    inputRef.current?.click()
  }, [])

  const controls = useMemo(
    () => ({
      ...state,
      acceptAttribute: FAIRBOT_FILE.ACCEPT_ATTRIBUTE,
      openFileDialog,
      handleDragEnter,
      handleDragLeave,
      handleDragOver,
      handleDrop,
      handleFileSelect,
      reset,
    }),
    [
      handleDragEnter,
      handleDragLeave,
      handleDragOver,
      handleDrop,
      handleFileSelect,
      openFileDialog,
      reset,
      state,
    ],
  )

  return useMemo(
    () => ({
      inputRef,
      controls,
    }),
    [controls, inputRef],
  )
}

import {
  useCallback,
  useMemo,
  useRef,
  useState,
  type ChangeEvent,
  type DragEvent,
  type RefObject,
} from 'react'
import { CHAT_NUMBERS, CHAT_TEXT } from '../constants/chat.constants'
import type { ChatError, ChatUploadState, ChatFileConfig } from '../types/chat.types'

// Default file configuration (can be overridden).
const DEFAULT_FILE_CONFIG: ChatFileConfig = {
  acceptedTypes: ['csv', 'xlsx'],
  acceptedMime: [
    'text/csv',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ],
  maxSizeBytes: 50 * 1024 * 1024,
  maxSizeLabel: '50MB',
  acceptAttribute: '.csv,.xlsx',
}

// Default error messages (can be overridden).
const DEFAULT_ERRORS = {
  invalidFileType: 'Unsupported file type. Please upload a valid file.',
  fileTooLarge: 'File is too large.',
  fileRequired: 'Please select a file to continue.',
}

export interface FileUploadErrorMessages {
  invalidFileType?: string
  fileTooLarge?: string
  fileRequired?: string
}

export interface UseFileUploadOptions {
  onFileAccepted?: (file: File) => void
  fileConfig?: Partial<ChatFileConfig>
  errorMessages?: FileUploadErrorMessages
}

export interface FileUploadControls extends ChatUploadState {
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
  inputRef: RefObject<HTMLInputElement | null>
  controls: FileUploadControls
}

const getFileExtension = (fileName: string): string => {
  const segments = fileName.split('.')
  if (segments.length < CHAT_NUMBERS.ONE) {
    return CHAT_TEXT.EMPTY
  }

  return segments[segments.length - CHAT_NUMBERS.ONE].toLowerCase()
}

export const useFileUpload = (
  options: UseFileUploadOptions = {},
): UseFileUploadResult => {
  const { onFileAccepted, fileConfig, errorMessages } = options

  const config = useMemo<ChatFileConfig>(
    () => ({ ...DEFAULT_FILE_CONFIG, ...fileConfig }),
    [fileConfig],
  )

  const errors = useMemo(
    () => ({ ...DEFAULT_ERRORS, ...errorMessages }),
    [errorMessages],
  )

  const inputRef = useRef<HTMLInputElement | null>(null)
  const [state, setState] = useState<ChatUploadState>({
    isDragging: false,
    isUploading: false,
    error: null,
    acceptedFileTypes: [...config.acceptedTypes],
  })

  const isAcceptedFileType = useCallback(
    (file: File): boolean => {
      const extension = getFileExtension(file.name)
      if (config.acceptedTypes.includes(extension)) {
        return true
      }

      if (config.acceptedMime.includes(file.type)) {
        return true
      }

      return false
    },
    [config.acceptedTypes, config.acceptedMime],
  )

  const validateFile = useCallback(
    (file: File): ChatError | null => {
      if (!isAcceptedFileType(file)) {
        return { message: errors.invalidFileType }
      }

      if (file.size > config.maxSizeBytes) {
        return { message: `${errors.fileTooLarge} Max size is ${config.maxSizeLabel}.` }
      }

      return null
    },
    [isAcceptedFileType, errors, config.maxSizeBytes, config.maxSizeLabel],
  )

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
        setState((prev) => ({ ...prev, error: validationError }))
        return
      }

      setState((prev) => ({ ...prev, error: null, isUploading: false }))
      onFileAccepted?.(file)
    },
    [onFileAccepted, validateFile],
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
      if (files.length < CHAT_NUMBERS.ONE) {
        setState((prev) => ({
          ...prev,
          error: { message: errors.fileRequired },
        }))
        return
      }

      const file = files[CHAT_NUMBERS.ZERO]
      handleAcceptedFile(file)
    },
    [handleAcceptedFile, errors.fileRequired],
  )

  const handleFileSelect = useCallback(
    (event: ChangeEvent<HTMLInputElement>) => {
      const files = event.target.files
      if (!files || files.length < CHAT_NUMBERS.ONE) {
        setState((prev) => ({
          ...prev,
          error: { message: errors.fileRequired },
        }))
        return
      }

      const file = files[CHAT_NUMBERS.ZERO]
      handleAcceptedFile(file)
      event.target.value = CHAT_TEXT.EMPTY
    },
    [handleAcceptedFile, errors.fileRequired],
  )

  const openFileDialog = useCallback(() => {
    inputRef.current?.click()
  }, [])

  const controls = useMemo(
    () => ({
      ...state,
      acceptAttribute: config.acceptAttribute,
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
      config.acceptAttribute,
    ],
  )

  return useMemo(
    () => ({
      inputRef,
      controls,
    }),
    [controls],
  )
}

import { useRef, useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import {
  uploadRoster,
  type UploadRosterResponse,
  type ParserWarning,
} from '@/services/rosterApi'
import type { UploadedFile } from '@/shared/compliance-check'

export function useUploadRoster() {
  const navigate = useNavigate()
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([])
  const [warnings, setWarnings] = useState<ParserWarning[]>([])
  const actualFileRef = useRef<File | null>(null)

  const {
    mutate: upload,
    isPending,
    error: uploadError,
    reset,
  } = useApiMutation<UploadRosterResponse, File>({
    mutationFn: (file) => uploadRoster(file),
  })

  const handleFileUpload = useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      const file = event.target.files?.[0]
      if (!file) return

      actualFileRef.current = file

      const newFile: UploadedFile = {
        id: Date.now(),
        name: file.name,
        size: `${Math.ceil(file.size / 1024)} KB`,
        date: new Date().toISOString(),
        status: 'ready',
      }

      setUploadedFiles([newFile])
      reset()
      setWarnings([])
    },
    [reset],
  )

  const handleRemoveFile = useCallback(
    (id: number) => {
      setUploadedFiles((prev) => prev.filter((file) => file.id !== id))
      actualFileRef.current = null
      reset()
      setWarnings([])
    },
    [reset],
  )

  const handleStartAnalysis = useCallback(() => {
    if (!actualFileRef.current) return

    reset()
    setWarnings([])

    upload(actualFileRef.current, {
      onSuccess: (response) => {
        navigate(`/roster/results/${response.rosterId}`, {
          state: { warnings: response.warnings ?? [] },
        })
      },
    })
  }, [reset, upload, navigate])

  const handleCancel = useCallback(() => {
    setUploadedFiles([])
    actualFileRef.current = null
    reset()
    setWarnings([])
  }, [reset])

  return {
    uploadedFiles,
    warnings,
    isPending,
    uploadError: uploadError?.message ?? null,
    handleFileUpload,
    handleRemoveFile,
    handleStartAnalysis,
    handleCancel,
  }
}

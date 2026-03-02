import { useRef, useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { useApiMutation } from '@/shared/hooks/useApiMutation'
import { uploadRoster, type UploadRosterResponse } from '@/services/rosterApi'
import type { UploadedFile } from '@/shared/compliance-check'

export function useUploadRoster() {
  const navigate = useNavigate()
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([])
  const actualFileRef = useRef<File | null>(null)

  const {
    mutate: upload,
    isPending,
    error: uploadError,
    reset,
  } = useApiMutation<UploadRosterResponse, File>({
    mutationFn: file => uploadRoster(file),
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
    },
    [reset]
  )

  const handleRemoveFile = useCallback(
    (id: number) => {
      setUploadedFiles(prev => prev.filter(file => file.id !== id))
      actualFileRef.current = null
      reset()
    },
    [reset]
  )

  const handleStartAnalysis = useCallback(() => {
    if (!actualFileRef.current) return

    reset()

    upload(actualFileRef.current, {
      onSuccess: response => {
        navigate(`/roster/results/${response.rosterId}`)
      },
    })
  }, [reset, upload, navigate])

  const handleCancel = useCallback(() => {
    setUploadedFiles([])
    actualFileRef.current = null
    reset()
  }, [reset])

  return {
    uploadedFiles,
    isPending,
    uploadError: uploadError?.message ?? null,
    handleFileUpload,
    handleRemoveFile,
    handleStartAnalysis,
    handleCancel,
  }
}

import { useState, useCallback } from 'react'
import type { ValidationErrors } from '../types/companyProfile.types'

type ValidateFn<T> = (formData: T) => ValidationErrors

interface UseEditableCardOptions<T> {
  data: T
  onSave: (data: T) => Promise<boolean>
  validate: ValidateFn<T>
}

export function useEditableCard<T extends object>({
  data,
  onSave,
  validate,
}: UseEditableCardOptions<T>) {
  const [isEditing, setIsEditing] = useState(false)
  const [formData, setFormData] = useState<T>(data)
  const [errors, setErrors] = useState<ValidationErrors>({})

  const handleEdit = useCallback(() => {
    setFormData(data)
    setErrors({})
    setIsEditing(true)
  }, [data])

  const handleSave = useCallback(async () => {
    const newErrors = validate(formData)
    setErrors(newErrors)
    if (Object.keys(newErrors).length > 0) return

    const success = await onSave(formData)
    if (success) {
      setIsEditing(false)
      setErrors({})
    }
  }, [formData, onSave, validate])

  const handleCancel = useCallback(() => {
    setFormData(data)
    setErrors({})
    setIsEditing(false)
  }, [data])

  const handleChange = useCallback((field: keyof T, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }))
    setErrors(prev => {
      if (!prev[field as string]) return prev
      const next = { ...prev }
      delete next[field as string]
      return next
    })
  }, [])

  const hasErrors = Object.keys(errors).length > 0

  return {
    isEditing,
    formData,
    errors,
    hasErrors,
    handleEdit,
    handleSave,
    handleCancel,
    handleChange,
  }
}

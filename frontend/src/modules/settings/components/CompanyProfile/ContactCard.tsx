import { useState } from 'react'
import { TextField } from '@mui/material'
import { CompanyProfileCard } from './CompanyProfileCard'
import {
  FormRow,
  FieldLabel,
  FieldValue,
  ErrorText,
  FormField,
} from './CompanyProfile.styles'
import type { ContactInfo, ValidationErrors } from '../../types/companyProfile.types'

interface ContactCardProps {
  data: ContactInfo
  onSave: (data: ContactInfo) => void
}


export function ContactCard({ data, onSave }: ContactCardProps) {
  const [isEditing, setIsEditing] = useState(false)
  const [formData, setFormData] = useState<ContactInfo>(data)
  const [errors, setErrors] = useState<ValidationErrors>({})


  const validateEmail = (email: string): string => {
    if (!email) return 'Email is required'
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(email)) return 'Invalid email format'
    return ''
  }


  const validatePhone = (phone: string): string => {
    if (!phone) return 'Phone number is required'
    return ''
  }

  const validateForm = (): boolean => {
    const newErrors: ValidationErrors = {}

    const emailError = validateEmail(formData.contactEmail)
    if (emailError) newErrors.contactEmail = emailError

    const phoneError = validatePhone(formData.phoneNumber)
    if (phoneError) newErrors.phoneNumber = phoneError

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleEdit = () => {
    setFormData(data)
    setErrors({})
    setIsEditing(true)
  }

  const handleSave = () => {
    if (validateForm()) {
      onSave(formData)
      setIsEditing(false)
    }
  }

  const handleCancel = () => {
    setFormData(data)
    setErrors({})
    setIsEditing(false)
  }

  const handleChange = (field: keyof ContactInfo, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }))
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }))
    }
  }

  return (
    <CompanyProfileCard
      title="Contact"
      description="How we can reach your business"
      isEditing={isEditing}
      onEdit={handleEdit}
      onSave={handleSave}
      onCancel={handleCancel}
      isSaveDisabled={Object.keys(errors).length > 0}
    >

      <FormRow>
        <FieldLabel>Contact Email</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              type="email"
              value={formData.contactEmail}
              onChange={(e) => handleChange('contactEmail', e.target.value)}
              error={!!errors.contactEmail}
              placeholder="hello@company.com"
            />
            {errors.contactEmail && (
              <ErrorText>{errors.contactEmail}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.contactEmail}</FieldValue>
        )}
      </FormRow>

  
      <FormRow>
        <FieldLabel>Phone Number</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              type="tel"
              value={formData.phoneNumber}
              onChange={(e) => handleChange('phoneNumber', e.target.value)}
              error={!!errors.phoneNumber}
              placeholder="+61 400 000 000"
            />
            {errors.phoneNumber && (
              <ErrorText>{errors.phoneNumber}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.phoneNumber}</FieldValue>
        )}
      </FormRow>
    </CompanyProfileCard>
  )
}
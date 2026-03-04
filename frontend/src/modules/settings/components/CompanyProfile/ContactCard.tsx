import { TextField } from '@mui/material'
import { CompanyProfileCard } from './CompanyProfileCard'
import {
  FormRow,
  FieldLabel,
  FieldValue,
  ErrorText,
  FormField,
} from './CompanyProfile.styles'
import type {
  ContactInfo,
  ValidationErrors,
} from '../../types/companyProfile.types'
import { useEditableCard } from '../../hooks/useEditableCard'

interface ContactCardProps {
  data: ContactInfo
  onSave: (data: ContactInfo) => Promise<boolean>
  isSaving?: boolean
}

function validate(formData: ContactInfo): ValidationErrors {
  const errors: ValidationErrors = {}

  if (!formData.contactEmail) {
    errors.contactEmail = 'Email is required'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.contactEmail)) {
    errors.contactEmail = 'Invalid email format'
  }

  return errors
}

export function ContactCard({
  data,
  onSave,
  isSaving = false,
}: ContactCardProps) {
  const {
    isEditing,
    formData,
    errors,
    hasErrors,
    handleEdit,
    handleSave,
    handleCancel,
    handleChange,
  } = useEditableCard({ data, onSave, validate })

  return (
    <CompanyProfileCard
      title="Contact"
      isEditing={isEditing}
      isSaving={isSaving}
      onEdit={handleEdit}
      onSave={handleSave}
      onCancel={handleCancel}
      isSaveDisabled={hasErrors}
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
              onChange={e => handleChange('contactEmail', e.target.value)}
              error={!!errors.contactEmail}
              placeholder="hello@company.com"
              disabled={isSaving}
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
              onChange={e => handleChange('phoneNumber', e.target.value)}
              placeholder="+61 400 000 000"
              disabled={isSaving}
            />
          </FormField>
        ) : (
          <FieldValue>{data.phoneNumber || '—'}</FieldValue>
        )}
      </FormRow>
    </CompanyProfileCard>
  )
}

import { useState } from 'react'
import { TextField, MenuItem } from '@mui/material'
import { CompanyProfileCard } from './CompanyProfileCard'
import {
  FormRow,
  FieldLabel,
  FieldValue,
  ErrorText,
  FormField,
} from './CompanyProfile.styles'
import type { AddressInfo, ValidationErrors } from '../../types/companyProfile.types'
import { AUSTRALIAN_STATES } from '../../types/companyProfile.types'

interface AddressCardProps {
  data: AddressInfo
  onSave: (data: AddressInfo) => void
}


export function AddressCard({ data, onSave }: AddressCardProps) {
  const [isEditing, setIsEditing] = useState(false)
  const [formData, setFormData] = useState<AddressInfo>(data)
  const [errors, setErrors] = useState<ValidationErrors>({})


  const validatePostcode = (postcode: string): string => {
    if (!postcode) return 'Postcode is required'
    if (!/^\d{4}$/.test(postcode)) return 'Postcode must be exactly 4 digits'
    return ''
  }

  const validateForm = (): boolean => {
    const newErrors: ValidationErrors = {}

    if (!formData.addressLine1.trim()) {
      newErrors.addressLine1 = 'Address line 1 is required'
    }

    if (!formData.suburb.trim()) {
      newErrors.suburb = 'Suburb is required'
    }

    if (!formData.state) {
      newErrors.state = 'State is required'
    }

    const postcodeError = validatePostcode(formData.postcode)
    if (postcodeError) {
      newErrors.postcode = postcodeError
    }

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

  const handleChange = (field: keyof AddressInfo, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }))
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }))
    }
  }

  return (
    <CompanyProfileCard
      title="Address"
      description="Your business location"
      isEditing={isEditing}
      onEdit={handleEdit}
      onSave={handleSave}
      onCancel={handleCancel}
      isSaveDisabled={Object.keys(errors).length > 0}
    >

      <FormRow>
        <FieldLabel>Address Line 1</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              value={formData.addressLine1}
              onChange={(e) => handleChange('addressLine1', e.target.value)}
              error={!!errors.addressLine1}
              placeholder="123 Main Street"
            />
            {errors.addressLine1 && (
              <ErrorText>{errors.addressLine1}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.addressLine1}</FieldValue>
        )}
      </FormRow>

      <FormRow>
        <FieldLabel>Address Line 2</FieldLabel>
        {isEditing ? (
          <TextField
            fullWidth
            size="small"
            value={formData.addressLine2}
            onChange={(e) => handleChange('addressLine2', e.target.value)}
            placeholder="Suite 100 (optional)"
          />
        ) : (
          <FieldValue>{data.addressLine2 || '—'}</FieldValue>
        )}
      </FormRow>

  
      <FormRow>
        <FieldLabel>Suburb</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              value={formData.suburb}
              onChange={(e) => handleChange('suburb', e.target.value)}
              error={!!errors.suburb}
              placeholder="Melbourne"
            />
            {errors.suburb && (
              <ErrorText>{errors.suburb}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.suburb}</FieldValue>
        )}
      </FormRow>


      <FormRow>
        <FieldLabel>State</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              select
              size="small"
              value={formData.state}
              onChange={(e) => handleChange('state', e.target.value)}
              error={!!errors.state}
            >
              {AUSTRALIAN_STATES.map((state) => (
                <MenuItem key={state.value} value={state.value}>
                  {state.label}
                </MenuItem>
              ))}
            </TextField>
            {errors.state && (
              <ErrorText>{errors.state}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>
            {AUSTRALIAN_STATES.find(s => s.value === data.state)?.label || data.state}
          </FieldValue>
        )}
      </FormRow>


      <FormRow>
        <FieldLabel>Postcode</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              value={formData.postcode}
              onChange={(e) => handleChange('postcode', e.target.value)}
              error={!!errors.postcode}
              placeholder="3000"
              helperText="Must be 4 digits"
              inputProps={{ maxLength: 4 }}
            />
            {errors.postcode && (
              <ErrorText>{errors.postcode}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.postcode}</FieldValue>
        )}
      </FormRow>
    </CompanyProfileCard>
  )
}
// modules/settings/components/CompanyProfile/BusinessInfoCard.tsx

import { useState } from 'react'
import { TextField, MenuItem } from '@mui/material'
import { Business as BusinessIcon } from '@mui/icons-material'
import { CompanyProfileCard } from './CompanyProfileCard'
import {
  FormRow,
  FieldLabel,
  FieldValue,
  LogoPlaceholder,
  ErrorText,
  FormField,
} from './CompanyProfile.styles'
import type { BusinessInfo, ValidationErrors } from '../../types/companyProfile.types'
import { INDUSTRY_TYPES } from '../../types/companyProfile.types'

interface BusinessInfoCardProps {
  data: BusinessInfo
  onSave: (data: BusinessInfo) => void
}

export function BusinessInfoCard({ data, onSave }: BusinessInfoCardProps) {
  const [isEditing, setIsEditing] = useState(false)
  
  const [formData, setFormData] = useState<BusinessInfo>(data)
  
  const [errors, setErrors] = useState<ValidationErrors>({})



  const validateABN = (abn: string): string => {
    if (!abn) return 'ABN is required'
    if (!/^\d{11}$/.test(abn)) return 'ABN must be exactly 11 digits'
    return ''
  }


  const validateForm = (): boolean => {
    const newErrors: ValidationErrors = {}

    if (!formData.companyName.trim()) {
      newErrors.companyName = 'Company name is required'
    }

    const abnError = validateABN(formData.abn)
    if (abnError) {
      newErrors.abn = abnError
    }

    if (!formData.industryType) {
      newErrors.industryType = 'Industry type is required'
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


  const handleChange = (field: keyof BusinessInfo, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }))
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }))
    }
  }

  return (
    <CompanyProfileCard
      title="Business Info"
      description="Your company's basic information"
      isEditing={isEditing}
      onEdit={handleEdit}
      onSave={handleSave}
      onCancel={handleCancel}
      isSaveDisabled={Object.keys(errors).length > 0}
    >
      <FormRow>
        <FieldLabel>Logo</FieldLabel>
        <LogoPlaceholder>
          <BusinessIcon/>
        </LogoPlaceholder>
      </FormRow>


      <FormRow>
        <FieldLabel>Company Name</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              value={formData.companyName}
              onChange={(e) => handleChange('companyName', e.target.value)}
              error={!!errors.companyName}
              placeholder="Enter company name"
            />
            {errors.companyName && (
              <ErrorText>{errors.companyName}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.companyName}</FieldValue>
        )}
      </FormRow>

  
      <FormRow>
        <FieldLabel>ABN</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              value={formData.abn}
              onChange={(e) => handleChange('abn', e.target.value)}
              error={!!errors.abn}
              placeholder="12345678901"
              helperText="Must be 11 digits"
              inputProps={{ maxLength: 11 }}
            />
            {errors.abn && (
              <ErrorText>{errors.abn}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.abn}</FieldValue>
        )}
      </FormRow>

      <FormRow>
        <FieldLabel>Industry Type</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              select
              size="small"
              value={formData.industryType}
              onChange={(e) => handleChange('industryType', e.target.value)}
              error={!!errors.industryType}
            >
              {INDUSTRY_TYPES.map((type) => (
                <MenuItem key={type} value={type}>
                  {type}
                </MenuItem>
              ))}
            </TextField>
            {errors.industryType && (
              <ErrorText>{errors.industryType}</ErrorText>
            )}
          </FormField>
        ) : (
          <FieldValue>{data.industryType}</FieldValue>
        )}
      </FormRow>
    </CompanyProfileCard>
  )
}
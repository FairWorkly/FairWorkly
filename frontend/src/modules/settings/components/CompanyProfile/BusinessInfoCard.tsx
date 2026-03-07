import { TextField, MenuItem } from '@mui/material'
import { CompanyProfileCard } from './CompanyProfileCard'
import {
  FormRow,
  FieldLabel,
  FieldValue,
  ErrorText,
  FormField,
} from './CompanyProfile.styles'
import type { BusinessInfo, ValidationErrors } from '../../types/companyProfile.types'
import { INDUSTRY_TYPES, AWARD_TYPES, suggestAwardForIndustry } from '../../types/companyProfile.types'
import { useEditableCard } from '../../hooks/useEditableCard'

interface BusinessInfoCardProps {
  data: BusinessInfo
  onSave: (data: BusinessInfo) => Promise<boolean>
  isSaving?: boolean
}

function validate(formData: BusinessInfo): ValidationErrors {
  const errors: ValidationErrors = {}

  if (!formData.companyName.trim()) {
    errors.companyName = 'Company name is required'
  }

  if (!formData.abn) {
    errors.abn = 'ABN is required'
  } else if (!/^\d{11}$/.test(formData.abn)) {
    errors.abn = 'ABN must be exactly 11 digits'
  }

  if (!formData.industryType) {
    errors.industryType = 'Industry type is required'
  }

  return errors
}

export function BusinessInfoCard({
  data,
  onSave,
  isSaving = false,
}: BusinessInfoCardProps) {
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

  function handleIndustryChange(value: string) {
    handleChange('industryType', value)
    const suggested = suggestAwardForIndustry(value)
    if (suggested) {
      handleChange('primaryAward', suggested)
    }
  }

  const primaryAwardLabel = AWARD_TYPES.find(a => a.value === data.primaryAward)?.label ?? null

  return (
    <CompanyProfileCard
      title="Business Info"
      isEditing={isEditing}
      isSaving={isSaving}
      onEdit={handleEdit}
      onSave={handleSave}
      onCancel={handleCancel}
      isSaveDisabled={hasErrors}
    >
      <FormRow>
        <FieldLabel>Company Name</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              value={formData.companyName}
              onChange={e => handleChange('companyName', e.target.value)}
              error={!!errors.companyName}
              placeholder="Enter company name"
              disabled={isSaving}
            />
            {errors.companyName && <ErrorText>{errors.companyName}</ErrorText>}
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
              onChange={e => handleChange('abn', e.target.value)}
              error={!!errors.abn}
              placeholder="12345678901"
              helperText="Must be 11 digits"
              disabled={isSaving}
              slotProps={{ htmlInput: { maxLength: 11, inputMode: 'numeric' } }}
            />
            {errors.abn && <ErrorText>{errors.abn}</ErrorText>}
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
              onChange={(e) => handleIndustryChange(e.target.value)}
              error={!!errors.industryType}
              disabled={isSaving}
            >
              {INDUSTRY_TYPES.map(type => (
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

      <FormRow>
        <FieldLabel>Applicable Award</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              select
              size="small"
              value={formData.primaryAward ?? ''}
              onChange={(e) => {
                const match = AWARD_TYPES.find(a => a.value === e.target.value)
                handleChange('primaryAward', match ? match.value : null)
              }}
              disabled={isSaving}
            >
              <MenuItem value=""><em>Not specified</em></MenuItem>
              {AWARD_TYPES.map((award) => (
                <MenuItem key={award.value} value={award.value}>
                  {award.label} ({award.maCode})
                </MenuItem>
              ))}
            </TextField>
          </FormField>
        ) : (
          <FieldValue>{primaryAwardLabel ?? 'Not specified'}</FieldValue>
        )}
      </FormRow>
    </CompanyProfileCard>
  )
}

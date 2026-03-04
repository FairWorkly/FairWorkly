import { TextField, MenuItem } from '@mui/material'
import { CompanyProfileCard } from './CompanyProfileCard'
import {
  FormRow,
  FieldLabel,
  FieldValue,
  ErrorText,
  FormField,
} from './CompanyProfile.styles'
import type {
  AddressInfo,
  ValidationErrors,
} from '../../types/companyProfile.types'
import { AUSTRALIAN_STATES } from '../../types/companyProfile.types'
import { useEditableCard } from '../../hooks/useEditableCard'

interface AddressCardProps {
  data: AddressInfo
  onSave: (data: AddressInfo) => Promise<boolean>
  isSaving?: boolean
}

function validate(formData: AddressInfo): ValidationErrors {
  const errors: ValidationErrors = {}

  if (!formData.addressLine1.trim()) {
    errors.addressLine1 = 'Address line 1 is required'
  }

  if (!formData.suburb.trim()) {
    errors.suburb = 'Suburb is required'
  }

  if (!formData.state) {
    errors.state = 'State is required'
  }

  if (!formData.postcode) {
    errors.postcode = 'Postcode is required'
  } else if (!/^\d{4}$/.test(formData.postcode)) {
    errors.postcode = 'Postcode must be exactly 4 digits'
  }

  return errors
}

export function AddressCard({
  data,
  onSave,
  isSaving = false,
}: AddressCardProps) {
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
      title="Address"
      isEditing={isEditing}
      isSaving={isSaving}
      onEdit={handleEdit}
      onSave={handleSave}
      onCancel={handleCancel}
      isSaveDisabled={hasErrors}
    >
      <FormRow>
        <FieldLabel>Address Line 1</FieldLabel>
        {isEditing ? (
          <FormField>
            <TextField
              fullWidth
              size="small"
              value={formData.addressLine1}
              onChange={e => handleChange('addressLine1', e.target.value)}
              error={!!errors.addressLine1}
              placeholder="123 Main Street"
              disabled={isSaving}
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
            onChange={e => handleChange('addressLine2', e.target.value)}
            placeholder="Suite 100 (optional)"
            disabled={isSaving}
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
              onChange={e => handleChange('suburb', e.target.value)}
              error={!!errors.suburb}
              placeholder="Melbourne"
              disabled={isSaving}
            />
            {errors.suburb && <ErrorText>{errors.suburb}</ErrorText>}
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
              onChange={e => handleChange('state', e.target.value)}
              error={!!errors.state}
              disabled={isSaving}
            >
              {AUSTRALIAN_STATES.map(state => (
                <MenuItem key={state.value} value={state.value}>
                  {state.label}
                </MenuItem>
              ))}
            </TextField>
            {errors.state && <ErrorText>{errors.state}</ErrorText>}
          </FormField>
        ) : (
          <FieldValue>
            {AUSTRALIAN_STATES.find(s => s.value === data.state)?.label ||
              data.state}
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
              onChange={e => handleChange('postcode', e.target.value)}
              error={!!errors.postcode}
              placeholder="3000"
              helperText="Must be 4 digits"
              disabled={isSaving}
              slotProps={{ htmlInput: { maxLength: 4, inputMode: 'numeric' } }}
            />
            {errors.postcode && <ErrorText>{errors.postcode}</ErrorText>}
          </FormField>
        ) : (
          <FieldValue>{data.postcode}</FieldValue>
        )}
      </FormRow>
    </CompanyProfileCard>
  )
}

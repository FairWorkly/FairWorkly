import { CircularProgress } from '@mui/material'
import { Edit as EditIcon } from '@mui/icons-material'
import {
  ProfileCard,
  CardHeader,
  CardContent,
  ButtonContainer,
  CancelButton,
  EditButton,
  SaveButton,
  CardTitle,
} from './CompanyProfile.styles'
import type { ReactNode } from 'react'

interface CompanyProfileCardProps {
  title: string
  isEditing: boolean
  isSaving?: boolean
  onEdit: () => void
  onSave: () => void
  onCancel: () => void
  children: ReactNode
  isSaveDisabled?: boolean
}


export function CompanyProfileCard({
  title,
  isEditing,
  isSaving = false,
  onEdit,
  onSave,
  onCancel,
  children,
  isSaveDisabled = false,
}: CompanyProfileCardProps) {
  return (
    <ProfileCard>
      <CardHeader>
        <CardTitle>
          {title}
        </CardTitle>

        {isEditing ? (
          <ButtonContainer>
            <CancelButton
              variant="outlined"
              onClick={onCancel}
              disabled={isSaving}
            >
              Cancel
            </CancelButton>
            <SaveButton
              variant="contained"
              onClick={onSave}
              disabled={isSaveDisabled || isSaving}
              startIcon={isSaving ? <CircularProgress size={16} /> : undefined}
            >
              {isSaving ? 'Saving...' : 'Save'}
            </SaveButton>
          </ButtonContainer>
        ) : (
          <EditButton
            variant="outlined"
            startIcon={<EditIcon />}
            onClick={onEdit}
          >
            Edit
          </EditButton>
        )}
      </CardHeader>

      <CardContent>
        {children}
      </CardContent>
    </ProfileCard>
  )
}
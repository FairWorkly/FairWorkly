import { Edit as EditIcon } from '@mui/icons-material'
import {
  ProfileCard,
  CardHeader,
  CardContent,
  ButtonContainer,
  CardHeaderContent,
  CancelButton,
  EditButton,
  SaveButton,
  CardDescription,
  CardTitle,
} from './CompanyProfile.styles'
import type { ReactNode } from 'react'

interface CompanyProfileCardProps {
  title: string
  description?: string
  isEditing: boolean
  onEdit: () => void
  onSave: () => void
  onCancel: () => void
  children: ReactNode
  isSaveDisabled?: boolean
}


export function CompanyProfileCard({
  title,
  description,
  isEditing,
  onEdit,
  onSave,
  onCancel,
  children,
  isSaveDisabled = false,
}: CompanyProfileCardProps) {
  return (
    <ProfileCard>
      <CardHeader>
        <CardHeaderContent>
          <CardTitle>
            {title}
          </CardTitle>
          {description && (
            <CardDescription>
              {description}
            </CardDescription>
          )}
        </CardHeaderContent>

        {isEditing ? (
          <ButtonContainer>
            <CancelButton
              variant="outlined"
              onClick={onCancel}
            >
              Cancel
            </CancelButton>
            <SaveButton
              variant="contained"
              onClick={onSave}
              disabled={isSaveDisabled}
            >
              Save
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
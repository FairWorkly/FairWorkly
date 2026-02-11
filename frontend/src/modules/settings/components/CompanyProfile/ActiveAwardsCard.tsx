import { useState } from 'react'
import { IconButton } from '@mui/material'
import { Add as AddIcon, Delete as DeleteIcon } from '@mui/icons-material'
import { CompanyProfileCard } from './CompanyProfileCard'
import { AddAwardDialog } from './AddAwardDialog'
import {
    AwardItem,
    AwardInfo,
    AwardMeta,
    BadgeContainer,
    AddAwardButton,
    AwardDateText,
    AwardMetaText,
    AwardTitleText,
    EmptyStateText,
} from './CompanyProfile.styles'
import type { Award } from '../../types/companyProfile.types'
import type { AwardType } from '@/shared/compliance-check/components/AwardSelector'

interface ActiveAwardsCardProps {
    awards: Award[]
    onAddAward?: (awardType: AwardType, employeeCount: number) => void
    onDeleteAward?: (awardId: string) => void
}


export function ActiveAwardsCard({
    awards,
    onAddAward,
    onDeleteAward,
}: ActiveAwardsCardProps) {

    const [isEditing, setIsEditing] = useState(false)

    const [isDialogOpen, setIsDialogOpen] = useState(false)


    const formatDate = (dateString: string): string => {
        const date = new Date(dateString)
        return date.toLocaleDateString('en-AU', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
        })
    }


    const handleEdit = () => {
        setIsEditing(true)
    }

    const handleSave = () => {
        setIsEditing(false)
    }

    const handleCancel = () => {
        setIsEditing(false)
    }


    const handleDelete = (awardId: string) => {
        if (onDeleteAward) {
            onDeleteAward(awardId)
        }
    }


    const handleAddAward = (awardType: AwardType, employeeCount: number) => {
        if (onAddAward) {
            onAddAward(awardType, employeeCount)
        }
    }

    return (
        <>
            <CompanyProfileCard
                title="Active Awards"
                description="Award rules applied to your employees"
                isEditing={isEditing}
                onEdit={handleEdit}
                onSave={handleSave}
                onCancel={handleCancel}
            >
                {awards.length === 0 ? (
                    <EmptyStateText>
                        No awards configured yet. Add your first award to get started.
                    </EmptyStateText>
                ) : (
                    awards.map((award) => (
                        <AwardItem key={award.id}>
                            <AwardInfo>
                                <AwardTitleText>
                                    {award.awardType}
                                </AwardTitleText>
                                {award.isPrimary && (
                                    <BadgeContainer>Primary</BadgeContainer>
                                )}
                            </AwardInfo>


                            {isEditing ? (
                                <IconButton
                                    size="small"
                                    color="error"
                                    onClick={() => handleDelete(award.id)}
                                    aria-label="Delete award"
                                >
                                    <DeleteIcon />
                                </IconButton>
                            ) : (
                                <AwardMeta>
                                    <AwardMetaText>
                                        {award.employeeCount} {award.employeeCount === 1 ? 'employee' : 'employees'}
                                    </AwardMetaText>
                                    <AwardDateText>Added {formatDate(award.addedAt)}</AwardDateText>
                                </AwardMeta>
                            )}
                        </AwardItem>
                    ))
                )}


                {!isEditing && (
                    <AddAwardButton
                        variant='outlined'
                        startIcon={<AddIcon />}
                        onClick={() => setIsDialogOpen(true)}
                    >
                        Add Award
                    </AddAwardButton>
                )}
            </CompanyProfileCard>


            <AddAwardDialog
                open={isDialogOpen}
                onClose={() => setIsDialogOpen(false)}
                onAdd={handleAddAward}
            />
        </>
    )
}
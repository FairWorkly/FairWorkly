// modules/settings/components/CompanyProfile/ActiveAwardsCard.tsx

import { useState } from 'react'
import { Typography, IconButton } from '@mui/material'
import { Add as AddIcon, Delete as DeleteIcon } from '@mui/icons-material'
import { CompanyProfileCard } from './CompanyProfileCard'
import { AddAwardDialog } from './AddAwardDialog'
import {
    AwardItem,
    AwardInfo,
    AwardMeta,
    BadgeContainer,
    AddAwardButton,
} from './CompanyProfile.styles'
import type { Award } from '../../types/companyProfile.types'
import type { AwardType } from '@/shared/compliance-check/components/AwardSelector'

interface ActiveAwardsCardProps {
    awards: Award[]
    onAddAward?: (awardType: AwardType, employeeCount: number) => void
    onDeleteAward?: (awardId: string) => void
}

/**
 * Active Awards卡片
 * 
 * 功能：
 * - 查看模式：显示已配置的 Awards 列表
 * - 编辑模式：每个 Award 显示删除按钮
 * - 点击 Add Award：打开对话框，使用共享的 AwardSelector 选择新Award
 * 
 * MVP阶段：只更新本地state，不调用API
 */
export function ActiveAwardsCard({ 
    awards, 
    onAddAward,
    onDeleteAward,
}: ActiveAwardsCardProps) {
    // 编辑状态
    const [isEditing, setIsEditing] = useState(false)
    
    // 对话框开关状态
    const [isDialogOpen, setIsDialogOpen] = useState(false)

    /**
     * 格式化日期显示
     */
    const formatDate = (dateString: string): string => {
        const date = new Date(dateString)
        return date.toLocaleDateString('en-AU', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
        })
    }

    /**
     * Award类型到显示名称的映射
     */
    const getAwardDisplayName = (awardType: string): string => {
        const mapping: Record<string, string> = {
            'retail': 'General Retail Industry Award',
            'hospitality': 'Hospitality Industry Award',
            'clerks': 'Clerks Private Sector Award',
        }
        return mapping[awardType.toLowerCase()] || awardType
    }

    /**
     * 开始编辑
     */
    const handleEdit = () => {
        setIsEditing(true)
    }

    /**
     * 保存更改（实际上编辑模式下只有删除操作，没有需要保存的内容）
     */
    const handleSave = () => {
        setIsEditing(false)
    }

    /**
     * 取消编辑
     */
    const handleCancel = () => {
        setIsEditing(false)
    }

    /**
     * 处理删除 Award
     */
    const handleDelete = (awardId: string) => {
        if (onDeleteAward) {
            onDeleteAward(awardId)
        }
    }

    /**
     * 处理添加 Award
     */
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
                {/* Awards列表 */}
                {awards.length === 0 ? (
                    // 空状态
                    <Typography variant="body2" color="text.secondary" sx={{ py: 2 }}>
                        No awards configured yet. Add your first award to get started.
                    </Typography>
                ) : (
                    awards.map((award) => (
                        <AwardItem key={award.id}>
                            {/* 左侧：Award信息 */}
                            <AwardInfo>
                                <Typography variant="subtitle2">
                                    {getAwardDisplayName(award.awardType)}
                                </Typography>
                                {award.isPrimary && (
                                    <BadgeContainer>Primary</BadgeContainer>
                                )}
                            </AwardInfo>

                            {/* 右侧：元数据或删除按钮 */}
                            {isEditing ? (
                                // 编辑模式：显示删除按钮
                                <IconButton
                                    size="small"
                                    color="error"
                                    onClick={() => handleDelete(award.id)}
                                    aria-label="Delete award"
                                >
                                    <DeleteIcon />
                                </IconButton>
                            ) : (
                                // 查看模式：显示元数据
                                <AwardMeta>
                                    <Typography variant="body2">
                                        {award.employeeCount} {award.employeeCount === 1 ? 'employee' : 'employees'}
                                    </Typography>
                                    <Typography variant="caption">
                                        Added {formatDate(award.addedAt)}
                                    </Typography>
                                </AwardMeta>
                            )}
                        </AwardItem>
                    ))
                )}

                {/* Add Award按钮 - 只在非编辑模式显示 */}
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

            {/* Add Award对话框 - 内部使用共享的 AwardSelector */}
            <AddAwardDialog
                open={isDialogOpen}
                onClose={() => setIsDialogOpen(false)}
                onAdd={handleAddAward}
            />
        </>
    )
}
// modules/settings/components/CompanyProfile/ActiveAwardsCard.tsx

import { useState } from 'react'
import { Typography, Button } from '@mui/material'
import { Add as AddIcon } from '@mui/icons-material'
import { CompanyProfileCard } from './CompanyProfileCard'
import { AddAwardDialog } from './AddAwardDialog'
import {
    AwardItem,
    AwardInfo,
    AwardMeta,
    BadgeContainer,
} from './CompanyProfile.styles'
import type { Award } from '../../types/companyProfile.types'
import type { AwardType } from '@/shared/compliance-check/components/AwardSelector'

interface ActiveAwardsCardProps {
    awards: Award[]
    onAddAward?: (awardType: AwardType, employeeCount: number) => void
}

/**
 * Active Awards卡片
 * 
 * 显示公司使用的Award规则列表
 * 
 * 功能：
 * - 展示已配置的 Awards（类型、Primary badge、员工数、添加日期）
 * - 点击"Add Award"打开对话框，使用共享的 AwardSelector 选择新Award
 * 
 * MVP阶段：只是UI演示，不调用API
 */
export function ActiveAwardsCard({ awards, onAddAward }: ActiveAwardsCardProps) {
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
     * 处理添加 Award
     * MVP阶段：只是console.log
     * 未来：调用 onAddAward 回调，更新后端
     */
    const handleAddAward = (awardType: AwardType, employeeCount: number) => {
        console.log('Add Award:', { awardType, employeeCount })

        // MVP阶段：可以在这里更新本地state来演示效果
        if (onAddAward) {
            onAddAward(awardType, employeeCount)
        }

        // TODO: 未来调用 API
        // await organizationApi.addAward({ awardType, employeeCount })
    }

    return (
        <>
            <CompanyProfileCard
                title="Active Awards"
                description="Award rules applied to your employees"
                isEditing={false}
                onEdit={() => { }} // Awards卡片不使用卡片级别的编辑模式
                onSave={() => { }}
                onCancel={() => { }}
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
                                <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                    <Typography variant="subtitle2">
                                        {getAwardDisplayName(award.awardType)}
                                    </Typography>
                                    {award.isPrimary && (
                                        <BadgeContainer>Primary</BadgeContainer>
                                    )}
                                </div>
                            </AwardInfo>

                            {/* 右侧：元数据 */}
                            <AwardMeta>
                                <Typography variant="body2">
                                    {award.employeeCount} {award.employeeCount === 1 ? 'employee' : 'employees'}
                                </Typography>
                                <Typography variant="caption">
                                    Added {formatDate(award.addedAt)}
                                </Typography>
                            </AwardMeta>
                        </AwardItem>
                    ))
                )}

                {/* Add Award按钮 - 打开对话框 */}
                <Button
                    variant="outlined"
                    startIcon={<AddIcon />}
                    onClick={() => setIsDialogOpen(true)}
                    sx={{ mt: 1 }}
                >
                    Add Award
                </Button>
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
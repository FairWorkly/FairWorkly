// modules/settings/components/CompanyProfile/CompanyProfileCard.tsx

import { Typography, Button } from '@mui/material'
import { Edit as EditIcon } from '@mui/icons-material'
import {
  ProfileCard,
  CardHeader,
  CardContent,
  ActionButtons,
} from './CompanyProfile.styles'
import type { ReactNode } from 'react'

interface CompanyProfileCardProps {
  /** 卡片标题 */
  title: string
  /** 卡片描述（可选） */
  description?: string
  /** 是否处于编辑模式 */
  isEditing: boolean
  /** 点击编辑按钮的回调 */
  onEdit: () => void
  /** 点击保存按钮的回调 */
  onSave: () => void
  /** 点击取消按钮的回调 */
  onCancel: () => void
  /** 卡片内容 - 传入不同的表单或显示内容 */
  children: ReactNode
  /** 保存按钮是否禁用（例如：表单验证失败时） */
  isSaveDisabled?: boolean
}

/**
 * 可复用的公司资料卡片容器
 * 
 * 功能：
 * 1. 提供统一的卡片外观和布局
 * 2. 管理编辑/查看模式切换的UI
 * 3. 显示标题、描述和操作按钮
 * 
 * 使用场景：
 * - BusinessInfoCard
 * - ContactCard
 * - AddressCard
 * - ActiveAwardsCard
 */
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
      {/* 卡片头部：标题 + 编辑/保存/取消按钮 */}
      <CardHeader>
        <div>
          <Typography variant="h6" component="h3" gutterBottom={!!description}>
            {title}
          </Typography>
          {description && (
            <Typography variant="body2" color="text.secondary">
              {description}
            </Typography>
          )}
        </div>

        {/* 编辑模式：显示保存/取消按钮 */}
        {isEditing ? (
          <ActionButtons>
            <Button
              variant="outlined"
              size="small"
              onClick={onCancel}
            >
              Cancel
            </Button>
            <Button
              variant="contained"
              size="small"
              onClick={onSave}
              disabled={isSaveDisabled}
            >
              Save
            </Button>
          </ActionButtons>
        ) : (
          // 查看模式：显示编辑按钮
          <Button
            variant="outlined"
            size="small"
            startIcon={<EditIcon />}
            onClick={onEdit}
          >
            Edit
          </Button>
        )}
      </CardHeader>

      {/* 卡片内容：由父组件传入的children */}
      <CardContent>
        {children}
      </CardContent>
    </ProfileCard>
  )
}
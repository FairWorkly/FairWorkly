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
} from './CompanyProfile.styles'
import type { BusinessInfo, ValidationErrors } from '../../types/companyProfile.types'
import { INDUSTRY_TYPES } from '../../types/companyProfile.types'

interface BusinessInfoCardProps {
  /** 初始数据 */
  data: BusinessInfo
  /** 保存回调 - 将更新后的数据传递给父组件 */
  onSave: (data: BusinessInfo) => void
}

/**
 * 商业信息卡片
 * 
 * 包含字段：
 * - Logo (占位符，MVP阶段不实现上传功能)
 * - Company Name
 * - ABN (11位数字，必须验证)
 * - Industry Type (下拉选择)
 * 
 * 验证规则：
 * - Company Name: 必填
 * - ABN: 必须是11位数字
 * - Industry Type: 必填
 */
export function BusinessInfoCard({ data, onSave }: BusinessInfoCardProps) {
  // 编辑状态
  const [isEditing, setIsEditing] = useState(false)
  
  // 表单数据 - 使用独立state管理编辑中的数据
  const [formData, setFormData] = useState<BusinessInfo>(data)
  
  // 验证错误
  const [errors, setErrors] = useState<ValidationErrors>({})

  /**
   * 验证ABN格式
   * ABN必须是恰好11位数字
   */
  const validateABN = (abn: string): string => {
    if (!abn) return 'ABN is required'
    if (!/^\d{11}$/.test(abn)) return 'ABN must be exactly 11 digits'
    return ''
  }

  /**
   * 验证所有字段
   * 返回true表示验证通过
   */
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

  /**
   * 开始编辑
   * 重置表单数据为初始值，清空错误
   */
  const handleEdit = () => {
    setFormData(data)
    setErrors({})
    setIsEditing(true)
  }

  /**
   * 保存更改
   * 验证通过后调用父组件的onSave回调
   */
  const handleSave = () => {
    if (validateForm()) {
      onSave(formData)
      setIsEditing(false)
    }
  }

  /**
   * 取消编辑
   * 恢复为初始数据，清空错误
   */
  const handleCancel = () => {
    setFormData(data)
    setErrors({})
    setIsEditing(false)
  }

  /**
   * 字段变更处理
   */
  const handleChange = (field: keyof BusinessInfo, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }))
    // 清除该字段的错误
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
      {/* Logo 占位符 - MVP阶段不实现上传功能 */}
      <FormRow>
        <FieldLabel>Logo</FieldLabel>
        <LogoPlaceholder>
          <BusinessIcon sx={{ fontSize: 40, opacity: 0.3 }} />
        </LogoPlaceholder>
      </FormRow>

      {/* Company Name */}
      <FormRow>
        <FieldLabel>Company Name</FieldLabel>
        {isEditing ? (
          <div>
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
          </div>
        ) : (
          <FieldValue>{data.companyName}</FieldValue>
        )}
      </FormRow>

      {/* ABN */}
      <FormRow>
        <FieldLabel>ABN</FieldLabel>
        {isEditing ? (
          <div>
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
          </div>
        ) : (
          <FieldValue>{data.abn}</FieldValue>
        )}
      </FormRow>

      {/* Industry Type */}
      <FormRow>
        <FieldLabel>Industry Type</FieldLabel>
        {isEditing ? (
          <div>
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
          </div>
        ) : (
          <FieldValue>{data.industryType}</FieldValue>
        )}
      </FormRow>
    </CompanyProfileCard>
  )
}
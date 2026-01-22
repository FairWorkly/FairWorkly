// modules/settings/components/CompanyProfile/CompanyProfile.styles.ts

import { Box, Paper} from '@mui/material'
import { styled } from '@/styles/styled'

/**
 * 主容器 - Company Profile区域的最外层容器
 * 使用flexbox布局，垂直排列所有卡片
 */
export const CompanyProfileContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(3), // 卡片之间的间距
}))

/**
 * 卡片容器 - 统一的卡片外观
 * 提供白色背景、圆角、边框和阴影
 */
export const ProfileCard = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(3),
  borderRadius: `${theme.fairworkly.radius.lg}px`,
  border: `1px solid ${theme.palette.divider}`,
  transition: theme.transitions.create(['box-shadow', 'border-color'], {
    duration: theme.transitions.duration.short,
  }),

  // hover效果 - 提升用户体验
  '&:hover': {
    borderColor: theme.palette.primary.light,
    boxShadow: theme.fairworkly.shadow.md,
  },
}))

/**
 * 卡片头部 - 包含标题和编辑/保存/取消按钮
 */
export const CardHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(3),
  paddingBottom: theme.spacing(2),
  borderBottom: `1px solid ${theme.palette.divider}`,
}))

/**
 * 卡片内容区域 - 包含所有表单字段或显示内容
 */
export const CardContent = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2.5),
}))

/**
 * 表单行 - 在查看模式下显示label和value
 * 使用grid布局实现label和value的对齐
 */
export const FormRow = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '180px 1fr', // label宽度固定，value自适应
  gap: theme.spacing(2),
  alignItems: 'start',

  // 响应式：小屏幕改为垂直布局
  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: '1fr',
    gap: theme.spacing(1),
  },
}))

/**
 * 字段标签 - 灰色、加粗
 */
export const FieldLabel = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  color: theme.palette.text.secondary,
  lineHeight: 1.6,
  paddingTop: theme.spacing(1.5), // 对齐输入框的视觉中心
}))

/**
 * 字段值 - 查看模式下显示的数据
 */
export const FieldValue = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.primary,
  lineHeight: 1.6,
  paddingTop: theme.spacing(1.5),
  minHeight: theme.spacing(5), // 确保即使空值也有高度

  // 空值显示占位符
  '&:empty::before': {
    content: '"—"',
    color: theme.palette.text.disabled,
  },
}))

/**
 * Logo占位符容器 - 用于显示公司Logo
 */
export const LogoPlaceholder = styled(Box)(({ theme }) => ({
  width: 120,
  height: 120,
  borderRadius: `${theme.fairworkly.radius.md}px`,
  border: `2px dashed ${theme.palette.divider}`,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  backgroundColor: theme.palette.background.default,
  color: theme.palette.text.disabled,
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightMedium,
  transition: theme.transitions.create(['border-color', 'background-color'], {
    duration: theme.transitions.duration.short,
  }),

  // hover效果
  '&:hover': {
    borderColor: theme.palette.primary.main,
    backgroundColor: theme.palette.action.hover,
    cursor: 'pointer',
  },
}))

/**
 * 按钮组 - 编辑模式下的Save/Cancel按钮容器
 */
export const ActionButtons = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(1.5),
  justifyContent: 'flex-end',
  marginTop: theme.spacing(1),
}))

/**
 * Badge容器 - 用于显示Primary badge
 */
export const BadgeContainer = styled(Box)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  padding: theme.spacing(0.5, 1.5),
  borderRadius: `${theme.fairworkly.radius.pill}px`,
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  textTransform: 'uppercase',
  letterSpacing: '0.05em',
}))

/**
 * Awards列表项 - 显示单个Award的信息
 */
export const AwardItem = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'flex-start',
  padding: theme.spacing(2),
  borderRadius: `${theme.fairworkly.radius.md}px`,
  backgroundColor: theme.palette.background.default,
  border: `1px solid ${theme.palette.divider}`,
  transition: theme.transitions.create(['background-color', 'border-color'], {
    duration: theme.transitions.duration.short,
  }),

  '&:hover': {
    backgroundColor: theme.palette.action.hover,
    borderColor: theme.palette.primary.light,
  },
}))

/**
 * Award信息左侧 - Award类型和badge
 */
export const AwardInfo = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
}))

/**
 * Award元数据 - 员工数和添加日期
 */
export const AwardMeta = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),
  fontSize: theme.typography.body2.fontSize,
  color: theme.palette.text.secondary,
  textAlign: 'right',
}))

/**
 * 错误提示文本 - 表单验证错误
 */
export const ErrorText = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.caption.fontSize,
  color: theme.palette.error.main,
  marginTop: theme.spacing(0.5),
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(0.5),
}))
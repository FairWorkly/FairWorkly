import { styled } from '@mui/material/styles'
import { Box, Typography, Container } from '@mui/material'

/**
 * ✅ 规范 1: 命名导出 (Named Export)
 * ✅ 规范 2: 严禁使用 sx 属性
 * ✅ 规范 3: 必须使用 theme 驱动颜色和间距
 */

// 1. 容器样式：负责背景、高度等大面积布局
const PageWrapper = styled(Box)(({ theme }) => ({
  minHeight: '100vh',
  backgroundColor: theme.palette.background.default,
  // 使用 theme.spacing 而非硬编码 px
  padding: theme.spacing(4),
}))

// 2. 局部渐变组件：体现“部分地区渐变色”
const HeroBanner = styled(Box)(({ theme }) => ({
  // 引用 theme.ts 中定义的自定义渐变
  background: theme.palette.brand.gradient,
  borderRadius: theme.shape.borderRadius,
  padding: theme.spacing(6),
  color: theme.palette.common.white,
  boxShadow: theme.shadows[2],
}))

// 3. 内容区块
const ContentSection = styled(Container)(({ theme }) => ({
  marginTop: theme.spacing(4),
  padding: theme.spacing(3),
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.shape.borderRadius,
}))

export const StyleGuideSample = () => {
  return (
    <PageWrapper>
      <HeroBanner>
        <Typography variant="h4" fontWeight={700}>
          Project Style Guide
        </Typography>
        <Typography variant="body1">
          所有组员必须遵循此 Styled 模式开发。
        </Typography>
      </HeroBanner>

      <ContentSection>
        <Typography color="text.primary">
          主体内容区域，背景为 background.paper
        </Typography>
      </ContentSection>
    </PageWrapper>
  )
}

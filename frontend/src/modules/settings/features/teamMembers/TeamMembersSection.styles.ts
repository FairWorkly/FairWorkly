import { Box, Skeleton } from '@mui/material'
import { styled } from '@/styles/styled'

export const SectionWrapper = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(3),
}))

export const TableSkeleton = styled(Skeleton)(({ theme }) => ({
  height: theme.spacing(40),
}))

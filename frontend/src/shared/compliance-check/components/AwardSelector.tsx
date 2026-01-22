import React from 'react'
import { Box, Typography, styled, alpha } from '@mui/material'
import StorefrontOutlinedIcon from '@mui/icons-material/StorefrontOutlined'
import RestaurantOutlinedIcon from '@mui/icons-material/RestaurantOutlined'
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined'

export type AwardType = 'retail' | 'hospitality' | 'clerks'

interface AwardOption {
  type: AwardType
  label: string
  code: string
  icon: React.ReactNode
}

const awardOptions: AwardOption[] = [
  {
    type: 'retail',
    label: 'General Retail Industry Award',
    code: 'MA000004',
    icon: <StorefrontOutlinedIcon />,
  },
  {
    type: 'hospitality',
    label: 'Hospitality Industry Award',
    code: 'MA000009',
    icon: <RestaurantOutlinedIcon />,
  },
  {
    type: 'clerks',
    label: 'Clerks Private Sector Award',
    code: 'MA000002',
    icon: <WorkOutlineOutlinedIcon />,
  },
]

const awardPaletteKey: Record<AwardType, 'primary' | 'secondary' | 'info'> = {
  retail: 'primary',
  hospitality: 'secondary',
  clerks: 'info',
}

interface AwardCardProps {
  selected?: boolean
  paletteKey: 'primary' | 'secondary' | 'info'
}

const AwardCard = styled(Box, {
  shouldForwardProp: prop => prop !== 'selected' && prop !== 'paletteKey',
})<AwardCardProps>(({ theme, selected, paletteKey }) => {
  const palette = theme.palette[paletteKey]
  const hoverBg = alpha(palette.main, 0.06)
  const selectedBg = alpha(palette.main, 0.12)

  return {
    flex: 1,
    padding: theme.spacing(4, 3),
    borderRadius: theme.fairworkly.radius.lg,
    border: selected ? `2px solid ${palette.main}` : `1.5px solid ${theme.palette.divider}`,
    backgroundColor: selected ? selectedBg : theme.palette.background.paper,
    cursor: 'pointer',
    textAlign: 'center',
    transition: theme.transitions.create(['border-color', 'background-color'], {
      duration: theme.transitions.duration.short,
    }),
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    '&:hover': {
      borderColor: palette.main,
      backgroundColor: hoverBg,
    },
  }
})

interface AwardIconBoxProps {
  paletteKey: 'primary' | 'secondary' | 'info'
  selected?: boolean
}

const AwardIconBox = styled(Box, {
  shouldForwardProp: prop => prop !== 'paletteKey' && prop !== 'selected',
})<AwardIconBoxProps>(({ theme, paletteKey, selected }) => {
  const palette = theme.palette[paletteKey]
  const iconBg = alpha(palette.main, selected ? 0.18 : 0.1)

  return {
    width: theme.spacing(6),
    height: theme.spacing(6),
    borderRadius: theme.fairworkly.radius.md,
    backgroundColor: iconBg,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    color: palette.main,
    marginBottom: theme.spacing(2),
    '& .MuiSvgIcon-root': {
      fontSize: theme.spacing(3),
    },
  }
})

const SectionLabel = styled(Typography)(({ theme }) => ({
  ...theme.typography.subtitle2,
  fontWeight: theme.typography.fontWeightBold,
  marginBottom: theme.spacing(2.5),
  color: theme.palette.text.primary,
}))

const AwardGrid = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2.5),
  [theme.breakpoints.down('sm')]: {
    flexDirection: 'column',
  },
}))

const AwardLabel = styled(Typography)(({ theme }) => ({
  ...theme.typography.subtitle2,
  fontWeight: theme.typography.fontWeightBold,
  marginBottom: theme.spacing(0.5),
}))

const AwardDescription = styled(Typography)(({ theme }) => ({
  ...theme.typography.caption,
  color: theme.palette.text.secondary,
  fontWeight: theme.typography.fontWeightMedium,
}))

interface AwardSelectorProps {
  selectedAward: AwardType
  onAwardChange: (award: AwardType) => void
}

export const AwardSelector: React.FC<AwardSelectorProps> = ({
  selectedAward,
  onAwardChange,
}) => {
  return (
    <Box>
      <SectionLabel>Select Award</SectionLabel>
      <AwardGrid>
        {awardOptions.map(award => {
          const paletteKey = awardPaletteKey[award.type]

          return (
          <AwardCard
            key={award.type}
            paletteKey={paletteKey}
            selected={selectedAward === award.type}
            onClick={() => onAwardChange(award.type)}
          >
            <AwardIconBox paletteKey={paletteKey} selected={selectedAward === award.type}>
              {award.icon}
            </AwardIconBox>
            <AwardLabel>{award.label}</AwardLabel>
            <AwardDescription>{award.code}</AwardDescription>
          </AwardCard>
          )
        })}
      </AwardGrid>
    </Box>
  )
}

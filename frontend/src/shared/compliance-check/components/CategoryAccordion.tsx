// Shared building block: collapsible category panel.
// Extracted from IssuesByCategory — renders the accordion header
// (icon badge, title, employee count, optional amount label) and a
// collapsible body slot via children.
//
// Domain-agnostic: the consumer provides `icon` as ReactNode because
// roster uses string-key icon mapping while payroll uses CategoryType
// enum keys — two completely different resolution strategies.
// The `amountLabel` prop replaces the old `resultType` conditional:
// pass a formatted string to show it, omit to hide.

import React from 'react'
import { Box, Typography, Collapse, alpha, ButtonBase } from '@mui/material'
import { styled } from '@/styles/styled'
import ExpandMoreOutlinedIcon from '@mui/icons-material/ExpandMoreOutlined'
import ChevronRightOutlinedIcon from '@mui/icons-material/ChevronRightOutlined'

const CategoryPanel = styled(Box)(({ theme }) => ({
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.background.default}`,
  overflow: 'visible',
}))

const CategoryHeaderRow = styled(ButtonBase)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  width: '100%',
  textAlign: 'left',
  padding: theme.spacing(1.5, 2),
  cursor: 'pointer',
  borderRadius: theme.fairworkly.radius.md,
  transition: theme.transitions.create('background-color', {
    duration: theme.transitions.duration.short,
  }),
  [theme.breakpoints.down('sm')]: {
    alignItems: 'flex-start',
  },
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(2, 2.5),
  },
  '&:hover': {
    backgroundColor: theme.palette.background.default,
  },
}))

const ExpandToggleIcon = styled(Box)(({ theme }) => ({
  color: theme.palette.text.disabled,
  marginRight: theme.spacing(1.5),
}))

const CategoryIconBadge = styled(Box, {
  shouldForwardProp: prop => prop !== 'iconColor',
})<{ iconColor?: string }>(({ theme, iconColor }) => ({
  width: theme.spacing(4.5),
  height: theme.spacing(4.5),
  borderRadius: theme.fairworkly.radius.sm,
  backgroundColor: alpha(iconColor || theme.palette.error.main, 0.1),
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: iconColor || theme.palette.error.main,
  marginRight: theme.spacing(2),
  [theme.breakpoints.up('sm')]: {
    width: theme.spacing(5),
    height: theme.spacing(5),
  },
}))

const CategoryInfoContainer = styled(Box)(() => ({
  flex: 1,
  minWidth: 0,
}))

const CategoryInfoStack = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexWrap: 'wrap',
  flexDirection: 'column',
  alignItems: 'flex-start',
  gap: theme.spacing(0.5),
  [theme.breakpoints.up('md')]: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: theme.spacing(2),
  },
}))

const CategoryTitle = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.h2.fontWeight,
  color: theme.palette.text.primary,
}))

const CategoryMetaText = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.caption.fontWeight,
  color: theme.palette.text.secondary,
}))

const CategoryAmountText = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.subtitle1.fontWeight,
  color: theme.palette.error.main,
}))

/**
 * Fix MUI Collapse overflow:hidden scroll issue on md breakpoint.
 * MUI Collapse sets overflow:hidden during animation which blocks
 * trackpad/wheel scrolling. We override overflowY to visible on all
 * three layers (root, wrapper, wrapperInner) while keeping overflowX
 * hidden to prevent horizontal overflow.
 */
const StyledCollapse = styled(Collapse)(() => ({
  // Root level
  overflowY: 'visible',
  overflowX: 'hidden',
  // Wrapper level
  '& .MuiCollapse-wrapper': {
    overflowY: 'visible',
    overflowX: 'hidden',
  },
  // Inner wrapper level
  '& .MuiCollapse-wrapperInner': {
    overflowY: 'visible',
    overflowX: 'hidden',
  },
}))

const CategoryBody = styled(Box)(({ theme }) => ({
  borderTop: `1px solid ${theme.palette.background.default}`,
  backgroundColor: theme.palette.background.default,
}))

export interface CategoryAccordionProps {
  title: string
  icon: React.ReactNode
  iconColor: string
  employeeCount: number
  amountLabel?: string
  expanded: boolean
  onToggle: () => void
  children: React.ReactNode
}

export const CategoryAccordion: React.FC<CategoryAccordionProps> = ({
  title,
  icon,
  iconColor,
  employeeCount,
  amountLabel,
  expanded,
  onToggle,
  children,
}) => (
  <CategoryPanel>
    <CategoryHeaderRow onClick={onToggle} aria-expanded={expanded}>
      <ExpandToggleIcon>
        {expanded ? <ExpandMoreOutlinedIcon /> : <ChevronRightOutlinedIcon />}
      </ExpandToggleIcon>
      <CategoryIconBadge iconColor={iconColor}>{icon}</CategoryIconBadge>
      <CategoryInfoContainer>
        <CategoryInfoStack>
          <CategoryTitle variant="subtitle1" noWrap>
            {title}
          </CategoryTitle>
          <CategoryMetaText variant="body2" noWrap>
            {employeeCount} {employeeCount === 1 ? 'employee' : 'employees'}{' '}
            flagged
          </CategoryMetaText>
          {amountLabel && (
            <CategoryAmountText variant="body2" noWrap>
              {amountLabel}
            </CategoryAmountText>
          )}
        </CategoryInfoStack>
      </CategoryInfoContainer>
    </CategoryHeaderRow>
    <StyledCollapse in={expanded}>
      <CategoryBody>{children}</CategoryBody>
    </StyledCollapse>
  </CategoryPanel>
)

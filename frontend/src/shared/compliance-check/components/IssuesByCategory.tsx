import React, { useState } from 'react'
import {
  Box,
  Typography,
  Button,
  Paper,
  Stack,
  Collapse,
  styled,
  alpha,
} from '@mui/material'
import FactCheckOutlinedIcon from '@mui/icons-material/FactCheckOutlined'
import ExpandMoreOutlinedIcon from '@mui/icons-material/ExpandMoreOutlined'
import ChevronRightOutlinedIcon from '@mui/icons-material/ChevronRightOutlined'
import AttachMoneyOutlinedIcon from '@mui/icons-material/AttachMoneyOutlined'
import GavelOutlinedIcon from '@mui/icons-material/GavelOutlined'
import ScheduleOutlinedIcon from '@mui/icons-material/ScheduleOutlined'
import CardGiftcardOutlinedIcon from '@mui/icons-material/CardGiftcardOutlined'
import BeachAccessOutlinedIcon from '@mui/icons-material/BeachAccessOutlined'
import type { IssueCategory, IssueItem } from '../types/complianceCheck.type'
import { IssueRow, type GuidanceContent } from './IssueRow'

const iconMap: Record<string, React.ElementType> = {
  attach_money: AttachMoneyOutlinedIcon,
  gavel: GavelOutlinedIcon,
  schedule: ScheduleOutlinedIcon,
  card_giftcard: CardGiftcardOutlinedIcon,
  beach_access: BeachAccessOutlinedIcon,
}

const IssuesWrapper = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(2),
  borderRadius: theme.fairworkly.radius.xl,
  border: `1px solid ${theme.palette.divider}`,
  boxShadow: 'none',
  backgroundColor: theme.palette.background.paper,
  marginTop: theme.spacing(2),
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(3),
    marginTop: theme.spacing(3),
  },
  [theme.breakpoints.up('md')]: {
    padding: theme.spacing(1),
  },
}))

const IssuesHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(3),
  [theme.breakpoints.down('sm')]: {
    flexDirection: 'column',
    alignItems: 'flex-start',
    gap: theme.spacing(2),
  },
  [theme.breakpoints.up('md')]: {
    marginBottom: theme.spacing(2),
  },
}))

const IssuesHeaderTitle = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
}))

const IssuesHeaderActions = styled(Stack)(({ theme }) => ({
  alignItems: 'center',
  [theme.breakpoints.down('sm')]: {
    width: '100%',
    flexDirection: 'column',
    alignItems: 'stretch',
    gap: theme.spacing(1),
  },
  [theme.breakpoints.up('md')]: {
    marginRight: theme.spacing(1.5),
  },
}))

const IssuesHeaderIcon = styled(FactCheckOutlinedIcon)(({ theme }) => ({
  color: theme.palette.text.primary,
}))

const CategoryPanel = styled(Box)(({ theme }) => ({
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.background.default}`,
  overflow: 'visible',
}))

const CategoryHeaderRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
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

const CategoryInfoStack = styled(Stack)(({ theme }) => ({
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

const EmptyState = styled(Box)(({ theme }) => ({
  padding: theme.spacing(4),
  textAlign: 'center',
}))

const EmptyStateText = styled(Typography)({
  fontStyle: 'italic',
})

const ShowMoreRow = styled(Box)(({ theme }) => ({
  padding: theme.spacing(1.25),
  textAlign: 'center',
  borderTop: `1px solid ${theme.palette.background.default}`,
  backgroundColor: theme.palette.background.paper,
  [theme.breakpoints.up('sm')]: {
    padding: theme.spacing(1.5),
  },
}))

const ShowMoreAction = styled(Button)(({ theme }) => ({
  color: theme.palette.primary.main,
  fontWeight: theme.typography.button.fontWeight,
  fontSize: theme.typography.body2.fontSize,
  [theme.breakpoints.down('sm')]: {
    width: '100%',
  },
}))

const SelectAllAction = styled(Button, {
  shouldForwardProp: prop => prop !== 'isSelected',
})<{ isSelected?: boolean }>(({ theme, isSelected }) => ({
  borderColor: isSelected ? theme.palette.primary.main : theme.palette.divider,
  color: isSelected ? theme.palette.primary.main : theme.palette.text.primary,
  backgroundColor: isSelected
    ? alpha(theme.palette.primary.main, 0.05)
    : theme.palette.background.paper,
  borderRadius: theme.fairworkly.radius.sm,
  paddingLeft: theme.spacing(2),
  paddingRight: theme.spacing(2),
  height: theme.spacing(5),
  fontWeight: theme.typography.button.fontWeight,
  [theme.breakpoints.down('sm')]: {
    width: '100%',
    fontSize: theme.typography.caption.fontSize,
    paddingLeft: theme.spacing(1.5),
    paddingRight: theme.spacing(1.5),
    justifyContent: 'flex-start',
    textAlign: 'left',
  },
}))

const CategoriesStack = styled(Stack)(({ theme }) => ({
  gap: theme.spacing(2),
}))

interface IssuesByCategoryProps {
  categories: IssueCategory[]
  onExport?: () => void
  guidanceForIssue?: (issue: IssueItem) => GuidanceContent | undefined
  resultType?: 'payroll' | 'roster'
}

export const IssuesByCategory: React.FC<IssuesByCategoryProps> = ({
  categories,
  guidanceForIssue,
  resultType = 'payroll',
}) => {
  const [expandedCategories, setExpandedCategories] = useState<
    Record<string, boolean>
  >({
    [categories[0]?.id]: true,
  })
  const [selectedIssueIds, setSelectedIssueIds] = useState<number[]>([])

  const allIssues = categories.flatMap(cat => cat.issues)
  const allIssueIds = allIssues.map(issue => issue.id)
  const isAllSelected =
    allIssueIds.length > 0 && selectedIssueIds.length === allIssueIds.length

  const toggleCategory = (id: string) => {
    setExpandedCategories(prev => ({ ...prev, [id]: !prev[id] }))
  }

  const toggleIssueSelection = (id: number) => {
    setSelectedIssueIds(prev =>
      prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id]
    )
  }

  const handleSelectAll = () => {
    if (isAllSelected) {
      setSelectedIssueIds([])
    } else {
      setSelectedIssueIds(allIssueIds)
    }
  }

  return (
    <IssuesWrapper>
      <IssuesHeader>
        <IssuesHeaderTitle>
          <IssuesHeaderIcon />
          <Typography variant="h6">Issues by Category</Typography>
        </IssuesHeaderTitle>
        <IssuesHeaderActions direction="row" spacing={1.5}>
          <SelectAllAction
            variant="outlined"
            size="small"
            onClick={handleSelectAll}
            isSelected={isAllSelected}
          >
            {isAllSelected ? 'Deselect All' : 'Select All Issues'}
          </SelectAllAction>
        </IssuesHeaderActions>
      </IssuesHeader>

      <CategoriesStack>
        {categories
          .filter(cat => cat.employeeCount > 0)
          .map(category => (
            <CategoryPanel key={category.id}>
              <CategoryHeaderRow onClick={() => toggleCategory(category.id)}>
                <ExpandToggleIcon>
                  {expandedCategories[category.id] ? (
                    <ExpandMoreOutlinedIcon />
                  ) : (
                    <ChevronRightOutlinedIcon />
                  )}
                </ExpandToggleIcon>
                <CategoryIconBadge iconColor={category.color}>
                  {(() => {
                    const IconComponent = iconMap[category.icon]
                    return IconComponent ? (
                      <IconComponent fontSize="small" />
                    ) : (
                      <FactCheckOutlinedIcon fontSize="small" />
                    )
                  })()}
                </CategoryIconBadge>
                <CategoryInfoContainer>
                  <CategoryInfoStack>
                    <CategoryTitle variant="subtitle1" noWrap>
                      {category.title}
                    </CategoryTitle>
                    <CategoryMetaText variant="body2" noWrap>
                      {category.employeeCount} employees flagged
                    </CategoryMetaText>
                    {resultType !== 'roster' && (
                      <CategoryAmountText variant="body2" noWrap>
                        {category.totalUnderpayment} underpayment
                      </CategoryAmountText>
                    )}
                  </CategoryInfoStack>
                </CategoryInfoContainer>
              </CategoryHeaderRow>

              <StyledCollapse in={expandedCategories[category.id]}>
                <CategoryBody>
                  {category.issues.length > 0 ? (
                    category.issues.map(issue => (
                      <IssueRow
                        key={issue.id}
                        issue={issue}
                        isSelected={selectedIssueIds.includes(issue.id)}
                        onToggleSelection={() => toggleIssueSelection(issue.id)}
                        guidance={guidanceForIssue?.(issue)}
                      />
                    ))
                  ) : (
                    <EmptyState>
                      <EmptyStateText variant="body2" color="text.secondary">
                        Generating detailed record breakdown for{' '}
                        {category.title}...
                      </EmptyStateText>
                    </EmptyState>
                  )}

                  {category.issues.length > 0 &&
                    category.employeeCount > category.issues.length && (
                      <ShowMoreRow>
                        <ShowMoreAction
                          variant="text"
                          startIcon={<ExpandMoreOutlinedIcon />}
                        >
                          Show {category.employeeCount - category.issues.length}{' '}
                          more results
                        </ShowMoreAction>
                      </ShowMoreRow>
                    )}
                </CategoryBody>
              </StyledCollapse>
            </CategoryPanel>
          ))}
      </CategoriesStack>
    </IssuesWrapper>
  )
}

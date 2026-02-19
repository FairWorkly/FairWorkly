// Roster-oriented category list: wraps CategoryAccordion with
// roster-specific icon mapping, issue selection checkboxes, and
// IssueRow rendering. Payroll will build its own category section
// using CategoryAccordion directly (Issue #6) with different issue
// row components and icon resolution.

import React, { useState } from 'react'
import {
  Box,
  Typography,
  Button,
  Paper,
  Stack,
  styled,
  alpha,
} from '@mui/material'
import FactCheckOutlinedIcon from '@mui/icons-material/FactCheckOutlined'
import AttachMoneyOutlinedIcon from '@mui/icons-material/AttachMoneyOutlined'
import GavelOutlinedIcon from '@mui/icons-material/GavelOutlined'
import ScheduleOutlinedIcon from '@mui/icons-material/ScheduleOutlined'
import CardGiftcardOutlinedIcon from '@mui/icons-material/CardGiftcardOutlined'
import BeachAccessOutlinedIcon from '@mui/icons-material/BeachAccessOutlined'
import type { IssueCategory, IssueItem } from '../types/complianceCheck.type'
import { formatMoney } from '../utils/formatters'
import { IssueRow, type GuidanceContent } from './IssueRow'
import { CategoryAccordion } from './CategoryAccordion'

// Roster icon mapping: backend mock data uses Material icon string keys.
// Payroll categories use CategoryType enum ('BaseRate', etc.) and will
// have their own mapping — this iconMap is roster-only.
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
  resultType = 'roster',
}) => {
  const [expandedCategories, setExpandedCategories] = useState<
    Record<string, boolean>
  >(() => {
    const firstId = categories[0]?.id
    return firstId ? { [firstId]: true } : {}
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
          .map(category => {
            const IconComponent = iconMap[category.icon]
            return (
              <CategoryAccordion
                key={category.id}
                title={category.title}
                icon={
                  IconComponent ? (
                    <IconComponent fontSize="small" />
                  ) : (
                    <FactCheckOutlinedIcon fontSize="small" />
                  )
                }
                iconColor={category.color}
                employeeCount={category.employeeCount}
                // Roster hides amount label; payroll shows "$X underpayment"
                amountLabel={
                  resultType !== 'roster'
                    ? `${formatMoney(category.totalUnderpayment)} underpayment`
                    : undefined
                }
                expanded={!!expandedCategories[category.id]}
                onToggle={() => toggleCategory(category.id)}
              >
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
                      Generating detailed record breakdown for {category.title}
                      ...
                    </EmptyStateText>
                  </EmptyState>
                )}

                {category.issues.length > 0 &&
                  category.employeeCount > category.issues.length && (
                    <ShowMoreRow>
                      <Typography variant="body2" color="text.secondary">
                        {category.employeeCount - category.issues.length} more
                        employees not shown
                      </Typography>
                    </ShowMoreRow>
                  )}
              </CategoryAccordion>
            )
          })}
      </CategoriesStack>
    </IssuesWrapper>
  )
}

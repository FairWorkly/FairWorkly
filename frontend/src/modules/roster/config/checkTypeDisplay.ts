export interface CategoryDisplayMeta {
  id: string
  title: string
  icon: string
  color: string
}

/**
 * Maps backend RosterCheckType enum values to display metadata
 * for the ComplianceResults categories UI.
 */
export const checkTypeDisplayMap: Record<string, CategoryDisplayMeta> = {
  MinimumShiftHours: {
    id: 'minimum-hours',
    title: 'Minimum Shift Hours',
    icon: 'schedule',
    color: '#ef4444',
  },
  MealBreak: {
    id: 'meal-breaks',
    title: 'Meal Break Requirements',
    icon: 'restaurant',
    color: '#f97316',
  },
  RestPeriodBetweenShifts: {
    id: 'rest-periods',
    title: 'Rest Period Between Shifts',
    icon: 'bedtime',
    color: '#eab308',
  },
  WeeklyHoursLimit: {
    id: 'weekly-hours',
    title: 'Weekly Hours Limit',
    icon: 'timer',
    color: '#3b82f6',
  },
  MaximumConsecutiveDays: {
    id: 'consecutive-days',
    title: 'Maximum Consecutive Days',
    icon: 'date_range',
    color: '#8b5cf6',
  },
}

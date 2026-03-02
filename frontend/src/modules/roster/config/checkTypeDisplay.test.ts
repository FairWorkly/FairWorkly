import { describe, it, expect } from 'vitest'
import { checkTypeDisplayMap } from './checkTypeDisplay'

describe('checkTypeDisplayMap', () => {
  const expectedCheckTypes = [
    'MinimumShiftHours',
    'MealBreak',
    'RestPeriodBetweenShifts',
    'WeeklyHoursLimit',
    'MaximumConsecutiveDays',
  ]

  it('contains all 5 compliance check type entries', () => {
    const keys = Object.keys(checkTypeDisplayMap)
    expect(keys).toHaveLength(5)
    for (const checkType of expectedCheckTypes) {
      expect(checkTypeDisplayMap).toHaveProperty(checkType)
    }
  })

  it.each(expectedCheckTypes)(
    '%s has all required display properties',
    checkType => {
      const display = checkTypeDisplayMap[checkType]
      expect(display).toBeDefined()
      expect(display.id).toBeTruthy()
      expect(display.title).toBeTruthy()
      expect(display.icon).toBeTruthy()
      expect(display.color).toMatch(/^#[0-9a-f]{6}$/i)
    }
  )

  it('has unique ids for each check type', () => {
    const ids = Object.values(checkTypeDisplayMap).map(d => d.id)
    const uniqueIds = new Set(ids)
    expect(uniqueIds.size).toBe(ids.length)
  })

  it('has unique icons for each check type', () => {
    const icons = Object.values(checkTypeDisplayMap).map(d => d.icon)
    const uniqueIcons = new Set(icons)
    expect(uniqueIcons.size).toBe(icons.length)
  })

  it('maps specific check types to expected display values', () => {
    expect(checkTypeDisplayMap.MinimumShiftHours).toEqual({
      id: 'minimum-hours',
      title: 'Minimum Shift Hours',
      icon: 'schedule',
      color: '#ef4444',
    })

    expect(checkTypeDisplayMap.MealBreak).toEqual({
      id: 'meal-breaks',
      title: 'Meal Break Requirements',
      icon: 'restaurant',
      color: '#f97316',
    })

    expect(checkTypeDisplayMap.MaximumConsecutiveDays).toEqual({
      id: 'consecutive-days',
      title: 'Maximum Consecutive Days',
      icon: 'date_range',
      color: '#8b5cf6',
    })
  })
})

import type { ShiftScenario } from '../types/debate.types'

const DAYS_OF_WEEK = 'Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday'

export const SAMPLE_DEBATE_SCENARIO: ShiftScenario = {
  employee_name: 'Alice',
  shift_date: '2024-03-16 (Saturday)',
  shift_hours: 10,
  week_hours_before_shift: 38,
  award_name: 'Hospitality Industry (General) Award 2020',
  extra_context: 'Full-time employee',
}

interface DebateCommandParseSuccess {
  ok: true
  scenario: ShiftScenario
  mode: 'example' | 'parsed'
}

interface DebateCommandParseFailure {
  ok: false
  message: string
}

export type DebateCommandParseResult =
  | DebateCommandParseSuccess
  | DebateCommandParseFailure

const EXAMPLE_COMMANDS = new Set(['', '--example', 'example', 'sample'])

const normalizeWhitespace = (value: string): string =>
  value.replace(/\s+/g, ' ').trim()

const parseNumber = (value?: string): number | null => {
  if (!value) {
    return null
  }
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : null
}

const isValidShiftHours = (value: number | null): value is number =>
  value !== null && value > 0

const isValidWeekHoursBeforeShift = (value: number | null): value is number =>
  value !== null && value >= 0

const capitalizeFirst = (value: string): string =>
  value ? `${value.charAt(0).toUpperCase()}${value.slice(1)}` : value

const coerceScenario = (value: unknown): ShiftScenario | null => {
  if (!value || typeof value !== 'object') {
    return null
  }

  const candidate = value as Partial<ShiftScenario>
  const employeeName =
    typeof candidate.employee_name === 'string'
      ? normalizeWhitespace(candidate.employee_name)
      : ''
  const shiftDate =
    typeof candidate.shift_date === 'string'
      ? normalizeWhitespace(candidate.shift_date)
      : ''
  const awardName =
    typeof candidate.award_name === 'string'
      ? normalizeWhitespace(candidate.award_name)
      : ''
  const shiftHours =
    typeof candidate.shift_hours === 'number'
      ? candidate.shift_hours
      : parseNumber(String(candidate.shift_hours ?? ''))
  const weekHoursBeforeShift =
    typeof candidate.week_hours_before_shift === 'number'
      ? candidate.week_hours_before_shift
      : parseNumber(String(candidate.week_hours_before_shift ?? ''))
  const extraContext =
    typeof candidate.extra_context === 'string'
      ? normalizeWhitespace(candidate.extra_context)
      : undefined

  if (
    !employeeName ||
    !shiftDate ||
    !awardName ||
    !isValidShiftHours(shiftHours) ||
    !isValidWeekHoursBeforeShift(weekHoursBeforeShift)
  ) {
    return null
  }

  return {
    employee_name: employeeName,
    shift_date: shiftDate,
    shift_hours: shiftHours,
    week_hours_before_shift: weekHoursBeforeShift,
    award_name: awardName,
    extra_context: extraContext || undefined,
  }
}

const parseJsonScenario = (text: string): ShiftScenario | null => {
  if (!text.startsWith('{')) {
    return null
  }

  try {
    const parsed = JSON.parse(text)
    return coerceScenario(parsed)
  } catch {
    return null
  }
}

const parseNaturalLanguageScenario = (text: string): ShiftScenario | null => {
  const employeeNameMatch = text.match(
    /^([A-Z][A-Za-z'-]*(?:\s+[A-Z][A-Za-z'-]*)*)\s+(?:worked|works|is working)\b/
  )
  const shiftHoursMatch = text.match(/\bworked\s+(\d+(?:\.\d+)?)\s+hours?\b/i)
  const weekHoursMatch = text.match(
    /\b(?:already\s+worked|worked)\s+(\d+(?:\.\d+)?)\s+hours?\s+(?:this week|so far this week|before this shift)\b/i
  )
  const isoDateMatch = text.match(/\b\d{4}-\d{2}-\d{2}(?:\s*\([^)]+\))?/)
  const dayOfWeekMatch = text.match(
    new RegExp(`\\bon\\s+(${DAYS_OF_WEEK})\\b`, 'i')
  )
  const awardMatch = text.match(
    /(?:under|covered by|under the)\s+(?:the\s+)?(.+?Award(?:\s+\d{4})?)(?=[.?!]|,|$)/i
  )
  const employmentMatch = text.match(
    /\b(full-time|part-time|casual)\s+employee\b/i
  )

  const employeeName = employeeNameMatch
    ? normalizeWhitespace(employeeNameMatch[1])
    : ''
  const shiftHours = parseNumber(shiftHoursMatch?.[1])
  const weekHoursBeforeShift = parseNumber(weekHoursMatch?.[1])
  const shiftDate = normalizeWhitespace(
    isoDateMatch?.[0] ?? dayOfWeekMatch?.[1] ?? ''
  )
  const awardName = normalizeWhitespace(awardMatch?.[1] ?? '')
  const extraContext = employmentMatch
    ? `${capitalizeFirst(employmentMatch[1].toLowerCase())} employee`
    : undefined

  if (
    !employeeName ||
    !shiftDate ||
    !awardName ||
    !isValidShiftHours(shiftHours) ||
    !isValidWeekHoursBeforeShift(weekHoursBeforeShift)
  ) {
    return null
  }

  return {
    employee_name: employeeName,
    shift_date: shiftDate,
    shift_hours: shiftHours,
    week_hours_before_shift: weekHoursBeforeShift,
    award_name: awardName,
    extra_context: extraContext,
  }
}

export const parseDebateCommand = (
  message: string
): DebateCommandParseResult => {
  const commandMatch = message.trim().match(/^\/debate\b([\s\S]*)$/i)
  if (!commandMatch) {
    return {
      ok: false,
      message: 'Debate commands must start with /debate.',
    }
  }

  const rawScenario = commandMatch[1].trim()
  if (EXAMPLE_COMMANDS.has(rawScenario.toLowerCase())) {
    return {
      ok: true,
      scenario: SAMPLE_DEBATE_SCENARIO,
      mode: 'example',
    }
  }

  const jsonScenario = parseJsonScenario(rawScenario)
  if (jsonScenario) {
    return {
      ok: true,
      scenario: jsonScenario,
      mode: 'parsed',
    }
  }

  const naturalLanguageScenario = parseNaturalLanguageScenario(rawScenario)
  if (naturalLanguageScenario) {
    return {
      ok: true,
      scenario: naturalLanguageScenario,
      mode: 'parsed',
    }
  }

  return {
    ok: false,
    message:
      'Could not parse that debate scenario. Use /debate followed by the employee name, shift hours, shift date or day, prior week hours, and Award, or run /debate --example.',
  }
}

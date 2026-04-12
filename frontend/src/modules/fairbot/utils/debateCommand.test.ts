import { describe, expect, it } from 'vitest'
import { parseDebateCommand, SAMPLE_DEBATE_SCENARIO } from './debateCommand'

describe('parseDebateCommand', () => {
  it('returns the sample scenario when the example command is used', () => {
    expect(parseDebateCommand('/debate --example')).toEqual({
      ok: true,
      mode: 'example',
      scenario: SAMPLE_DEBATE_SCENARIO,
    })
  })

  it('parses the existing natural language debate prompt into a scenario', () => {
    expect(
      parseDebateCommand(
        '/debate Alice worked 10 hours on 2024-03-16 (Saturday). She already worked 38 hours this week. Full-time employee under the Hospitality Industry (General) Award 2020. What is the correct pay rate?'
      )
    ).toEqual({
      ok: true,
      mode: 'parsed',
      scenario: SAMPLE_DEBATE_SCENARIO,
    })
  })

  it('accepts explicit JSON scenarios', () => {
    expect(
      parseDebateCommand(
        '/debate {"employee_name":"Bob","shift_date":"Sunday","shift_hours":8,"week_hours_before_shift":32,"award_name":"Clerks Award 2020","extra_context":"Casual employee"}'
      )
    ).toEqual({
      ok: true,
      mode: 'parsed',
      scenario: {
        employee_name: 'Bob',
        shift_date: 'Sunday',
        shift_hours: 8,
        week_hours_before_shift: 32,
        award_name: 'Clerks Award 2020',
        extra_context: 'Casual employee',
      },
    })
  })

  it('returns a clear error when required scenario fields are missing', () => {
    expect(parseDebateCommand('/debate Alice worked a long shift')).toEqual({
      ok: false,
      message:
        'Could not parse that debate scenario. Use /debate followed by the employee name, shift hours, shift date or day, prior week hours, and Award, or run /debate --example.',
    })
  })

  it('rejects scenarios with non-positive shift hours', () => {
    expect(
      parseDebateCommand(
        '/debate {"employee_name":"Bob","shift_date":"Sunday","shift_hours":0,"week_hours_before_shift":32,"award_name":"Clerks Award 2020"}'
      )
    ).toEqual({
      ok: false,
      message:
        'Could not parse that debate scenario. Use /debate followed by the employee name, shift hours, shift date or day, prior week hours, and Award, or run /debate --example.',
    })
  })

  it('rejects scenarios with negative prior week hours', () => {
    expect(
      parseDebateCommand(
        '/debate {"employee_name":"Bob","shift_date":"Sunday","shift_hours":8,"week_hours_before_shift":-1,"award_name":"Clerks Award 2020"}'
      )
    ).toEqual({
      ok: false,
      message:
        'Could not parse that debate scenario. Use /debate followed by the employee name, shift hours, shift date or day, prior week hours, and Award, or run /debate --example.',
    })
  })
})

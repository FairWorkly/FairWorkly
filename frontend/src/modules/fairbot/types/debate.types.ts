export interface ShiftScenario {
  employee_name: string
  shift_date: string
  shift_hours: number
  week_hours_before_shift: number
  award_name: string
  extra_context?: string
}

export interface DebateSource {
  source: string
  page: number
  content?: string
}

export interface DebateRound {
  agent: string
  role: string
  icon: string
  stance: string
  reasoning: string
  challenges: string | null
  agrees_with: string | null
  sources: DebateSource[]
}

export interface DebateResult {
  scenario_summary: string
  rounds: DebateRound[]
  final_ruling: string
  cited_award_section: string | null
  model: string | null
}

export interface DebateApiResponse {
  code: number
  msg: string
  data: DebateResult
}

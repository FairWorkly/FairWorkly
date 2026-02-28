// Chat participant roles used to render message styling and labels.
export type FairBotMessageRole = 'user' | 'assistant'

export interface FairBotMessageSource {
  source: string
  page?: number
  content?: string
}

export interface FairBotActionShift {
  employee: string
  dates: string
  description: string
}

export interface FairBotActionPlanItem {
  id: string
  priority: string
  title: string
  owner: string
  checkType: string
  issueCount: number
  criticalCount: number
  affectedShifts: FairBotActionShift[]
  whatToChange: string
  why: string
  expectedOutcome: string
  riskIfIgnored: string
  focusExamples: string
}

export interface FairBotActionFollowUp {
  id: string
  label: string
  prompt: string
  actionId: string
}

export interface FairBotActionPlan {
  title: string
  validationId: string | null
  actions: FairBotActionPlanItem[]
  quickFollowUps: FairBotActionFollowUp[]
}

export interface FairBotMessageMetadata {
  model?: string
  note?: string | null
  sources?: FairBotMessageSource[]
  actionPlan?: FairBotActionPlan
}

// Chat message model used across UI and session storage.
export interface FairBotMessage {
  id: string
  role: FairBotMessageRole
  text: string
  timestamp: string
  metadata?: FairBotMessageMetadata
}

// Standard error shape used by FairBot hooks.
export interface FairBotError {
  message: string
  code?: string
  status?: number
  requestId?: string
}

export interface FairBotTraceInfo {
  requestId: string | null
  status: 'success' | 'error'
  note?: string | null
  timestamp: string
}

// Conversation state owned by the main FairBot hook.
export interface FairBotConversationState {
  messages: FairBotMessage[]
  isLoading: boolean
  error: FairBotError | null
}

export type FairBotMessageRole = 'user' | 'assistant'

export interface FairBotFileMeta {
  name: string
  size: number
  type: string
}

export interface FairBotMessage {
  id: string
  role: FairBotMessageRole
  text: string
  timestamp: string
  file?: File
  fileMeta?: FairBotFileMeta
}

export interface FairBotSendMessagePayload {
  text: string
  file?: File
}

export interface FairBotError {
  message: string
  code?: string
}

export interface FairBotConversationState {
  messages: FairBotMessage[]
  isLoading: boolean
  error: FairBotError | null
}

export interface FairBotUploadState {
  isDragging: boolean
  isUploading: boolean
  error: FairBotError | null
  acceptedFileTypes: string[]
}

export interface PayrollIssue {
  id: string
  description: string
  severity?: string
}

export interface PayrollSummaryData {
  issuesFound: number
  topIssues: PayrollIssue[]
  totalRecords?: number
}

export interface RosterIssue {
  id: string
  description: string
  severity?: string
}

export interface RosterSummaryData {
  issuesFound: number
  topIssues: RosterIssue[]
  shiftCount?: number
}

export interface EmployeeSummaryData {
  employeesReviewed: number
  issuesFound: number
}

export interface DocumentSummaryData {
  documentsGenerated: number
  lastGeneratedAt?: string
}

export type FairBotResultType = 'payroll' | 'roster' | 'employee' | 'document'

export interface FairBotPayrollResult {
  type: 'payroll'
  data: PayrollSummaryData
  detailsUrl: string
}

export interface FairBotRosterResult {
  type: 'roster'
  data: RosterSummaryData
  detailsUrl: string
}

export interface FairBotEmployeeResult {
  type: 'employee'
  data: EmployeeSummaryData
  detailsUrl: string
}

export interface FairBotDocumentResult {
  type: 'document'
  data: DocumentSummaryData
  detailsUrl: string
}

export type FairBotResult =
  | FairBotPayrollResult
  | FairBotRosterResult
  | FairBotEmployeeResult
  | FairBotDocumentResult

export interface FairBotAgentResponse {
  textResponse: string
  quickSummary?: FairBotResult
}

export type FairBotPermission = string

export interface FairBotQuickAction {
  id: string
  title: string
  description: string
  icon: string
  color: string
  requiresFile: boolean
  acceptedFileTypes?: string[]
  initialMessage: string
  requiredPermission: FairBotPermission | null
}

export interface FairBotResultsPanelState {
  currentResult: FairBotResult | null
}

export interface FairBotSessionState {
  messages: FairBotMessage[]
  currentResult: FairBotResult | null
}

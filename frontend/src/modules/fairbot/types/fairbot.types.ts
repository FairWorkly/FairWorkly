// Chat participant roles used to render message styling and labels.
export type FairBotMessageRole = 'user' | 'assistant'

// Serializable metadata for uploaded files (stored alongside messages).
export interface FairBotFileMeta {
  name: string
  size: number
  type: string
}

// Chat message model used across UI and session storage.
export interface FairBotMessage {
  id: string
  role: FairBotMessageRole
  text: string
  timestamp: string
  // Raw file object (not persisted to session storage).
  file?: File
  // Persisted metadata for file display in the chat.
  fileMeta?: FairBotFileMeta
}

// Payload for sending a message with an optional file.
export interface FairBotSendMessagePayload {
  text: string
  file?: File
}

// Standard error shape used by FairBot hooks.
export interface FairBotError {
  message: string
  code?: string
}

// Conversation state owned by the main FairBot hook.
export interface FairBotConversationState {
  messages: FairBotMessage[]
  isLoading: boolean
  error: FairBotError | null
}

// Upload state used by the file drop zone and controls.
export interface FairBotUploadState {
  isDragging: boolean
  isUploading: boolean
  error: FairBotError | null
  acceptedFileTypes: string[]
}

// Payroll issue row surfaced in summaries.
export interface PayrollIssue {
  id: string
  description: string
  severity?: string
}

// Summary data for payroll results.
export interface PayrollSummaryData {
  issuesFound: number
  topIssues: PayrollIssue[]
  totalRecords?: number
}

// Roster issue row surfaced in summaries.
export interface RosterIssue {
  id: string
  description: string
  severity?: string
}

// Summary data for roster results.
export interface RosterSummaryData {
  issuesFound: number
  topIssues: RosterIssue[]
  shiftCount?: number
}

// Summary data for employee review results.
export interface EmployeeSummaryData {
  employeesReviewed: number
  issuesFound: number
}

// Summary data for document generation results.
export interface DocumentSummaryData {
  documentsGenerated: number
  lastGeneratedAt?: string
}

// Discriminator for the union of result types.
export type FairBotResultType = 'payroll' | 'roster' | 'employee' | 'document'

// Result type for payroll checks.
export interface FairBotPayrollResult {
  type: 'payroll'
  data: PayrollSummaryData
  // Route to the detailed report page.
  detailsUrl: string
}

// Result type for roster checks.
export interface FairBotRosterResult {
  type: 'roster'
  data: RosterSummaryData
  // Route to the detailed report page.
  detailsUrl: string
}

// Result type for employee reviews.
export interface FairBotEmployeeResult {
  type: 'employee'
  data: EmployeeSummaryData
  // Route to the detailed report page.
  detailsUrl: string
}

// Result type for document generation.
export interface FairBotDocumentResult {
  type: 'document'
  data: DocumentSummaryData
  // Route to the detailed report page.
  detailsUrl: string
}

// Union of all supported summary result shapes.
export type FairBotResult =
  | FairBotPayrollResult
  | FairBotRosterResult
  | FairBotEmployeeResult
  | FairBotDocumentResult

// Response payload expected from the FairBot agent service.
export interface FairBotAgentResponse {
  textResponse: string
  // Optional summary shown in the results panel.
  quickSummary?: FairBotResult
}

// Permission token string used to gate quick actions.
export type FairBotPermission = string

// Configuration for a single quick action card.
export interface FairBotQuickAction {
  id: string
  title: string
  description: string
  icon: string
  color: string
  requiresFile: boolean
  // Optional override for accepted file types on this action.
  acceptedFileTypes?: string[]
  initialMessage: string
  // Permission required to display the action (null = always visible).
  requiredPermission: FairBotPermission | null
}

// State for the results panel summary card.
export interface FairBotResultsPanelState {
  currentResult: FairBotResult | null
}

// Persisted session snapshot for conversation + results.
export interface FairBotSessionState {
  messages: FairBotMessage[]
  currentResult: FairBotResult | null
}

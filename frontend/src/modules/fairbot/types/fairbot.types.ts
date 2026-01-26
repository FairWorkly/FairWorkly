// FairBot-specific types that extend the shared chat types.
// For base chat types, see @/shared/chat

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
  detailsUrl: string
}

// Result type for roster checks.
export interface FairBotRosterResult {
  type: 'roster'
  data: RosterSummaryData
  detailsUrl: string
}

// Result type for employee reviews.
export interface FairBotEmployeeResult {
  type: 'employee'
  data: EmployeeSummaryData
  detailsUrl: string
}

// Result type for document generation.
export interface FairBotDocumentResult {
  type: 'document'
  data: DocumentSummaryData
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
  acceptedFileTypes?: string[]
  initialMessage: string
  requiredPermission: FairBotPermission | null
}

// State for the results panel summary card.
export interface FairBotResultsPanelState {
  currentResult: FairBotResult | null
}

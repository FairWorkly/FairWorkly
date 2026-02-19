import httpClient from "./httpClient";
import { normalizeApiError } from "@/shared/types/api.types";

/**
 * Response from roster upload endpoint.
 * Backend returns roster ID and summary after successful parsing and storage.
 */
export interface UploadRosterResponse {
  rosterId: string;
  weekStartDate: string; // ISO date string
  weekEndDate: string; // ISO date string
  totalShifts: number;
  totalHours: number;
  totalEmployees: number;
  warnings: ParserWarning[];
}

/**
 * Parser warning (non-fatal issue from Agent Service).
 * Examples: missing break duration, employee not found, data quality concerns.
 */
export interface ParserWarning {
  code: string;
  message: string;
  row: number;
  column?: string;
  value?: string;
  hint?: string;
}

/**
 * Upload roster file to backend for parsing and storage.
 * File is uploaded to S3 and parsed via Agent Service.
 * Returns roster ID and any warnings (non-fatal issues).
 *
 * @param file - Roster Excel file (.xlsx)
 * @returns Promise with roster ID, summary, and warnings
 * @throws ApiError with normalized error structure
 */
export async function uploadRoster(file: File): Promise<UploadRosterResponse> {
  const formData = new FormData();
  formData.append("file", file);

  // Use httpClient directly for FormData upload
  // Override default application/json Content-Type; axios will append the boundary automatically
  try {
    const response = await httpClient.post<UploadRosterResponse>(
      "/roster/upload",
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );

    return response.data;
  } catch (err) {
    throw normalizeApiError(err);
  }
}

// ─── Roster Details (GET) ────────────────────────────────────────────

export interface ShiftSummary {
  shiftId: string
  date: string
  startTime: string
  endTime: string
  duration: number
  hasMealBreak: boolean
  mealBreakDuration: number | null
  location: string | null
}

export interface EmployeeShiftGroup {
  employeeId: string
  employeeName: string
  employeeNumber: string | null
  shiftCount: number
  totalHours: number
  shifts: ShiftSummary[]
}

export interface RosterDetailsResponse {
  rosterId: string
  weekStartDate: string
  weekEndDate: string
  weekNumber: number
  year: number
  totalShifts: number
  totalHours: number
  totalEmployees: number
  isFinalized: boolean
  originalFileName: string | null
  createdAt: string
  hasValidation: boolean
  validationId: string | null
  employees: EmployeeShiftGroup[]
}

/**
 * Fetch roster details by ID.
 * Returns roster metadata and shifts grouped by employee.
 */
export async function getRosterDetails(rosterId: string): Promise<RosterDetailsResponse> {
  try {
    const response = await httpClient.get<RosterDetailsResponse>(`/roster/${rosterId}`)
    return response.data
  } catch (err) {
    throw normalizeApiError(err)
  }
}

// ─── Roster Validation ───────────────────────────────────────────────

/**
 * Individual compliance issue found during validation.
 */
export interface RosterIssueSummary {
  id: string
  shiftId: string | null
  employeeId: string
  employeeName: string | null
  employeeNumber: string | null
  checkType: string
  severity: string
  description: string
  expectedValue: number | null
  actualValue: number | null
  affectedDates: string | null
}

/**
 * Response from roster validation endpoint.
 * Includes roster metadata and all compliance issues found.
 */
export interface ValidateRosterResponse {
  validationId: string
  status: string
  totalShifts: number
  passedShifts: number
  failedShifts: number
  totalIssues: number
  criticalIssues: number
  affectedEmployees: number
  weekStartDate: string
  weekEndDate: string
  totalEmployees: number
  validatedAt: string | null
  issues: RosterIssueSummary[]
}

/**
 * Trigger compliance validation for a roster (idempotent).
 * Returns existing results if validation already completed.
 */
export async function validateRoster(rosterId: string): Promise<ValidateRosterResponse> {
  try {
    const response = await httpClient.post<ValidateRosterResponse>(
      `/roster/${rosterId}/validate`
    )
    return response.data
  } catch (err) {
    throw normalizeApiError(err)
  }
}

/**
 * Get existing validation results for a roster.
 * Returns 404 if no validation has been run yet.
 */
export async function getValidationResults(rosterId: string): Promise<ValidateRosterResponse> {
  try {
    const response = await httpClient.get<ValidateRosterResponse>(
      `/roster/${rosterId}/validation`
    )
    return response.data
  } catch (err) {
    throw normalizeApiError(err)
  }
}

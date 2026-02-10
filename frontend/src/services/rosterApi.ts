import { post } from "./baseApi";
import httpClient from "./httpClient";

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
 * @param file - Roster Excel file (.xlsx or .xls)
 * @returns Promise with roster ID, summary, and warnings
 * @throws ApiError with normalized error structure
 */
export async function uploadRoster(file: File): Promise<UploadRosterResponse> {
  const formData = new FormData();
  formData.append("file", file);

  // Use httpClient directly for FormData upload
  // Browser will automatically set Content-Type with boundary for multipart/form-data
  const response = await httpClient.post<UploadRosterResponse>(
    "/roster/upload",
    formData,
    {
      headers: {
        // Remove default Content-Type to let browser set multipart/form-data with boundary
        'Content-Type': undefined,
      },
    } as any
  );

  return response.data;
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
  const response = await httpClient.get<RosterDetailsResponse>(`/roster/${rosterId}`)
  return response.data
}

// ─── Compliance Check (placeholder) ─────────────────────────────────

/**
 * Roster 合规检查 API 骨架
 * 后续会补：
 * - CSV 上传与校验
 * - 结果查询/导出
 */
export interface RosterComplianceCheckRequest {
  fileId: string;
  awardCode?: string;
  rosterStart?: string;
  rosterEnd?: string;
}

export interface RosterComplianceCheckResponse {
  runId: string;
}

export function startRosterComplianceCheck(
  payload: RosterComplianceCheckRequest,
): Promise<RosterComplianceCheckResponse> {
  return post<RosterComplianceCheckResponse, RosterComplianceCheckRequest>(
    "/roster/compliance/check",
    payload,
  );
}

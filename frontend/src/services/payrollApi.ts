import { post } from "./baseApi";

/**
 * Payroll 模块 API 骨架
 * 后续会补：
 * - pay run CSV import check
 * - SG check
 * - STP field validation
 */

export interface PayrollPlaceholderRequest {
  ping: true;
}

export interface PayrollPlaceholderResponse {
  ok: boolean;
}

export function payrollHealthCheck(): Promise<PayrollPlaceholderResponse> {
  return post<PayrollPlaceholderResponse, PayrollPlaceholderRequest>(
    "/payroll/health",
    { ping: true },
  );
}
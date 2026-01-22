import { post } from "./baseApi";

/**
 * Payroll 合规检查 API 骨架
 * 后续会补：
 * - CSV 上传与校验
 * - 结果查询/导出
 */

export interface PayrollComplianceCheckRequest {
  fileId: string;
  awardCode?: string;
  payPeriodStart?: string;
  payPeriodEnd?: string;
}

export interface PayrollComplianceCheckResponse {
  runId: string;
}

export function startPayrollComplianceCheck(
  payload: PayrollComplianceCheckRequest,
): Promise<PayrollComplianceCheckResponse> {
  return post<PayrollComplianceCheckResponse, PayrollComplianceCheckRequest>(
    "/payroll/compliance/check",
    payload,
  );
}

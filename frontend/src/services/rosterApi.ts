import { post } from "./baseApi";

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

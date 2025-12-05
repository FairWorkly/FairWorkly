import { post } from "./baseApi";

/**
 * 占位，证明链路可用
 * 后续 Compliance Q&A ticket 会在这里补真实 DTO
 */
export interface AskComplianceQuestionRequest {
  question: string;
  awardCode?: string;
  audience: "admin"|"manager" | "employee";
  orgId?: string;
  userId?: string;
}

export interface AskComplianceQuestionResponse {
  plainExplanation: string;
  keyPoints: string[];
  riskLevel: "green" | "yellow" | "red";
  whatToDoNext: string[];
  legalReferences: Array<{
    source: "NES" | "Award" | "FairWork" | "Other";
    title: string;
    url?: string | null;
  }>;
  disclaimer: string;
}

export function postComplianceQuestion(
  payload: AskComplianceQuestionRequest,
): Promise<AskComplianceQuestionResponse> {
  // 注意：baseURL 已在 httpClient 统一设置为 /api
  // 所以这里写 /compliance/qa
  return post<AskComplianceQuestionResponse, AskComplianceQuestionRequest>(
    "/compliance/qa",
    payload,
  );
}
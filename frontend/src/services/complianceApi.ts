import type { ComplianceQuestionPayload, AskComplianceQuestionResponse } from "../modules/compliance/types/compliance.types";
import { httpClient } from "./httpClient";

export async function postComplianceQuestion(
    payload: ComplianceQuestionPayload
): Promise<AskComplianceQuestionResponse> {
    const response = await httpClient.post<AskComplianceQuestionResponse, ComplianceQuestionPayload>(
        "/api/compliance/qa",
        payload
    )
    return response.data
}
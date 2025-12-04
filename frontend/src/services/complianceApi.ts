import type { AskComplianceRequest, AskComplianceQuestionResponse } from "../modules/compliance/types/compliance.types";
import { httpClient } from "./httpClient";

export interface ApiResponse<T> {
    success: boolean;
    data?: T;
    error?: {
        statusCode?: number;
        code: string;
        message: string;
    }
}

export async function postComplianceQuestion(
    payload: AskComplianceRequest
): Promise<ApiResponse<AskComplianceQuestionResponse>> {

    try {
        const response = await httpClient.post<AskComplianceQuestionResponse, AskComplianceRequest>(
            "/api/compliance/qa",
            payload
        )
        return {
            success: true,
            data: response,
        }

    } catch (error:any) {
            if (error.response) {
                const statusCode = error.response.status;

                if (statusCode >= 400 && statusCode < 500) {
                    return {
                        success: false,
                        error: {
                            code: 'CLIENT_ERROR',
                            statusCode: statusCode,
                            message: error.response.data?.message || "Client side error. Please try again later."
                        }
                    }
                }
                if (statusCode >= 500) {
                    return {
                        success: false,
                        error: {
                            code: 'SERVER_ERROR',
                            statusCode: statusCode,
                            message: error.response.data?.message || "Server side error. Please try again later."
                        }
                    }
                }
            }
            if (error.request) {
                return {
                    success: false,
                    error: {
                        code: 'NETWORK_ERROR',
                        message: "Connection error. Please try again later."
                    }
                }
            }

            return {
                success: false,
                error: {
                    code: 'UNKNOWN_ERROR',
                    message: error.message || 'Unknown error. Please try again later.'
                }
            }
        }
    }

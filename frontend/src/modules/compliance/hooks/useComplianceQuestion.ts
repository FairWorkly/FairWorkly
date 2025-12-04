import { useMutation } from '@tanstack/react-query';
import { postComplianceQuestion, type ApiResponse } from '../../../services/complianceApi';
import type { AskComplianceQuestionResponse, AskComplianceRequest } from '../types/compliance.types';



export function useComplianceQuestion() {
    const mutation = useMutation<
        ApiResponse<AskComplianceQuestionResponse>,
        Error,
        AskComplianceRequest
    >({
        mutationFn: async (payload) => await postComplianceQuestion(payload)
    })


    return {
        ...mutation,
    }
}


import { useMutation  } from '@tanstack/react-query';
import { postComplianceQuestion } from '../../../services/complianceApi';
import type { AskComplianceQuestionResponse, ComplianceQuestionPayload } from '../types/compliance.types';



export function useComplianceQuestion(){
    const mutation = useMutation<
    AskComplianceQuestionResponse,
    Error,
    ComplianceQuestionPayload
    >({
        mutationFn: (payload) => postComplianceQuestion(payload)
    })

    const genericErrorMessage = mutation.isError
    ? "Something went wrong while asking the compliance question"
    : null;

    return {
        ...mutation, 
        genericErrorMessage,
    }
}


import { useApiMutation } from "../../../shared/hooks/useApiMutation";
import {
  postComplianceQuestion,
  type AskComplianceQuestionRequest,
  type AskComplianceQuestionResponse,
} from "../../../services/complianceApi";

export function useComplianceQuestion() {
  return useApiMutation<AskComplianceQuestionResponse, AskComplianceQuestionRequest>(
    {
      mutationFn: postComplianceQuestion,
    },
  );
}
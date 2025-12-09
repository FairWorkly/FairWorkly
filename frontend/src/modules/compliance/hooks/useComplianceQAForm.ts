import { useCallback, useState } from "react";
import type { ChangeEvent, MouseEvent } from "react";
import type { SelectChangeEvent } from "@mui/material/Select";
import * as Constants from "../constants/ComplianceConstants.ts";
import type * as Types from "../types/compliance.types.ts";

type AwardCode = keyof typeof Constants.AWARD_OPTIONS;

interface UseComplianceQAFormParams {
  onSubmit?: (values: Types.ComplianceQAFormValues) => void;
}

export const useComplianceQAForm = (params?: UseComplianceQAFormParams) => {
  const [question, setQuestion] = useState("");
  const [showQuestionError, setShowQuestionError] = useState(false);
  const [awardCode, setAwardCode] = useState<AwardCode>("");
  const [audience, setAudience] = useState<Types.AudienceOption>(
    Constants.AUDIENCE_OPTIONS[0],
  );

  const onSubmit = params?.onSubmit;

  const handleSubmit = useCallback(
    (values: Types.ComplianceQAFormValues) => {
      if (onSubmit) {
        onSubmit(values);
        return;
      }

      console.log("Compliance QA submission", values);
    },
    [onSubmit],
  );

  const handleAsk = useCallback(() => {
    const trimmedQuestion = question.trim();
    if (trimmedQuestion.length < Constants.MIN_QUESTION_LENGTH) {
      setShowQuestionError(true);
      return;
    }

    setShowQuestionError(false);

    const submission: Types.ComplianceQAFormValues = {
      question: trimmedQuestion,
      audience,
    };

    if (awardCode) {
      submission.awardCode = awardCode;
    }

    handleSubmit(submission);
  }, [audience, awardCode, handleSubmit, question]);

  const handleChangeQuestion = useCallback(
    (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      const nextValue = event.target.value;
      const isValid = nextValue.trim().length >= Constants.MIN_QUESTION_LENGTH;

      if (showQuestionError && isValid) {
        setShowQuestionError(false);
      }
      setQuestion(nextValue);
    },
    [showQuestionError],
  );

  const handleAwardCode = useCallback((event: SelectChangeEvent) => {
    setAwardCode(event.target.value as AwardCode);
  }, []);

  const handleAudienceOption = useCallback(
    (_event: MouseEvent<HTMLElement>, nextAudience: Types.AudienceOption | null) => {
      if (nextAudience !== null) {
        setAudience(nextAudience);
      }
    },
    [],
  );

  return {
    question,
    showQuestionError,
    awardCode,
    audience,
    handleAsk,
    handleChangeQuestion,
    handleAwardCode,
    handleAudienceOption,
  };
};

export type ComplianceQAFormController = ReturnType<typeof useComplianceQAForm>;

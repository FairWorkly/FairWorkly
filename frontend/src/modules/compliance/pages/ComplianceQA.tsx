import React from "react";
import {
  ComplianceQATextField,
  ComplianceAwardOption,
  ComplianceSubmissionButton,
  ComplianceAudienceGroup,
} from "../features";
import { useComplianceQAForm } from "../hooks";
import { ComplianceQADescription, ComplianceQATitle } from "../ui";
import { Box, FormControl } from "@mui/material";

export const ComplianceQA: React.FC = () => {
  const {
    question,
    showQuestionError,
    awardCode,
    audience,
    handleAsk,
    handleChangeQuestion,
    handleAwardCode,
    handleAudienceOption,
  } = useComplianceQAForm();

  return (
    <Box>
      <ComplianceQATitle />
      <ComplianceQADescription />
      <FormControl>
        <ComplianceQATextField
          question={question}
          showQuestionError={showQuestionError}
          handleChangeQuestion={handleChangeQuestion}
        />
        <FormControl>
          <ComplianceAwardOption
            awardCode={awardCode}
            handleAwardCode={handleAwardCode}
          />
          <ComplianceAudienceGroup
            audience={audience}
            handleAudienceOption={handleAudienceOption}
          />
        </FormControl>
        <ComplianceSubmissionButton
          question={question}
          handleAsk={handleAsk}
        />
      </FormControl>
    </Box>
  );
};

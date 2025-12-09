import React from "react";
import {
  ComplianceQATextField,
  ComplianceAwardOption,
  ComplianceSubmissionButton,
  ComplianceAudienceGroup,
} from "../features";
import { useComplianceQAForm } from "../hooks";
import { ComplianceQADescription, ComplianceQATitle } from "../ui";
import { Box, FormControl, Stack } from "@mui/material";

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
    <Box maxWidth={720} width="100%">
      <Stack spacing={3}>
        <ComplianceQATitle />
        <ComplianceQADescription />
        <Stack spacing={2}>
          <ComplianceQATextField
            question={question}
            showQuestionError={showQuestionError}
            handleChangeQuestion={handleChangeQuestion}
          />
          <Stack spacing={1.5}>
            <FormControl fullWidth>
              <ComplianceAwardOption
                awardCode={awardCode}
                handleAwardCode={handleAwardCode}
              />
            </FormControl>
            <ComplianceAudienceGroup
              audience={audience}
              handleAudienceOption={handleAudienceOption}
            />
          </Stack>
          <ComplianceSubmissionButton
            question={question}
            handleAsk={handleAsk}
          />
        </Stack>
      </Stack>
    </Box>
  );
};

import React, { useState } from "react";
import * as Constants from "../constants/ComplianceConstants.ts";
import * as Types from "../types/compliance.types.ts";
import {
  Button,
  Box,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
  TextField,
  type SelectChangeEvent,
} from "@mui/material";

type AwardCode = keyof typeof Constants.AWARD_OPTIONS;

export const ComplianceQA: React.FC = () => {
  const [question, setQuestion] = useState<string>("");
  const [showQuestionError, setShowQuestionError] = useState<boolean>(false);
  const [awardCode, setAwardCode] = useState<AwardCode>("");
  const [audience, setAudience] = useState<Types.AudienceOption>(
    Constants.AUDIENCE_OPTIONS[0],
  );

  const handleAsk = () => {
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
  };

  const handleChangeQuestion = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const nextValue = event.target.value;
    const isValid = nextValue.trim().length >= Constants.MIN_QUESTION_LENGTH;

    if (showQuestionError && isValid) {
      setShowQuestionError(false);
    }
    setQuestion(nextValue);
  };

  const handleAwardCode = (event: SelectChangeEvent) => {
    setAwardCode(event.target.value as AwardCode);
  };

  const handleAudienceOption = (
    _event: React.MouseEvent<HTMLElement>,
    nextAudience: Types.AudienceOption | null,
  ) => {
    if (nextAudience !== null) {
      setAudience(nextAudience);
    }
  };

  const handleSubmit = (values: Types.ComplianceQAFormValues) => {
    // please modify this once you know what to do
    console.log("Compliance QA submission", values);
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom >
        {Constants.QA_PAGE_TITLE}
      </Typography >
      <Typography variant="body1">
        {Constants.QA_PAGE_DESCRIPTION}
      </Typography>
      <FormControl>
        <TextField
          id="compliance-qa-textfield"
          placeholder="Ask anything"
          variant="standard"
          required
          multiline
          maxRows={8}
          value={question}
          onChange={handleChangeQuestion}
          error={showQuestionError}
          helperText={
            showQuestionError
              ? `Please enter at least ${Constants.MIN_QUESTION_LENGTH} characters to continue.`
              : undefined
          }
          slotProps={{ htmlInput: { minLength: Constants.MIN_QUESTION_LENGTH } }}
        />

        <FormControl>
          <InputLabel id="compliance-qa-award-select-label">Award</InputLabel>
          <Select
            labelId="compliance-qa-award-select-label"
            id="compliance-qa-award-select"
            value={awardCode}
            onChange={handleAwardCode}
            label="Award"
          >
            {Object.entries(Constants.AWARD_OPTIONS).map(([code, label]) => (
              <MenuItem key={code} value={code}>
                {label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        <ToggleButtonGroup
          color="primary"
          value={audience}
          exclusive
          onChange={handleAudienceOption}
          aria-label="Platform"
        >
          {Constants.AUDIENCE_OPTIONS.map((option) => (
            <ToggleButton key={option} value={option}>
              {option}
            </ToggleButton>
          ))}
        </ToggleButtonGroup>

        <Button
          variant="contained"
          onClick={handleAsk}
          disabled={question.trim().length < Constants.MIN_QUESTION_LENGTH}
        >
          Ask
        </Button>
      </FormControl>
    </Box >
  );
};

import React, { useState } from "react";
import * as Constants from "../constants/ComplianceConstants.ts";
import * as Types from "../types/compliance.types.ts";
import {
  Button,
  Box,
  FormControl,
  MenuItem,
  Select,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
  TextField,
  type SelectChangeEvent,
} from "@mui/material";

export const ComplianceQA: React.FC = () => {
  const [question, setQuestion] = useState<string>("");
  const [showQuestionError, setShowQuestionError] = useState<boolean>(false);
  const [awardCode, setAwardCode] = useState<string>(Constants.AWARD_OPTIONS[0]);
  const [audience, setAudience] = useState<string>(Constants.AUDIENCE_OPTIONS[0]);

  const handleAsk = () => {
    const trimmedQuestion = question.trim();
    if (trimmedQuestion.length < Constants.MIN_QUESTION_LENGTH) {
      setShowQuestionError(true);
      return;
    }

    setShowQuestionError(false);
    handleSubmit({ question: trimmedQuestion });
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
    setAwardCode(event.target.value as string);
  };

  const handleAudienceOption = (
    _event: React.MouseEvent<HTMLElement>,
    nextAudience: string | null,
  ) => {
    if (nextAudience !== null) {
      setAudience(nextAudience);
    }
  };

  const handleSubmit = (values: Types.ComplianceQAFormSumitionValues) => {
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

        <Select
          labelId="compliance-qa-award-select-label"
          id="demo-simple-select"
          value={awardCode}
          onChange={handleAwardCode}
        >
          {Constants.AWARD_OPTIONS.map((award) => (
            <MenuItem key={award} value={award}>
              {award}
            </MenuItem>
          ))}
        </Select>
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

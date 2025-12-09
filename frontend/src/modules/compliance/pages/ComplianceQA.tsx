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
} from "@mui/material";

export const ComplianceQA: React.FC = () => {
  const [question, setQuestion] = useState<string>("");
  const [showQuestionError, setShowQuestionError] = useState<boolean>(false);
  const [awardCode, setAwardcode] = useState<string>(Constants.AWARD_OPTIONS[0]);
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

  const handleAwardCode = (event: Types.SelectChangeEvent) => {
    setAwardcode(event.target.value as string);
  }

  const handleAudienceOption = (event: React.MouseEvent<HTMLElement>) => {
    setAudience(event.target.value as string);
  }

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
      <TextField
        id="compliance-qa-textfield"
        placeholder="Ask anything"
        variant="standard"
        required
        multiline
        fullWidth
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

      <FormControl fullWidth>
        <InputLabel id="compliance-qa-award-select-label">Award</InputLabel>
        <Select
          labelId="compliance-qa-award-select-label"
          id="demo-simple-select"
          value={awardCode}
          onChange={handleAwardCode}
        >
          {Constants.AWARD_OPTIONS.map((awardCode) => (
            <MenuItem key={awardCode} value={awardCode}>{awardCode}</MenuItem>
          ))}
        </Select>
        <ToggleButtonGroup
          color="primary"
          value={audience}
          exclusive
          onChange={handleAudienceOption}
          aria-label="Platform"
        >
          <ToggleButton value="web">Web</ToggleButton>
          <ToggleButton value="android">Android</ToggleButton>
          <ToggleButton value="ios">iOS</ToggleButton>
        </ToggleButtonGroup>
      </FormControl>

      <Button
        variant="contained"
        onClick={handleAsk}
        disabled={question.trim().length < Constants.MIN_QUESTION_LENGTH}
      >
        Ask
      </Button>
    </Box >
  );
};

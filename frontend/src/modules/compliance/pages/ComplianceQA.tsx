import React, { useState } from "react";
import { Box, Typography, TextField, Button } from "@mui/material";
import type { ComplianceQAFormValues } from "../types/compliance.types";

const MIN_QUESTION_LENGTH = 3;


export const ComplianceQA: React.FC = () => {
  const [question, setQuestion] = useState("");
  const [showQuestionError, setShowQuestionError] = useState(false);

  const handleAsk = () => {
    const trimmedQuestion = question.trim();
    if (trimmedQuestion.length < MIN_QUESTION_LENGTH) {
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
    const isValid = nextValue.trim().length >= MIN_QUESTION_LENGTH;

    if (showQuestionError && isValid) {
      setShowQuestionError(false);
    }
    setQuestion(nextValue);
  };

  const handleSubmit = (values: ComplianceQAFormValues) => {
    // please modify this once you know what to do
    console.log("Compliance QA submission", values);
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Compliance Q&A (AI Copilot)
      </Typography>
      <Typography variant="body1">
        This page will host the AI Q&A interface for compliance questions.
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
            ? `Please enter at least ${MIN_QUESTION_LENGTH} characters to continue.`
            : undefined
        }
        slotProps={{ htmlInput: { minLength: MIN_QUESTION_LENGTH } }}
      />
      <Button
        variant="contained"
        onClick={handleAsk}
        disabled={question.trim().length < MIN_QUESTION_LENGTH}
      >
        Ask
      </Button>
    </Box>
  );
};

import React, { useState } from "react";
import { Box, Typography, TextField, Button } from "@mui/material";

export const ComplianceQA: React.FC = () => {
  const [question, setQuestion] = useState("");
  const [showQuestionError, setShowQuestionError] = useState(false);

  const handleAsk = () => {
    if (!question.trim()) {
      setShowQuestionError(true);
      return;
    }
    setShowQuestionError(false);
  };

  const handleChangeQuestion = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    if (showQuestionError) {
      setShowQuestionError(false);
    }
    setQuestion(event.target.value);
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
        helperText={showQuestionError ? "Please enter a question to continue." : undefined}
        slotProps={{ htmlInput: { minLength: 1 } }}
      />
      <Button variant="contained" onClick={handleAsk} disabled={!question.trim()}>
        Ask
      </Button>
    </Box>
  );
};

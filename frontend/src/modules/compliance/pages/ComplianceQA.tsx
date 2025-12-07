import React from "react";
import { Box, Typography, TextField, Button } from "@mui/material";

export const ComplianceQA: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Compliance Q&A (AI Copilot)
      </Typography>
      <Typography variant="body1">
        This page will host the AI Q&A interface for compliance questions.
      </Typography>
      <TextField
        id="filled-multiline-flexible-icon"
        placeholder="Ask anything"
        variant="standard"
        required
        multiline
        fullWidth
        maxRows={8}
        slotProps={{ htmlInput: { minLength: 1 } }}
      />
      <Button>Ask</Button>
    </Box>
  );
};

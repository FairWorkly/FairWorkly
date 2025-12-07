import React from "react";
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  IconButton,
  InputAdornment,
} from "@mui/material";
import { Add } from "@mui/icons-material";

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
        inputProps={{ minLength: 3 }}
      />
      <Button>Ask</Button>
    </Box>
  );
};

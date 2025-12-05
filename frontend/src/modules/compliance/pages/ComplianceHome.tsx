import React from "react";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";

export const ComplianceHome: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Compliance Agent (Home)
      </Typography>
      <Typography variant="body1">
        Overview of Compliance features (Q&A, roster checks, risk reports).
      </Typography>
    </Box>
  );
};
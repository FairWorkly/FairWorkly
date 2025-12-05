import React from "react";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";

export const EmployeeHome: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Employee Portal / Help
      </Typography>
      <Typography variant="body1">
        This area will host employee self-service features and Q&A (MVP placeholder).
      </Typography>
    </Box>
  );
};
import React from "react";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";

export const PayrollHome: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Payroll & STP Checks
      </Typography>
      <Typography variant="body1">
        This module will provide second-line checks on pay runs, STP, and SG (MVP placeholder).
      </Typography>
    </Box>
  );
};
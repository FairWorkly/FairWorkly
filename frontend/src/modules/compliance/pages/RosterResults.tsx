import React from "react";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";

export const RosterResults: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Roster Check Results
      </Typography>
      <Typography variant="body1">
        This page will display issues, risk levels and suggestions based on the
        roster check.
      </Typography>
    </Box>
  );
};
import React from "react";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";

export const RosterCheck: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Roster Check
      </Typography>
      <Typography variant="body1">
        Upload rosters here and run basic award/NES checks (MVP placeholder).
      </Typography>
    </Box>
  );
};
import React from "react";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";

export const DocumentsHome: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Documents & Contracts
      </Typography>
      <Typography variant="body1">
        Generate, upload and manage contracts, letters and HR documents (MVP placeholder).
      </Typography>
    </Box>
  );
};
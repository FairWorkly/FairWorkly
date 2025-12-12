import React from "react";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";
import { ProblemSection } from "../components/sections/ProblemSection";
import { FeaturesSection } from "../components/sections/FeaturesSection";
import { ShowcaseSection } from "../components/sections/ShowcaseSection";

export const FairWorklyHome: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        FairWorkly – Workplace Compliance Copilot
      </Typography>
      <Typography variant="body1">
        Welcome! This is the product home page. From here you can navigate to
        Compliance, Payroll, Documents and Employee tools.
      </Typography>
      <ProblemSection />
      <FeaturesSection />
      <ShowcaseSection />
    </Box>
  );
};
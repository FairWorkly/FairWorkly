import React from "react";
import {
  ComplianceQATextField,
  ComplianceAwardOption,
  ComplilanceSubmitionButton,
  CompliabceAudienceGroup,
} from "../features";
import {
  ComplianceQADescription,
  ComplianceQATitle
} from "../ui";
import {
  Box,
  FormControl,
} from "@mui/material";

export const ComplianceQA: React.FC = () => {
  return (
    <Box>
      <ComplianceQATitle />
      <ComplianceQADescription />
      <FormControl>
        <ComplianceQATextField />
        <FormControl>
          <ComplianceAwardOption />
          <CompliabceAudienceGroup />
        </FormControl>
        <ComplilanceSubmitionButton />
      </FormControl>
    </Box >
  );
};

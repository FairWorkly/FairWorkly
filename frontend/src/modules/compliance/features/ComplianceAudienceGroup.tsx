import { ToggleButtonGroup, ToggleButton } from "@mui/material";
import { AUDIENCE_OPTIONS } from "../constants/ComplianceConstants";
import type { ComplianceQAFormController } from "../hooks";

type Props = Pick<ComplianceQAFormController, "audience" | "handleAudienceOption">;

const ComplianceAudienceGroup = ({ audience, handleAudienceOption }: Props) => {
  return (
    <ToggleButtonGroup
      color="primary"
      value={audience}
      exclusive
      onChange={handleAudienceOption}
      aria-label="Platform"
    >
      {AUDIENCE_OPTIONS.map((option) => (
        <ToggleButton key={option} value={option}>
          {option}
        </ToggleButton>
      ))}
    </ToggleButtonGroup>
  );
};

export default ComplianceAudienceGroup;

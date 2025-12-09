import { ToggleButtonGroup, ToggleButton } from "@mui/material"
import { AUDIENCE_OPTIONS } from "../constants/ComplianceConstants"
import { useComplianceQAForm } from "../hooks";
const CompliabceAudienceGroup = () => {
    const {
        audience,
        handleAudienceOption,
    } = useComplianceQAForm();

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
    )
}

export default CompliabceAudienceGroup
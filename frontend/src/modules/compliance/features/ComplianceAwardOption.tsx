import { InputLabel, Select, MenuItem } from "@mui/material"
import { useComplianceQAForm } from "../hooks";
import { AWARD_OPTIONS } from "../constants/ComplianceConstants";

const ComplianceAwardOption = () => {
    const {
        awardCode,
        handleAwardCode,
    } = useComplianceQAForm();

    return (
        <>
            <InputLabel id="compliance-qa-award-select-label">Award</InputLabel>
            <Select
                labelId="compliance-qa-award-select-label"
                id="compliance-qa-award-select"
                value={awardCode}
                onChange={handleAwardCode}
                label="Award"
            >
                {Object.entries(AWARD_OPTIONS).map(([code, label]) => (
                    <MenuItem key={code} value={code}>
                        {label}
                    </MenuItem>
                ))}
            </Select>
        </>
    )
}

export default ComplianceAwardOption
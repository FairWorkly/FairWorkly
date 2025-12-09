import { TextField } from "@mui/material"
import { MIN_QUESTION_LENGTH } from "../constants/ComplianceConstants"
import { useComplianceQAForm } from "../hooks";

const ComplianceQATextField = () => {
    const {
        question,
        showQuestionError,
        handleChangeQuestion,
    } = useComplianceQAForm();

    return (
        <TextField
            id="compliance-qa-textfield"
            placeholder="Ask anything"
            variant="standard"
            required
            multiline
            maxRows={8}
            value={question}
            onChange={handleChangeQuestion}
            error={showQuestionError}
            helperText={
                showQuestionError
                    ? `Please enter at least ${MIN_QUESTION_LENGTH} characters to continue.`
                    : undefined
            }
            slotProps={{ htmlInput: { minLength: MIN_QUESTION_LENGTH } }}
        />
    );
}

export default ComplianceQATextField;
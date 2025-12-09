import { Button } from "@mui/material"
import { MIN_QUESTION_LENGTH } from "../constants/ComplianceConstants"
import { useComplianceQAForm } from "../hooks";

const ComplilanceSubmitionButton = () => {
    const {
        question,
        handleAsk,
    } = useComplianceQAForm();

    return (
        <Button
            variant="contained"
            onClick={handleAsk}
            disabled={question.trim().length < MIN_QUESTION_LENGTH}
        >
            Ask
        </Button>
    )
}

export default ComplilanceSubmitionButton
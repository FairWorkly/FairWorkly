import { Button } from "@mui/material";
import { MIN_QUESTION_LENGTH } from "../constants/ComplianceConstants";
import type { ComplianceQAFormController } from "../hooks";

type Props = Pick<ComplianceQAFormController, "question" | "handleAsk">;

const ComplianceSubmissionButton = ({ question, handleAsk }: Props) => {
  return (
    <Button
      variant="contained"
      onClick={handleAsk}
      disabled={question.trim().length < MIN_QUESTION_LENGTH}
    >
      Ask
    </Button>
  );
};

export default ComplianceSubmissionButton;

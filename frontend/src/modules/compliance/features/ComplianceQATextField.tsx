import { TextField } from "@mui/material";
import { MIN_QUESTION_LENGTH } from "../constants/ComplianceConstants";
import type { ComplianceQAFormController } from "../hooks";

type Props = Pick<
  ComplianceQAFormController,
  "question" | "showQuestionError" | "handleChangeQuestion"
>;

const ComplianceQATextField = ({
  question,
  showQuestionError,
  handleChangeQuestion,
}: Props) => {
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
};

export default ComplianceQATextField;

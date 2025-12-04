
import { useComplianceQuestion } from '../hooks/useComplianceQuestion';

const ComplianceQA = () => {

    const { mutate, isLoading, isError, data, error, genericErrorMessage } =
        useComplianceQuestion();

    return (
        <div>
            <p>ComplianceQA</p>
        </div>
    )
}

export default ComplianceQA
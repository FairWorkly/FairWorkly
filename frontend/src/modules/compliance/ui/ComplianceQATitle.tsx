import { QA_PAGE_TITLE } from '../constants/ComplianceConstants'
import { Typography } from '@mui/material'

const ComplianceQATitle = () => {
    return (
        <Typography variant="h4" gutterBottom >
            {QA_PAGE_TITLE}
        </Typography >
    )
}

export default ComplianceQATitle
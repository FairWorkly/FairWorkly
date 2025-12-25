import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import Stack from '@mui/material/Stack'
import Divider from '@mui/material/Divider'
import { PayrollSummary } from './PayrollSummary'
import { RosterSummary } from './RosterSummary'
import {
  FAIRBOT_LABELS,
  FAIRBOT_RESULTS,
  FAIRBOT_RESULTS_UI,
} from '../../constants/fairbot.constants'
import type { FairBotResult } from '../../types/fairbot.types'

interface QuickSummaryProps {
  result: FairBotResult
}

const SummaryContainer = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_RESULTS_UI.STACK_GAP}px`,
})

export const QuickSummary = ({ result }: QuickSummaryProps) => {
  // Switch on result type to render the appropriate summary card.
  switch (result.type) {
    case FAIRBOT_RESULTS.TYPES.PAYROLL:
      return <PayrollSummary data={result.data} detailsUrl={result.detailsUrl} />
    case FAIRBOT_RESULTS.TYPES.ROSTER:
      return <RosterSummary data={result.data} detailsUrl={result.detailsUrl} />
    case FAIRBOT_RESULTS.TYPES.EMPLOYEE:
      return (
        <SummaryContainer>
          <Typography variant="h6">{FAIRBOT_LABELS.EMPLOYEE_SUMMARY_TITLE}</Typography>
          <Divider />
          <Typography variant="body2">{FAIRBOT_LABELS.EMPLOYEE_ISSUES_SUMMARY}</Typography>
        </SummaryContainer>
      )
    case FAIRBOT_RESULTS.TYPES.DOCUMENT:
      return (
        <SummaryContainer>
          <Typography variant="h6">{FAIRBOT_LABELS.DOCUMENT_SUMMARY_TITLE}</Typography>
          <Divider />
          <Typography variant="body2">{FAIRBOT_LABELS.DOCUMENTS_SUMMARY}</Typography>
        </SummaryContainer>
      )
    default:
      return (
        <Stack spacing={FAIRBOT_RESULTS_UI.STAT_GAP}>
          <Typography variant="h6">{FAIRBOT_LABELS.RESULTS_PANEL_TITLE}</Typography>
          <Typography variant="body2">{FAIRBOT_LABELS.RESULTS_PANEL_SUBTITLE}</Typography>
        </Stack>
      )
  }
}

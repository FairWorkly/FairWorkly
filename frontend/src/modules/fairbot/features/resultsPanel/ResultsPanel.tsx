import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
  FAIRBOT_RESULTS_UI,
} from '../../constants/fairbot.constants'
import { useResultsPanel } from '../../hooks/useResultsPanel'
import { ResultsEmpty } from './ResultsEmpty'
import { QuickSummary } from './QuickSummary'

// Results panel reads the latest summary from session storage and renders the right state.
const PanelContainer = styled('section')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.RESULTS_PANEL_GAP}px`,
  padding: `${FAIRBOT_LAYOUT.RESULTS_PANEL_PADDING}px`,
  backgroundColor: theme.palette.background.paper,
  borderRadius: `${FAIRBOT_RESULTS_UI.CARD_RADIUS}px`,
  border: `${FAIRBOT_RESULTS_UI.CARD_BORDER_WIDTH}px solid ${theme.palette.divider}`,
  minHeight: `${FAIRBOT_RESULTS_UI.MIN_HEIGHT}px`,
}))

const PanelHeader = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_RESULTS_UI.HEADER_GAP}px`,
})

export const ResultsPanel = () => {
  const { currentResult } = useResultsPanel()

  return (
    <PanelContainer aria-label={FAIRBOT_ARIA.RESULTS_PANEL}>
      <PanelHeader>
        <Typography variant="subtitle1">{FAIRBOT_LABELS.RESULTS_PANEL_TITLE}</Typography>
        <Typography variant="body2" color="text.secondary">
          {FAIRBOT_LABELS.RESULTS_PANEL_SUBTITLE}
        </Typography>
      </PanelHeader>
      {currentResult ? <QuickSummary result={currentResult} /> : <ResultsEmpty />}
    </PanelContainer>
  )
}

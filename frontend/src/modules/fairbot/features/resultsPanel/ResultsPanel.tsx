import { styled } from '@mui/material/styles'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LAYOUT,
  FAIRBOT_RESULTS_UI,
} from '../../constants/fairbot.constants'
import { useResultsPanel } from '../../hooks/useResultsPanel'
import { ResultsEmpty } from './ResultsEmpty'
import { QuickSummary } from './QuickSummary'

// Results panel reads the latest summary from session storage and renders the right state.
const PanelContainer = styled('section')({
  display: FAIRBOT_LAYOUT.DISPLAY_FLEX,
  flexDirection: FAIRBOT_LAYOUT.FLEX_DIRECTION_COLUMN,
  gap: `${FAIRBOT_LAYOUT.RESULTS_PANEL_GAP}px`,
  padding: `${FAIRBOT_LAYOUT.RESULTS_PANEL_PADDING}px`,
  boxSizing: FAIRBOT_LAYOUT.BOX_SIZING_BORDER_BOX,
  borderRadius: `${FAIRBOT_RESULTS_UI.CARD_RADIUS}px`,
  border: FAIRBOT_RESULTS_UI.PANEL_BORDER,
  width: `min(${FAIRBOT_LAYOUT.RESULTS_PANEL_WIDTH}px, ${FAIRBOT_LAYOUT.COLUMN_FULL_WIDTH})`,
  maxWidth: FAIRBOT_LAYOUT.COLUMN_FULL_WIDTH,
  minHeight: `${FAIRBOT_RESULTS_UI.MIN_HEIGHT}px`,
})

export const ResultsPanel = () => {
  const { currentResult } = useResultsPanel()

  return (
    <PanelContainer aria-label={FAIRBOT_ARIA.RESULTS_PANEL}>
      {currentResult ? <QuickSummary result={currentResult} /> : <ResultsEmpty />}
    </PanelContainer>
  )
}

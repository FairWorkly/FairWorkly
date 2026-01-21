import { styled } from '@/styles/styled'
import {
  FAIRBOT_ARIA,
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
  gap: theme.spacing(2),
  padding: theme.spacing(3),
  boxSizing: 'border-box',
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.divider}`,
  width: `min(${FAIRBOT_LAYOUT.RESULTS_PANEL_WIDTH}px, 100%)`,
  maxWidth: '100%',
  minHeight: `${FAIRBOT_RESULTS_UI.MIN_HEIGHT}px`,
}))

export const ResultsPanel = () => {
  const { currentResult } = useResultsPanel()

  return (
    <PanelContainer aria-label={FAIRBOT_ARIA.RESULTS_PANEL}>
      {currentResult ? <QuickSummary result={currentResult} /> : <ResultsEmpty />}
    </PanelContainer>
  )
}

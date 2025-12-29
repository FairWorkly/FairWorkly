import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import AutoAwesomeOutlined from '@mui/icons-material/AutoAwesomeOutlined'
import {
  FAIRBOT_LABELS,
  FAIRBOT_RESULTS_UI,
} from '../../constants/fairbot.constants'

const EmptyStateContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  textAlign: 'center',
  gap: `${FAIRBOT_RESULTS_UI.STAT_GAP}px`,
  padding: `${FAIRBOT_RESULTS_UI.CARD_PADDING}px`,
  color: theme.palette.text.secondary,
}))

const EmptyIcon = styled('div')(({ theme }) => ({
  width: `${FAIRBOT_RESULTS_UI.EMPTY_ICON_RADIUS * 2}px`,
  height: `${FAIRBOT_RESULTS_UI.EMPTY_ICON_RADIUS * 2}px`,
  borderRadius: '50%',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  backgroundColor: theme.palette.action.hover,
  color: theme.palette.text.secondary,
}))

// Empty-state panel shown before any summary is available.
export const ResultsEmpty = () => {
  return (
    <EmptyStateContainer>
      <EmptyIcon>
        <AutoAwesomeOutlined fontSize="large" />
      </EmptyIcon>
      <Typography variant="h6">{FAIRBOT_LABELS.EMPTY_RESULTS_TITLE}</Typography>
      <Typography variant="body2">{FAIRBOT_LABELS.EMPTY_RESULTS_SUBTITLE}</Typography>
    </EmptyStateContainer>
  )
}

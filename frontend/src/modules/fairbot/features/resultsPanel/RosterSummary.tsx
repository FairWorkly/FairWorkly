import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import Button from '@mui/material/Button'
import Alert from '@mui/material/Alert'
import Stack from '@mui/material/Stack'
import { useNavigate } from 'react-router-dom'
import {
    FAIRBOT_LABELS,
    FAIRBOT_NUMBERS,
    FAIRBOT_RESULTS_UI,
} from '../../constants/fairbot.constants'
import type { RosterSummaryData } from '../../types/fairbot.types'

interface RosterSummaryProps {
    data: RosterSummaryData
    detailsUrl: string
}

const SummaryCard = styled('div')(({ theme }) => ({
    borderRadius: `${FAIRBOT_RESULTS_UI.CARD_RADIUS}px`,
    padding: `${FAIRBOT_RESULTS_UI.CARD_PADDING}px`,
    border: `1px solid ${theme.palette.divider}`,
    backgroundColor: theme.palette.background.paper,
    display: 'flex',
    flexDirection: 'column',
    gap: `${FAIRBOT_RESULTS_UI.STAT_GAP}px`,
}))

const StatsGrid = styled('div')({
    display: 'grid',
    gap: `${FAIRBOT_RESULTS_UI.STAT_GAP}px`,
    gridTemplateColumns: `repeat(${FAIRBOT_RESULTS_UI.STATS_GRID_COLUMNS}, minmax(0, 1fr))`,
})

const StatCard = styled('div')(({ theme }) => ({
    padding: `${FAIRBOT_RESULTS_UI.CARD_PADDING}px`,
    borderRadius: `${FAIRBOT_RESULTS_UI.CARD_RADIUS}px`,
    backgroundColor: theme.palette.action.hover,
}))

// Roster summary card with stats, issues list, and link to detailed results.
export const RosterSummary = ({ data, detailsUrl }: RosterSummaryProps) => {
    const navigate = useNavigate()
    const hasIssues = data.issuesFound > FAIRBOT_NUMBERS.ZERO

    return (
        <SummaryCard>
            <Stack spacing={FAIRBOT_RESULTS_UI.STAT_GAP}>
                <Typography variant="h6">{FAIRBOT_LABELS.ROSTER_SUMMARY_TITLE}</Typography>
                <StatsGrid>
                    <StatCard>
                        <Typography variant="overline">{FAIRBOT_LABELS.ISSUES_FOUND_LABEL}</Typography>
                        <Typography variant="h5">{data.issuesFound}</Typography>
                    </StatCard>
                    <StatCard>
                        <Typography variant="overline">{FAIRBOT_LABELS.SHIFT_COUNT_LABEL}</Typography>
                        <Typography variant="h5">{data.shiftCount ?? FAIRBOT_NUMBERS.ZERO}</Typography>
                    </StatCard>
                </StatsGrid>
            </Stack>

            {hasIssues ? (
                <Stack spacing={FAIRBOT_RESULTS_UI.LIST_GAP}>
                    <Typography variant="subtitle1">{FAIRBOT_LABELS.TOP_ISSUES_LABEL}</Typography>
                    {data.topIssues.slice(FAIRBOT_NUMBERS.ZERO, FAIRBOT_NUMBERS.THREE).map((issue) => (
                        <Alert key={issue.id} severity="warning">
                            {issue.description}
                        </Alert>
                    ))}
                </Stack>
            ) : (
                <Alert severity="success">{FAIRBOT_LABELS.NO_ROSTER_ISSUES}</Alert>
            )}

            <Button
                variant="contained"
                onClick={() => {
                    navigate(detailsUrl)
                }}
            >
                {FAIRBOT_LABELS.VIEW_DETAILED_REPORT}
            </Button>
        </SummaryCard>
    )
}
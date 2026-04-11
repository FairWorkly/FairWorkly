import { useNavigate } from 'react-router-dom'
import { styled } from '@/styles/styled'
import Paper from '@mui/material/Paper'
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'
import ShieldOutlinedIcon from '@mui/icons-material/ShieldOutlined'
import PaymentsOutlinedIcon from '@mui/icons-material/PaymentsOutlined'
import GavelOutlinedIcon from '@mui/icons-material/GavelOutlined'
import { alpha } from '@mui/material/styles'
import {
  FAIRBOT_ARIA,
  FAIRBOT_ACTION_CARDS,
} from '../constants/fairbot.constants'

const CardsGrid = styled('section')(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: 'repeat(3, 1fr)',
  gap: theme.spacing(2),
  [theme.breakpoints.down('md')]: {
    gridTemplateColumns: 'repeat(2, 1fr)',
  },
  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: '1fr',
  },
}))

const Card = styled(Paper)(({ theme }) => ({
  display: 'flex',
  alignItems: 'flex-start',
  gap: theme.spacing(2),
  padding: theme.spacing(3),
  borderRadius: `${theme.fairworkly.radius.xl}px`,
  border: `1px solid ${theme.palette.divider}`,
  boxShadow: theme.fairworkly.shadow.sm,
  cursor: 'pointer',
  transition: theme.transitions.create(
    ['border-color', 'box-shadow', 'transform'],
    {
      duration: theme.transitions.duration.short,
    }
  ),
  '&:hover': {
    borderColor: alpha(theme.palette.primary.main, 0.3),
    boxShadow: theme.fairworkly.shadow.md,
    transform: 'translateY(-2px)',
  },
}))

const IconBox = styled(Box)(({ theme }) => ({
  width: theme.spacing(6),
  height: theme.spacing(6),
  borderRadius: theme.spacing(1.5),
  display: 'grid',
  placeItems: 'center',
  flexShrink: 0,
}))

const RosterIconBox = styled(IconBox)(({ theme }) => ({
  background: alpha(theme.palette.info.main, 0.08),
  border: `1px solid ${alpha(theme.palette.info.main, 0.1)}`,
  color: theme.palette.info.main,
}))

const PayrollIconBox = styled(IconBox)(({ theme }) => ({
  background: alpha(theme.palette.success.main, 0.08),
  border: `1px solid ${alpha(theme.palette.success.main, 0.1)}`,
  color: theme.palette.success.main,
}))

const DebateIconBox = styled(IconBox)(({ theme }) => ({
  background: alpha(theme.palette.warning.main, 0.08),
  border: `1px solid ${alpha(theme.palette.warning.main, 0.1)}`,
  color: theme.palette.warning.main,
}))

const CardContent = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(0.5),
}))

interface ActionCardsProps {
  onDebateClick?: () => void
}

export const ActionCards = ({ onDebateClick }: ActionCardsProps) => {
  const navigate = useNavigate()

  return (
    <CardsGrid aria-label={FAIRBOT_ARIA.ACTION_CARDS}>
      <Card
        onClick={() => navigate(FAIRBOT_ACTION_CARDS.ROSTER.route)}
        role="button"
        tabIndex={0}
        onKeyDown={e => {
          if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault()
            navigate(FAIRBOT_ACTION_CARDS.ROSTER.route)
          }
        }}
      >
        <RosterIconBox>
          <ShieldOutlinedIcon />
        </RosterIconBox>
        <CardContent>
          <Typography variant="subtitle1" fontWeight="bold">
            {FAIRBOT_ACTION_CARDS.ROSTER.title}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {FAIRBOT_ACTION_CARDS.ROSTER.description}
          </Typography>
        </CardContent>
      </Card>

      <Card
        onClick={() => navigate(FAIRBOT_ACTION_CARDS.PAYROLL.route)}
        role="button"
        tabIndex={0}
        onKeyDown={e => {
          if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault()
            navigate(FAIRBOT_ACTION_CARDS.PAYROLL.route)
          }
        }}
      >
        <PayrollIconBox>
          <PaymentsOutlinedIcon />
        </PayrollIconBox>
        <CardContent>
          <Typography variant="subtitle1" fontWeight="bold">
            {FAIRBOT_ACTION_CARDS.PAYROLL.title}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {FAIRBOT_ACTION_CARDS.PAYROLL.description}
          </Typography>
        </CardContent>
      </Card>

      <Card
        onClick={() => onDebateClick?.()}
        role="button"
        tabIndex={0}
        onKeyDown={e => {
          if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault()
            onDebateClick?.()
          }
        }}
      >
        <DebateIconBox>
          <GavelOutlinedIcon />
        </DebateIconBox>
        <CardContent>
          <Typography variant="subtitle1" fontWeight="bold">
            {FAIRBOT_ACTION_CARDS.DEBATE.title}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {FAIRBOT_ACTION_CARDS.DEBATE.description}
          </Typography>
        </CardContent>
      </Card>
    </CardsGrid>
  )
}

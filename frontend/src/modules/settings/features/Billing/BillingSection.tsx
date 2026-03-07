import {
  Box,
  Button,
  Chip,
  LinearProgress,
  Paper,
  Typography,
} from '@mui/material'
import { alpha } from '@mui/material/styles'
import {
  CheckCircle as CheckCircleIcon,
  AutoAwesome as AutoAwesomeIcon,
  ArrowForward as ArrowForwardIcon,
  Circle as CircleIcon,
} from '@mui/icons-material'
import { styled } from '@/styles/styled'

// ─── Styled ─────────────────────────────────────────────────────────────────

const BillingSectionRoot = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(3),
}))

const PlanCard = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(3),
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${theme.palette.divider}`,
}))

const UpgradeCard = styled(Paper)(({ theme }) => ({
  borderRadius: theme.fairworkly.radius.lg,
  border: `1px solid ${alpha(theme.palette.primary.main, 0.25)}`,
  overflow: 'hidden',
  boxShadow: theme.fairworkly.shadow.md,
}))

const GradientBar = styled(Box)(() => ({
  height: 4,
  background: 'linear-gradient(135deg, #6366f1, #ec4899)',
}))

const UpgradeCardBody = styled(Box)(({ theme }) => ({
  padding: theme.spacing(3),
  background: alpha(theme.palette.primary.main, 0.02),
}))

const PlanTitleRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(2),
}))

const PriceRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'baseline',
  gap: theme.spacing(0.75),
  marginBottom: theme.spacing(0.5),
}))

const PriceAmount = styled(Typography)(({ theme }) => ({
  fontSize: 36,
  fontWeight: 800,
  lineHeight: 1,
  color: theme.palette.text.primary,
  letterSpacing: '-0.02em',
}))

const PriceUnit = styled(Typography)(({ theme }) => ({
  ...theme.typography.body2,
  color: theme.palette.text.secondary,
}))

const BillingMeta = styled(Typography)(({ theme }) => ({
  ...theme.typography.caption,
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(2.5),
}))

const Divider = styled(Box)(({ theme }) => ({
  height: 1,
  background: theme.palette.divider,
  margin: theme.spacing(2.5, 0),
}))

const UsageRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: theme.spacing(0.75),
}))

const UsageBar = styled(LinearProgress)(({ theme }) => ({
  height: 6,
  borderRadius: theme.fairworkly.radius.pill,
  backgroundColor: alpha(theme.palette.primary.main, 0.1),
  marginBottom: theme.spacing(2.5),
  '& .MuiLinearProgress-bar': {
    borderRadius: theme.fairworkly.radius.pill,
    background: 'linear-gradient(90deg, #6366f1, #818cf8)',
  },
}))

const FeatureGrid = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '1fr 1fr',
  gap: theme.spacing(1, 2),

  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: '1fr',
  },
}))

const FeatureRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
}))

const SuccessCheckIcon = styled(CheckCircleIcon)(({ theme }) => ({
  fontSize: 16,
  color: theme.palette.success.main,
  flexShrink: 0,
}))

const PrimaryCheckIcon = styled(CheckCircleIcon)(({ theme }) => ({
  fontSize: 16,
  color: theme.palette.primary.main,
  flexShrink: 0,
}))

const ManageBillingRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'flex-end',
  marginTop: theme.spacing(2.5),
  paddingTop: theme.spacing(2),
  borderTop: `1px solid ${theme.palette.divider}`,
}))

const UpgradePriceRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'baseline',
  gap: theme.spacing(0.75),
  marginBottom: theme.spacing(0.5),
}))

const UpgradeFeatureList = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
  marginBottom: theme.spacing(3),
}))

const ActionRow = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(1.5),
}))

const ActiveChip = styled(Chip)(({ theme }) => ({
  height: theme.spacing(2.5),
  background: 'transparent',
  color: theme.palette.success.main,
  padding: 0,
  fontWeight: theme.typography.fontWeightBold,
  fontSize: theme.typography.caption.fontSize,
  letterSpacing: '0.1em',

  '& .MuiChip-icon': {
    color: theme.palette.success.main,
    fontSize: theme.spacing(0.75),
    marginLeft: 0,
    marginRight: theme.spacing(0.75),
    animation: 'pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite',
  },

  '@keyframes pulse': {
    '0%, 100%': { opacity: 1 },
    '50%': { opacity: 0.5 },
  },
}))

const ManageBillingButton = styled(Button)(({ theme }) => ({
  color: theme.palette.text.secondary,
  fontSize: 13,

  '& .MuiButton-endIcon svg': {
    fontSize: 14,
  },
}))

const GradientButton = styled(Button)(({ theme }) => ({
  background: 'linear-gradient(135deg, #6366f1, #ec4899)',
  boxShadow: theme.fairworkly.shadow.primaryButton,
  '&:hover': {
    background: 'linear-gradient(135deg, #4f46e5, #db2777)',
    boxShadow: theme.fairworkly.shadow.primaryButtonHover,
  },
}))

// ─── Data ────────────────────────────────────────────────────────────────────

const CURRENT_FEATURES = [
  'Roster & payroll compliance checking',
  'Team member management',
  '1 Modern Award supported',
  'XLSX & CSV upload',
]

const UPGRADE_FEATURES = [
  'Everything in Lite',
  'Up to 50 employees',
  'Document compliance checking',
  '3 Modern Awards covered',
  'Export reports (Excel / CSV)',
]

const EMPLOYEE_USED = 8
const EMPLOYEE_LIMIT = 20

// ─── Component ───────────────────────────────────────────────────────────────
// TODO(billing): Replace mock data with real API integration
//   - Fetch current plan, price, renewal date from backend (GET /api/billing/subscription)
//   - Fetch employee usage from organization stats
//   - Wire "Upgrade to Starter" button to Stripe checkout session
//   - Wire "Manage Billing" button to Stripe customer portal

export function BillingSection() {
  return (
    <BillingSectionRoot>
      {/* Current plan */}
      <PlanCard>
        <PlanTitleRow>
          <Typography variant="subtitle2" color="text.secondary">
            Current Plan
          </Typography>
          <ActiveChip icon={<CircleIcon />} label="ACTIVE" size="small" />
        </PlanTitleRow>

        <PriceRow>
          <Typography variant="overline" color="text.secondary">
            Lite ·
          </Typography>
          <PriceAmount>$49</PriceAmount>
          <PriceUnit>AUD / month</PriceUnit>
        </PriceRow>
        <BillingMeta>
          Billed monthly · Renews April 11, 2026
        </BillingMeta>

        <Divider />

        <UsageRow>
          <Typography variant="uiLabel" color="text.secondary">
            Employees
          </Typography>
          <Typography variant="uiLabel" color="text.primary">
            {EMPLOYEE_USED} / {EMPLOYEE_LIMIT}
          </Typography>
        </UsageRow>
        <UsageBar
          variant="determinate"
          value={(EMPLOYEE_USED / EMPLOYEE_LIMIT) * 100}
        />

        <FeatureGrid>
          {CURRENT_FEATURES.map(f => (
            <FeatureRow key={f}>
              <SuccessCheckIcon />
              <Typography variant="body2" color="text.secondary">{f}</Typography>
            </FeatureRow>
          ))}
        </FeatureGrid>

        <ManageBillingRow>
          <ManageBillingButton
            variant="text"
            size="small"
            color="inherit"
            endIcon={<ArrowForwardIcon />}
          >
            Manage Billing
          </ManageBillingButton>
        </ManageBillingRow>
      </PlanCard>

      {/* Upgrade card */}
      <UpgradeCard>
        <GradientBar />
        <UpgradeCardBody>
          <PlanTitleRow>
            <Typography variant="subtitle2" color="text.secondary">
              Starter Plan
            </Typography>
            <Chip
              label="Recommended"
              size="small"
              color="primary"
              variant="outlined"
            />
          </PlanTitleRow>

          <UpgradePriceRow>
            <PriceAmount color="primary.main">$149</PriceAmount>
            <PriceUnit>AUD / month</PriceUnit>
          </UpgradePriceRow>
          <BillingMeta>
            More employees, more awards, and document compliance — for growing teams
          </BillingMeta>

          <Divider />

          <UpgradeFeatureList>
            {UPGRADE_FEATURES.map(f => (
              <FeatureRow key={f}>
                <PrimaryCheckIcon />
                <Typography variant="body2" color="text.primary">{f}</Typography>
              </FeatureRow>
            ))}
          </UpgradeFeatureList>

          <ActionRow>
            <GradientButton
              variant="contained"
              color="primary"
              startIcon={<AutoAwesomeIcon />}
            >
              Upgrade to Starter
            </GradientButton>
            <Button variant="outlined" color="primary" component="a" href="/#pricing">
              View all plans
            </Button>
          </ActionRow>
        </UpgradeCardBody>
      </UpgradeCard>
    </BillingSectionRoot>
  )
}

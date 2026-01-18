import {
  Box,
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  Chip,
  styled,
} from '@mui/material';
import { CheckCircle, LocalOfferOutlined } from '@mui/icons-material';

const PricingSectionRoot = styled('section')(({ theme }) => ({
  padding: theme.spacing(12, 0),
  backgroundColor: theme.palette.background.paper,
  borderTop: `1px solid ${theme.palette.divider}`,
}));

const SectionHeader = styled(Box)(({ theme }) => ({
  textAlign: 'center',
  marginBottom: theme.spacing(8),
}));

const SectionLabel = styled(Chip)(({ theme }) => ({
  marginBottom: theme.spacing(2),
  padding: theme.spacing(0.5, 2),
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  textTransform: 'uppercase',
  letterSpacing: theme.typography.caption.letterSpacing,
  backgroundColor: theme.fairworkly.effect.primaryGlow,
  color: theme.palette.primary.main,
  borderRadius: theme.fairworkly.radius.pill,
  '& .MuiChip-icon': {
    color: theme.palette.primary.main,
  },
}));

const SectionTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h2.fontSize,
  fontWeight: theme.typography.h2.fontWeight,
  marginBottom: theme.spacing(2),
  color: theme.palette.text.primary,
  [theme.breakpoints.down('sm')]: {
    fontSize: theme.typography.h3.fontSize,
  },
}));

const SectionSubtitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.secondary,
  maxWidth: '600px',
  margin: '0 auto',
}));

const PricingGrid = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: 'repeat(3, 1fr)',
  gap: theme.spacing(4),
  maxWidth: '1100px',
  margin: '0 auto',
  [theme.breakpoints.down('md')]: {
    gridTemplateColumns: '1fr',
    maxWidth: '400px',
  },
}));

const PricingCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== 'featured',
})<{ featured?: boolean }>(({ theme, featured }) => ({
  padding: theme.spacing(5),
  overflow: 'visible',
  borderRadius: theme.fairworkly.radius.xl,
  border: `2px solid ${featured ? theme.palette.primary.main : theme.palette.divider}`,
  backgroundColor: featured ? theme.palette.background.paper : theme.palette.background.default,
  transition: theme.transitions.create(['all'], {
    duration: theme.transitions.duration.standard,
  }),
  position: 'relative',
  display: 'flex',
  flexDirection: 'column',
  ...(featured && {
    transform: 'scale(1.05)',
    boxShadow: theme.fairworkly.shadow.xl,
  }),
  '&:hover': {
    transform: featured ? 'scale(1.05) translateY(-8px)' : 'translateY(-8px)',
    boxShadow: theme.fairworkly.shadow.xl,
  },
  [theme.breakpoints.down('md')]: {
    transform: 'none !important',
    '&:hover': {
      transform: 'translateY(-4px) !important',
    },
  },
}));

// Featured badge
const FeaturedBadge = styled(Chip)(({ theme }) => ({
  position: 'absolute',
  top: theme.spacing(-1.5),
  left: '50%',
  transform: 'translateX(-50%)',
  background: theme.fairworkly.gradient.primary,
  color: theme.palette.common.white,
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  padding: theme.spacing(0.5, 2),
  borderRadius: theme.fairworkly.radius.pill,
}));

const PlanName = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h4.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  marginBottom: theme.spacing(1),
  color: theme.palette.text.primary,
}));

const PlanDescription = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(3),
}));

const PriceWrapper = styled(Box)(({ theme }) => ({
  marginBottom: theme.spacing(3),
}));

const PriceAmount = styled(Typography)(({ theme }) => ({
  fontSize: '3rem',
  fontWeight: 900,
  display: 'inline',
  color: theme.palette.text.primary,
}));

const PricePeriod = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.disabled,
  display: 'inline',
}));

const FeaturesList = styled(Box)(({ theme }) => ({
  marginBottom: theme.spacing(4),
  flex: 1,
}));

const FeatureItem = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  padding: theme.spacing(1.5, 0),
  borderBottom: `1px solid ${theme.palette.divider}`,
  fontSize: theme.typography.body2.fontSize,
  color: theme.palette.text.secondary,
  '&:last-child': {
    borderBottom: 'none',
  },
}));

const PlanButton = styled(Button, {
  shouldForwardProp: (prop) => prop !== 'featured',
})<{ featured?: boolean }>(({ theme, featured }) => ({
  borderRadius: theme.fairworkly.radius.md,
  padding: theme.spacing(1.5),
  fontWeight: theme.typography.button.fontWeight,
  fontSize: theme.typography.button.fontSize,
  ...(featured && {
    background: theme.fairworkly.gradient.primary,
    '&:hover': {
      background: `linear-gradient(135deg, ${theme.palette.primary.dark}, ${theme.palette.secondary.main})`,
    },
  }),
}));

const PricingNote = styled(Box)(({ theme }) => ({
  textAlign: 'center',
  marginTop: theme.spacing(6),
  paddingTop: theme.spacing(4),
  borderTop: `1px solid ${theme.palette.divider}`,
}));

const NoteText = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  color: theme.palette.text.secondary,
}));

interface Plan {
  name: string;
  description: string;
  price: string;
  features: string[];
  buttonText: string;
  buttonVariant: 'contained' | 'outlined';
  featured?: boolean;
}

export function PricingSection() {

  const plans: Plan[] = [
    {
      name: 'Starter',
      description: 'Perfect for single locations',
      price: '$149',
      features: [
        '1 location (same state)',
        'Up to 50 employees',
        'Unlimited CSV uploads',
        'Payroll + Roster + Document',
        '3 Awards (Hospitality/Retail/Clerks)',
      ],
      buttonText: 'Start Free Trial',
      buttonVariant: 'outlined',
    },
    {
      name: 'Professional',
      description: 'Perfect for multi-location chains',
      price: '$299',
      features: [
        'Everything in Starter, plus:',
        'Up to 10 locations (same state)',
        'Up to 150 employees',
        'Multi-location dashboard',
        'Export reports (Excel/CSV)',
      ],
      buttonText: 'Start Free Trial',
      buttonVariant: 'contained',
      featured: true,
    },
    {
      name: 'Enterprise',
      description: 'For large organizations',
      price: 'Custom',
      features: [
        'Everything in Professional, plus:',
        'Multi-state support (VIC/NSW/QLD...)',
        '10+ locations, 150+ employees',
        'API integrations (Xero/Deputy)',
        'Custom Award development',
      ],
      buttonText: 'Contact Sales',
      buttonVariant: 'outlined',
    },
  ];

  return (
    <PricingSectionRoot id="pricing" aria-labelledby="pricing-heading">
      <Container maxWidth="lg">
        <SectionHeader>
          <SectionLabel icon={<LocalOfferOutlined />} label="Pricing" />
          <SectionTitle variant="h2" component="h2" id="pricing-heading">
            Simple, Transparent Pricing
          </SectionTitle>
          <SectionSubtitle>Choose the plan that fits your business size</SectionSubtitle>
        </SectionHeader>

        <PricingGrid>
          {plans.map((plan, index) => (
            <PricingCard key={index} featured={plan.featured}>
              {plan.featured && <FeaturedBadge label="Most Popular" />}
              <CardContent
                sx={{
                  padding: 0,
                  '&:last-child': { paddingBottom: 0 },
                  flex: 1,
                  display: 'flex',
                  flexDirection: 'column',
                }}
              >
                <PlanName>{plan.name}</PlanName>
                <PlanDescription>{plan.description}</PlanDescription>
                <PriceWrapper>
                  <PriceAmount>{plan.price}</PriceAmount>
                  {plan.price !== 'Custom' && <PricePeriod>/month</PricePeriod>}
                </PriceWrapper>
                <FeaturesList>
                  {plan.features.map((feature, idx) => (
                    <FeatureItem key={idx}>
                      <CheckCircle
                        sx={{ color: 'success.main', fontSize: '1.25rem', flexShrink: 0 }}
                      />
                      <Typography variant="body2">{feature}</Typography>
                    </FeatureItem>
                  ))}
                </FeaturesList>
                <PlanButton
                  variant={plan.buttonVariant}
                  color="primary"
                  size="large"
                  fullWidth
                  featured={plan.featured}
                >
                  {plan.buttonText}
                </PlanButton>
              </CardContent>
            </PricingCard>
          ))}
        </PricingGrid>

        <PricingNote>
          <NoteText>All plans include 14-day free trial • No credit card required</NoteText>
        </PricingNote>
      </Container>
    </PricingSectionRoot>
  );
}
import {
  Box,
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  Grid,
  Chip,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  styled,
} from '@mui/material';
import { LocalOfferOutlined } from '@mui/icons-material';
import { CheckCircleOutline } from '@mui/icons-material';


const Section = styled('section')(({ theme }) => ({
  padding: theme.spacing(12, 0),
  backgroundColor: theme.palette.background.paper,
  borderTop: `1px solid ${theme.palette.divider}`,
}));

const Header = styled(Box)(({ theme }) => ({
  textAlign: 'center',
  marginBottom: theme.spacing(8),
}));

const Label = styled(Chip)(({ theme }) => ({
  marginBottom: theme.spacing(2),
  padding: theme.spacing(0.5, 2),
  fontSize: '0.8125rem',
  fontWeight: 600,
  textTransform: 'uppercase',
  letterSpacing: '0.05em',
  backgroundColor: 'rgba(99, 102, 241, 0.12)',
  color: theme.palette.primary.main,
  '& .MuiChip-icon': {
    color: theme.palette.primary.main,
  },
}));

const Title = styled(Typography)(({ theme }) => ({
  fontSize: '2.5rem',
  fontWeight: 800,
  marginBottom: theme.spacing(2),
  color: theme.palette.text.primary,
  [theme.breakpoints.down('sm')]: {
    fontSize: '2rem',
  },
}));

const Subtitle = styled(Typography)(({ theme }) => ({
  fontSize: '1.125rem',
  color: theme.palette.text.secondary,
  maxWidth: '600px',
  margin: '0 auto',
}));

const PricingGrid = styled(Grid)(({ theme }) => ({
  maxWidth: '1100px',
  margin: '0 auto',
}));

const PricingCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== 'featured',
})<{ featured?: boolean }>(({ theme, featured }) => ({
  padding: theme.spacing(5),
  borderRadius: '24px',
  border: `2px solid ${featured ? theme.palette.primary.main : theme.palette.divider}`,
  backgroundColor: featured ? theme.palette.background.paper : theme.palette.background.default,
  transition: 'all 0.3s ease',
  position: 'relative',
  height: '100%',
  display: 'flex',
  flexDirection: 'column',
  overflow: 'visible',
  ...(featured && {
    transform: 'scale(1.05)',
    boxShadow: theme.shadows[10],
  }),
  '&:hover': {
    transform: featured ? 'scale(1.05) translateY(-8px)' : 'translateY(-8px)',
    boxShadow: theme.shadows[10],
  },
  [theme.breakpoints.down('md')]: {
    transform: 'none !important',
    '&:hover': {
      transform: 'translateY(-4px) !important',
    },
  },
}));

const Badge = styled(Chip)(({ theme }) => ({
  position: 'absolute',
  top: '-12px',
  left: '50%',
  transform: 'translateX(-50%)',
  background: 'linear-gradient(135deg, #6366f1, #ec4899)',
  color: theme.palette.common.white,
  fontSize: '0.75rem',
  fontWeight: 700,
  padding: theme.spacing(0.5, 2),
}));

const PlanName = styled(Typography)(({ theme }) => ({
  fontSize: '1.25rem',
  fontWeight: 700,
  marginBottom: theme.spacing(1),
}));

const PlanDesc = styled(Typography)(({ theme }) => ({
  fontSize: '0.875rem',
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(3),
}));

const PriceAmount = styled(Typography)(({ theme }) => ({
  fontSize: '3rem',
  fontWeight: 900,
  display: 'inline',
}));

const PricePeriod = styled(Typography)(({ theme }) => ({
  fontSize: '1rem',
  color: theme.palette.text.secondary,
  display: 'inline',
}));

const FeatureList = styled(List)(({ theme }) => ({
  marginBottom: theme.spacing(4),
  flex: 1,
}));

const FeatureItem = styled(ListItem)(({ theme }) => ({
  padding: theme.spacing(1.5, 0),
  borderBottom: `1px solid ${theme.palette.divider}`,
  '&:last-child': {
    borderBottom: 'none',
  },
}));

const ActionButton = styled(Button, {
  shouldForwardProp: (prop) => prop !== 'featured',
})<{ featured?: boolean }>(({ theme, featured }) => ({
  borderRadius: theme.spacing(2.5),
  padding: theme.spacing(1.5),
  fontWeight: 600,
  ...(featured && {
    background: 'linear-gradient(135deg, #6366f1, #ec4899)',
    '&:hover': {
      background: 'linear-gradient(135deg, #4f46e5, #db2777)',
    },
  }),
}));

interface Plan {
  name: string;
  description: string;
  price: number;
  features: string[];
  buttonText: string;
  buttonVariant: 'contained' | 'outlined';
  featured?: boolean;
}

export default function PricingSection() {
  const plans: Plan[] = [
    {
      name: 'Starter',
      description: 'For small teams',
      price: 199,
      features: [
        'Up to 50 employees',
        'Compliance + Documents',
        '3 Modern Awards',
        'Email support',
      ],
      buttonText: 'Get Started',
      buttonVariant: 'outlined',
    },
    {
      name: 'Professional',
      description: 'For growing businesses',
      price: 349,
      features: [
        'Up to 100 employees',
        'All 4 AI agents',
        'Payroll integrations',
        '10 Modern Awards',
        'Priority support',
      ],
      buttonText: 'Start Free Trial',
      buttonVariant: 'contained',
      featured: true,
    },
    {
      name: 'Enterprise',
      description: 'For larger orgs',
      price: 499,
      features: [
        'Up to 150 employees',
        'All 4 AI agents',
        'API access',
        'All 122 Awards',
        'Dedicated manager',
      ],
      buttonText: 'Contact Sales',
      buttonVariant: 'outlined',
    },
  ];

  return (
    <Section id="pricing" aria-label="Pricing">
      <Container maxWidth="lg">
        <Header>
          <Label icon={<LocalOfferOutlined />} label="Pricing" />
          <Title variant="h2" component="h2">
            Simple, Transparent Pricing
          </Title>
          <Subtitle>Choose the plan that fits your business size</Subtitle>
        </Header>

        <PricingGrid container spacing={4}>
          {plans.map((plan, index) => (
            <Grid size={{ xs: 12, md: 4 }} key={index}>
              <PricingCard featured={plan.featured}>
                {plan.featured && <Badge label="Most Popular" />}
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
                  <PlanDesc>{plan.description}</PlanDesc>
                  <Box sx={{ mb: 3 }}>
                    <PriceAmount>${plan.price}</PriceAmount>
                    <PricePeriod>/month</PricePeriod>
                  </Box>
                  <FeatureList>
                    {plan.features.map((feature, idx) => (
                      <FeatureItem key={idx} disablePadding>
                        <ListItemIcon sx={{ minWidth: 36 }}>
                          <CheckCircleOutline sx={{ color: 'success.main', fontSize: '1.25rem' }} />
                        </ListItemIcon>
                        <ListItemText
                          primary={feature}
                          primaryTypographyProps={{
                            fontSize: '0.9375rem',
                            color: 'text.secondary',
                          }}
                        />
                      </FeatureItem>
                    ))}
                  </FeatureList>
                  <ActionButton
                    variant={plan.buttonVariant}
                    color="primary"
                    size="large"
                    fullWidth
                    featured={plan.featured}
                  >
                    {plan.buttonText}
                  </ActionButton>
                </CardContent>
              </PricingCard>
            </Grid>
          ))}
        </PricingGrid>
      </Container>
    </Section>
  );
}
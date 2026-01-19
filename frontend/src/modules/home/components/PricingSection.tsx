import { Box, Button, Card, Chip, styled, Typography, alpha, Stack } from "@mui/material";
import { CheckCircleOutline, SellOutlined } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";


const PageSection = styled(Box)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    padding: theme.spacing(12, 0),
    borderTop: `1px solid ${theme.palette.divider}`,
    borderBottom: `1px solid ${theme.palette.divider}`,
}));

const ContentContainer = styled(Box)(({ theme }) => ({
    margin: '0 auto',
    padding: theme.spacing(0, 4),
}));

const HeaderContainer = styled(Box)(({ theme }) => ({
    textAlign: 'center',
    marginBottom: theme.spacing(8),
}));

const SectionLabel = styled(Box)(({ theme }) => ({
    display: 'inline-flex',
    alignItems: 'center',
    gap: theme.spacing(1),
    padding: theme.spacing(0.75, 2),
    backgroundColor: theme.fairworkly.effect.primaryGlow,
    color: theme.palette.primary.main,
    borderRadius: theme.shape.borderRadius,
    marginBottom: theme.spacing(2),
    '& .MuiSvgIcon-root': {
        fontSize: theme.spacing(2),
    }
}));


const SectionTitle = styled(Typography)(({ theme }) => ({
    marginBottom: theme.spacing(2),
}));

const SectionSubTitle = styled(Typography)(({ theme }) => ({
    margin: '0 auto',
    color: theme.palette.text.secondary,
}));



const CardsLayout = styled(Box)(({ theme }) => ({
    display: 'grid',
    gap: theme.spacing(4),
    maxWidth: theme.fairworkly.layout.containerMaxWidth * 0.86,
    margin: '0 auto',
    gridTemplateColumns: `repeat(auto-fit, minmax(${theme.spacing(40)}, 1fr))`,

}));


const CardContainer = styled(Card, {
    shouldForwardProp: (prop) => prop !== 'featured',
})<{ featured?: boolean }>(({ theme, featured }) => ({
    padding: theme.spacing(5),
    borderRadius: theme.spacing(3),
    border: `2px solid ${featured ? theme.palette.primary.main : theme.palette.divider}`,
    backgroundColor: featured ? theme.palette.background.paper : theme.palette.background.default,
    transition: theme.transitions.create(['transform', 'box-shadow'], {
        duration: theme.transitions.duration.standard,
        easing: theme.transitions.easing.easeInOut,
    }),
    position: 'relative',
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'visible',
    ...(featured && {
        transform: 'scale(1.05)',
        boxShadow: theme.fairworkly.shadow.lg,
    }),
    '&:hover': {
        transform: featured
            ? `scale(1.05) translateY(${theme.spacing(-1)})`
            : `translateY(${theme.spacing(-1)})`,
        boxShadow: theme.fairworkly.shadow.lg,
    },
}));

const FeaturedBadge = styled(Chip)(({ theme }) => ({
    position: 'absolute',
    top: theme.spacing(-1.5),
    left: '50%',
    transform: 'translateX(-50%)',
    background: theme.fairworkly.gradient.primary,
    padding: theme.spacing(0.75, 2),
    color: theme.palette.common.white,
    fontWeight: 700,
    fontSize: theme.typography.caption.fontSize,
    height: theme.spacing(3),
    zIndex: 1,
}));

const CardTitle = styled(Typography)(({ theme }) => ({
    marginBottom: theme.spacing(1),
}));

const CardDescription = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
    marginBottom: theme.spacing(3),
}));

const PriceContainer = styled(Box)(({ theme }) => ({
    marginBottom: theme.spacing(3),
}));

const PriceAmount = styled(Typography)(({ theme }) => ({
    display: 'inline',
    fontSize: theme.typography.h2.fontSize,
    fontWeight: 900,
}));

const PricePeriod = styled(Typography)(({ theme }) => ({
    display: 'inline',
    fontSize: theme.typography.body1.fontSize,
    color: theme.palette.text.disabled,
    marginLeft: theme.spacing(0.5),
}));


const FeaturesList = styled(Stack)(({ theme }) => ({
    marginBottom: theme.spacing(4),
    flexGrow: 1,
}));

const FeatureItem = styled(Typography)(({ theme }) => ({
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

const FeatureCheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
    fontSize: theme.spacing(2.5),
    color: theme.palette.success.main,
    flexShrink: 0,
}));


const ActionButton = styled(Button, {
    shouldForwardProp: (prop) => prop !== 'featured',
})<{ featured?: boolean }>(({ theme, featured }) => ({
    borderRadius: theme.fairworkly.radius.sm,
    padding: theme.spacing(1.5),
    fontWeight: 600,
    background: featured ? theme.fairworkly.gradient.primary : theme.palette.background.paper,
    boxShadow: featured ? theme.fairworkly.shadow.primaryButton : 'none',
    '&:hover': {
        background: featured
            ? `linear-gradient(135deg, ${theme.palette.primary.dark}, ${theme.palette.secondary.main})`
            : `linear-gradient(${theme.fairworkly.effect.primaryGlow})`,
        transform: `translateY(${theme.spacing(-0.25)})`,
        boxShadow: featured ? theme.fairworkly.shadow.primaryButtonHover : 'none',
    },
}));



const BottomNoteContainer = styled(Box)(({ theme }) => ({
    textAlign: 'center',
    marginTop: theme.spacing(6),
    paddingTop: theme.spacing(4),
    borderTop: `1px solid ${theme.palette.divider}`,
}));

const BottomNote = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
}));


interface PricingPlan {
    id: string;
    name: string;
    description: string;
    price?: string;
    period?: string;
    features: string[];
    featured?: boolean;
    buttonText: string;
    buttonVariant: 'contained' | 'outlined';
    buttonAction: 'signup' | 'contact';

}



const PricingCard = ({ plan, onButtonClick }: { plan: PricingPlan; onButtonClick: (action: string) => void }) => {
    const { name, description, price, period, features, featured, buttonText, buttonVariant, buttonAction } = plan;


    return (
        <CardContainer elevation={0} featured={featured}>
            {featured && <FeaturedBadge label="Most Popular" />}

            <CardTitle variant="h4">{name}</CardTitle>
            <CardDescription variant="body2">{description}</CardDescription>

            <PriceContainer>
                <PriceAmount>{price || 'Custom'}</PriceAmount>
                {period && <PricePeriod>/{period}</PricePeriod>}
            </PriceContainer>

            <FeaturesList>
                {features.map((feature) => (
                    <FeatureItem key={feature}>
                        <FeatureCheckIcon aria-hidden="true" />
                        {feature}
                    </FeatureItem>
                ))}
            </FeaturesList>

            <ActionButton
                variant={buttonVariant}
                color="primary"
                fullWidth
                size="large"
                featured={featured}
                onClick={() => onButtonClick(buttonAction)}
            >
                {buttonText}
            </ActionButton>
        </CardContainer>
    );
};



export const PricingSection: React.FC = () => {
    const navigate = useNavigate();

    const handleClick = (action: string) => {
        switch (action) {
            case 'signup':
                navigate('/login?signup=true');
                break;
            case 'contact':
                window.location.href = 'mailto:support@fairworkly.com?subject=Enterprise Inquiry';
                break;
            default:
                console.warn('Unknown CTA action:', action);
        }
    };

    const content = {
        label: 'PRICING',
        title: 'Simple, Transparent Pricing',
        subtitle: 'Choose the plan that fits your business size',
        bottomNote: 'All plans include 14-day free trial • No credit card required',
    };


    const plans: PricingPlan[] = [
        {
            id: 'starter',
            name: 'Starter',
            description: 'Perfect for single locations',
            price: '$149',
            period: 'month',
            features: [
                '1 location (same state)',
                'Up to 50 employees',
                'Unlimited CSV uploads',
                'Payroll + Roster + Document',
                '3 Awards (Hospitality/Retail/Clerks)',
            ],
            buttonText: 'Start Free Trial',
            buttonVariant: 'outlined',
            buttonAction: 'signup',
        },
        {
            id: 'professional',
            name: 'Professional',
            description: 'Perfect for multi-location chains',
            price: '$299',
            period: 'month',
            features: [
                'Everything in Starter, plus:',
                'Up to 10 locations (same state)',
                'Up to 150 employees',
                'Multi-location dashboard',
                'Export reports (Excel/CSV)',
            ],
            featured: true,
            buttonText: 'Start Free Trial',
            buttonVariant: 'contained',
            buttonAction: 'signup',
        },
        {
            id: 'enterprise',
            name: 'Enterprise',
            description: 'For large organizations',
            features: [
                'Everything in Professional, plus:',
                'Multi-state support (VIC/NSW/QLD...)',
                '10+ locations, 150+ employees',
                'API integrations (Xero/Deputy)',
                'Custom Award development',
            ],
            buttonText: 'Contact Sales',
            buttonVariant: 'outlined',
            buttonAction: 'contact',
        },
    ];

    return (
        <PageSection id="pricing">
            <ContentContainer>
                <HeaderContainer>
                    <SectionLabel>
                        <SellOutlined fontSize="inherit" />
                        {content.label}
                    </SectionLabel>
                    <SectionTitle variant="h2">{content.title}</SectionTitle>
                    <SectionSubTitle variant="h5">{content.subtitle}</SectionSubTitle>
                </HeaderContainer>

                <CardsLayout>
                    {plans.map((plan) => (
                        <PricingCard
                            key={plan.id}
                            plan={plan}
                            onButtonClick={handleClick}
                        />
                    ))}
                </CardsLayout>

                <BottomNoteContainer>
                    <BottomNote variant="body1">{content.bottomNote}</BottomNote>
                </BottomNoteContainer>
            </ContentContainer>
        </PageSection>
    );
};
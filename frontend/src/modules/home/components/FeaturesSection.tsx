import { AutoAwesomeOutlined, CheckCircleOutline, DescriptionOutlined, EventOutlined, PaymentsOutlined } from "@mui/icons-material";
import { alpha, Box, Card, styled, Typography, type SvgIconProps, type TypographyProps } from "@mui/material";


type Tone = 'primary' | 'warning' | 'info';

const PageSection = styled('section')(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    padding: theme.spacing(12, 0),
    borderTop: `1px solid ${theme.palette.divider}`,
    borderBottom: `1px solid ${theme.palette.divider}`,
}));

const ContentContainer = styled(Box)(({ theme }) => ({
    maxWidth: theme.fairworkly.layout.containerMaxWidth,
    margin: '0 auto',
    padding: theme.spacing(0, 4),
}));

const HeaderContainer = styled('header')(({ theme }) => ({
    textAlign: 'center',
    marginBottom: theme.spacing(8),
}));

const SectionEyebrow = styled(Typography)(({ theme }) => ({
    display: 'inline-flex',
    alignItems: 'center',
    gap: theme.spacing(1),
    padding: theme.spacing(0.75, 2),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    color: theme.palette.primary.main,
    borderRadius: theme.shape.borderRadius,
    marginBottom: theme.spacing(2),
}));

const LabelIcon = styled(Box)({
    display: 'inline-flex',
    alignItems: 'center',
    color: 'inherit',
});

const TitleContainer = styled("header")(({ theme }) => ({
    marginBottom: theme.spacing(8),
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
    gridTemplateColumns: '1fr',
    gap: theme.spacing(3),
    [theme.breakpoints.up('sm')]: {
        gridTemplateColumns: 'repeat(2, 1fr)',
    },
    [theme.breakpoints.up('md')]: {
        gridTemplateColumns: 'repeat(3, 1fr)',
    }
}));

const CardContainer = styled(Card)(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(5, 4),
    border: `1px solid ${theme.palette.divider}`,
    borderRadius: theme.shape.borderRadius,
    transition: 'all 0.3s ease',
    minHeight: theme.spacing(56.25),

    '&:hover': {
        boxShadow: theme.shadows[3],
        borderColor: theme.palette.primary.main,
        transform: `translateY(${theme.spacing(-0.5)})`,
    },
}));

const CardHeader = styled(Box)(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    textAlign: 'center',
    gap: theme.spacing(2),
    marginBottom: theme.spacing(2.5),
}));

const CardIconContainer = styled(Box)<{ tone: Tone }>(({ theme, tone }) => ({
    width: theme.spacing(7.5),
    height: theme.spacing(7.5),
    borderRadius: theme.spacing(2),
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0,

    backgroundColor: alpha(theme.palette[tone].main, 0.1),
    color: theme.palette[tone].main,

    '& svg': {
        fontSize: theme.spacing(3.75),
    },
}));

const CardTitle = styled(Typography)({
    whiteSpace: 'nowrap'
});

const CardDescription = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
    flexGrow: 1,
    textAlign: 'center',
    marginBottom: theme.spacing(3),
}));

const CardFeaturesLayout = styled('ul')(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    listStyle: 'none',
    gap: theme.spacing(1.5),
    padding: 0,
}));

const CardFeatureItem = styled(Typography)<TypographyProps>(({ theme }) => ({
    display: 'flex',
    alignItems: 'flex-start',
    gap: theme.spacing(1.25),
    color: theme.palette.text.secondary,
}));

const CardCheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
    color: theme.palette.success.main,
    fontSize: theme.spacing(2.25),
    flexShrink: 0,
    marginTop: theme.spacing(0.25),
}));



interface FeatureCardData {
    id: string;
    tone: Tone;
    icon: React.ComponentType<SvgIconProps>;
    title: string;
    description: string;
    features: string[];
};

const FeatureCard = ({ data }: { data: FeatureCardData }) => {
    const { tone, icon: IconComponent, title, description, features } = data;

    return (
        <CardContainer elevation={0}>
            <CardHeader>
                <CardIconContainer tone={tone} aria-hidden='true'>
                    <IconComponent />
                </CardIconContainer>
                <CardTitle variant='h4'>{title}</CardTitle>
            </CardHeader>

            <CardDescription variant='body2'>{description}</CardDescription>

            <CardFeaturesLayout>
                {features.map((feature) => (
                    <CardFeatureItem key={feature} variant='body2' component='li'>
                        <CardCheckIcon aria-hidden='true' />
                        {feature}
                    </CardFeatureItem>
                ))}
            </CardFeaturesLayout>
        </CardContainer>
    )
};

export const FeaturesSection: React.FC = () => {
    const content = {
        label: 'FEATURES',
        title: 'Three Compliance Modules, One Platform',
        subtitle: 'Complete Fair Work validation for payroll, rosters, and documents'
    };
    const cards: FeatureCardData[] = [
        {
            id: 'roster-compliance',
            tone: 'primary',
            icon: EventOutlined,
            title: 'Roster Compliance',
            description: 'Upload roster CSV (from Deputy, Excel, or any system) to check scheduling violations.',
            features: ['Consecutive working days (max 6)', 'Rest between shifts (min 10h)', 'Weekly hour limits', 'Break requirements'],
        },
        {
            id: 'payroll-compliance',
            tone: 'warning',
            icon: PaymentsOutlined,
            title: 'Payroll Compliance',
            description: 'Upload payslip CSV and instantly validate against Award requirements.',
            features: ['Base rates vs Award minimum', 'Penalty rates (Sat/Sun/PH)', 'Casual loading (25%)', 'Superannuation compliance'],
        },
        {
            id: 'document-compliance',
            tone: 'info',
            icon: DescriptionOutlined,
            title: 'Document Compliance',
            description: 'Track mandatory employment documents and avoid missing critical deadlines.',
            features: ['Fair Work Info Statement', 'Separation Certificate (14-day)', 'Casual Conversion Notice', 'Deadline reminders'],
        },

    ];
    return (
        <PageSection>
            <ContentContainer>
                <HeaderContainer>
                    <SectionEyebrow variant='overline'>
                        <LabelIcon aria-hidden='true'>
                            <AutoAwesomeOutlined fontSize='inherit' />
                        </LabelIcon>
                        {content.label}
                    </SectionEyebrow>
                    <TitleContainer>
                        <SectionTitle variant='h2'>{content.title}</SectionTitle>
                        <SectionSubTitle variant='h5'>{content.subtitle}</SectionSubTitle>
                    </TitleContainer>
                </HeaderContainer>
                <CardsLayout>
                    {cards.map((card) => (
                        <FeatureCard key={card.id} data={card} />
                    ))}
                </CardsLayout>
            </ContentContainer>
        </PageSection>
    )
};
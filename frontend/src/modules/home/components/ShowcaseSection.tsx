import { Box, Card, Stack, Typography, alpha, styled, type SvgIconProps } from "@mui/material";
import { CheckCircleOutline, StoreOutlined, WarningOutlined } from "@mui/icons-material";

type Tone = 'primary' | 'warning' | 'success';

const PageSection = styled(Box)(({ theme }) => ({
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(12, 0),
    borderTop: `1px solid ${theme.palette.divider}`,
    borderBottom: `1px solid ${theme.palette.divider}`,
}));

const ContentContainer = styled(Box)(({ theme }) => ({
    maxWidth: theme.fairworkly.layout.containerMaxWidth,
    margin: '0 auto',
    padding: theme.spacing(0, 4),
}));

const ContentLayout = styled(Box)(({ theme }) => ({
    display: 'grid',
    gridTemplateColumns: '1fr',
    gap: theme.spacing(8),
    alignItems: 'center',

    [theme.breakpoints.up("md")]: {
        gridTemplateColumns: "1fr 1fr",
    },
}));

const LeftDashboardContainer = styled(Box)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    borderRadius: theme.shape.borderRadius,
    padding: theme.spacing(3),
    boxShadow: theme.shadows[4],
    border: `1px solid ${theme.palette.divider}`,
}));

const DashboardHeader = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.disabled,
    textTransform: 'uppercase',
    letterSpacing: theme.spacing(0.1),
    marginBottom: theme.spacing(3),
    textAlign: 'center',
}));

const DashboardCardsLayout = styled(Box)(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    gap: theme.spacing(2),
    marginBottom: theme.spacing(2)
}));

const CardContainer = styled(Card)<{ tone: Tone }>(({ theme, tone }) => ({
    display: 'flex',
    alignItems: 'center',
    gap: theme.spacing(2),
    borderRadius: theme.shape.borderRadius,
    border: `1px solid ${theme.palette[tone].main}`,
    padding: theme.spacing(2.5),
    backgroundColor: theme.palette.background.default,
    transition: theme.transitions.create(['transform'], {
        duration: theme.transitions.duration.short,
        easing: theme.transitions.easing.easeInOut,
    }),

    '&:hover': {
        transform: `translateX(${theme.spacing(0.5)})`,
    },
}));

const CardIconContainer = styled(Box)(({ theme }) => ({
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0,
    width: theme.spacing(6),
    height: theme.spacing(6),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    borderRadius: theme.shape.borderRadius,

    '& svg': {
        fontSize: theme.spacing(3),
        color: theme.palette.primary.main,
    },
}));

const CardTextContainer = styled(Box)({
    flexGrow: 1,
});

const CardTitle = styled(Typography)(({ theme }) => ({
    marginBottom: theme.spacing(0.5),
}));

const CardDescription = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.disabled,
}));

const CardBadgeContainer = styled(Typography)<{ tone: Tone }>(({ theme, tone }) => ({
    display: 'flex',
    alignItems: 'center',
    gap: theme.spacing(0.75),
    padding: theme.spacing(0.75, 1.5),
    borderRadius: theme.fairworkly.radius.sm,
    whiteSpace: 'nowrap',
    backgroundColor: alpha(theme.palette[tone].main, 0.1),
    color: theme.palette[tone].main,

    '& svg': {
        fontSize: theme.spacing(2),
        verticalAlign: 'middle',
    },
}));


const DashboardBottomNote = styled(Typography)(({ theme }) => ({
    textAlign: 'center',
    color: theme.palette.text.disabled,
    fontStyle: 'italic',
}));

const RightInfoContainer = styled(Box)(({ theme }) => ({
    color: theme.palette.text.primary,
}));

const SectionTitle = styled(Typography)(({ theme }) => ({
    marginBottom: theme.spacing(3),
}));

const SectionSubTitle = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
    marginBottom: theme.spacing(3),
}));

const ChainList = styled(Stack)(({ theme }) => ({
    marginBottom: theme.spacing(3),
}));


const ChainItem = styled(Typography)(({ theme }) => ({
    display: 'flex',
    alignItems: 'flex-start',
    gap: theme.spacing(1.5),
    padding: theme.spacing(1.5, 0),
}));

const ChainText = styled(Box)({
    display: 'flex',
    flexDirection: 'column',
});

const ChainTitle = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.primary,
    fontWeight: 'bolder',
}));

const ChainDescription = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.primary,
}));

const ChainCheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
    color: theme.palette.success.main,
    fontSize: 'inherit',
    flexShrink: 0,
    marginTop: theme.spacing(0.25),
}));

const AwardsContainer = styled(Box)(({ theme }) => ({
    color: theme.palette.text.secondary,
    padding: theme.spacing(2),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    borderRadius: theme.shape.borderRadius,
    borderLeft: `3px solid ${theme.palette.primary.main}`,
    marginBottom: theme.spacing(3),
}));

const AwardsTitle = styled(Typography)(({ theme }) => ({
    display: 'inline',
    color: theme.palette.primary.main,
    fontWeight: 'bolder',
}));


interface DashboardCardData {
    id: string;
    tone: Tone,
    title: string,
    description: string,
    badgeIcon: React.ComponentType<SvgIconProps>,
    badgeInfo: string,
};

const DashboardCard = ({ data }: { data: DashboardCardData }) => {
    const { tone, badgeIcon: IconComponent, title, description, badgeInfo } = data;

    return (
        <CardContainer elevation={0} tone={tone}>
            <CardIconContainer aria-hidden='true'>
                <StoreOutlined />
            </CardIconContainer>
            <CardTextContainer>
                <CardTitle variant='body1'>{title}</CardTitle>
                <CardDescription variant='body2'>{description}</CardDescription>
            </CardTextContainer>
            <CardBadgeContainer variant='subtitle2' tone={tone}>
                <IconComponent />
                {badgeInfo}
            </CardBadgeContainer>
        </CardContainer>
    )
};

export const ShowcaseSection: React.FC = () => {
    const content = {
        dashboardHeader: 'MULTI-LOCATION COMPLIANCE DASHBOARD',
        dashboardNote: 'View all locations from one dashboard',
        title: 'Perfect for Growing Australian Chains',
        subtitle: 'Whether you\'re managing 3 cafés across Melbourne, 5 retail stores in Sydney, or 8 hospitality venues nationwide — FairWorkly scales with your business.',
        awards: 'Supported Awards: ',
        awardsList: 'Hospitality • Retail • Clerks',

    };
    const cards: DashboardCardData[] = [
        {
            id: 'melbourne',
            tone: 'success',
            title: 'Melbourne CBD',
            description: '42 employees',
            badgeIcon: CheckCircleOutline,
            badgeInfo: 'Compliant'
        },
        {
            id: 'sydney',
            tone: 'success',
            title: 'Sydney Harbour',
            description: '38 employees',
            badgeIcon: CheckCircleOutline,
            badgeInfo: 'Compliant'
        },
        {
            id: 'brisbane',
            tone: 'warning',
            title: 'Brisbane South',
            description: '35 employees',
            badgeIcon: WarningOutlined,
            badgeInfo: '2 Issues'
        },
    ];

    const chains = [
        {
            id: 'café',
            title: 'Café Chains (3-10 stores)',
            description: 'Track compliance across all locations from one dashboard'
        },
        {
            id: 'retail',
            title: 'Retail Chains (5-15 stores)',
            description: 'Handle different penalty rates for weekends and public holidays'
        },
        {
            id: 'hospitality',
            title: 'Hospitality Groups',
            description: 'Manage split shifts, casual conversion, document deadlines'
        },

    ];

    return (
        <PageSection>
            <ContentContainer>
                <ContentLayout>
                    <LeftDashboardContainer>
                        <DashboardHeader variant='subtitle2'>{content.dashboardHeader}</DashboardHeader>
                        <DashboardCardsLayout>
                            {cards.map((card) => (
                                <DashboardCard key={card.id} data={card} />
                            ))}
                        </DashboardCardsLayout>
                        <DashboardBottomNote variant='body2'>{content.dashboardNote}</DashboardBottomNote>
                    </LeftDashboardContainer>
                    <RightInfoContainer>
                        <SectionTitle variant='h3'>{content.title}</SectionTitle>
                        <SectionSubTitle variant='body1'>{content.subtitle}</SectionSubTitle>
                        <ChainList>
                            {chains.map((chain) => (
                                <ChainItem key={chain.id}>
                                    <ChainCheckIcon aria-hidden='true' />
                                    <ChainText>
                                        <ChainTitle>{chain.title}</ChainTitle>
                                        <ChainDescription>{chain.description}</ChainDescription>
                                    </ChainText>
                                </ChainItem>
                            ))}
                        </ChainList>
                        <AwardsContainer>
                            <AwardsTitle>{content.awards}</AwardsTitle>{content.awardsList}
                        </AwardsContainer>
                    </RightInfoContainer>
                </ContentLayout>
            </ContentContainer>
        </PageSection>
    )
};
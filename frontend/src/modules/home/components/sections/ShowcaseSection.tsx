import type { Theme } from "@mui/material/styles";
import { Box, Card, Typography, alpha, styled, type SvgIconProps } from "@mui/material";
import { CheckCircleOutline, StoreOutlined, WarningOutlined } from "@mui/icons-material";


type PaletteKey = keyof Theme['palette'];
type Tone = Extract<
    PaletteKey,
    'primary' | 'success' | 'warning'    //取出palette定义好的样式
>;

const PageSection = styled('section')(({ theme }) => ({
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(12, 0),
    borderTop: `1px solid ${theme.palette.divider}`,
    borderBottom: `1px solid ${theme.palette.divider}`,
}));

const ContentContainer = styled(Box)(({ theme }) => ({
    maxWidth: 1280,          //hardcode: theme里没有匹配参数
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
    borderRadius: theme.shape.borderRadius,      //样式为24px，theme里只有20px
    padding: theme.spacing(3),
    boxShadow: theme.shadows[4],
    border: `1px solid ${theme.palette.divider}`,
}))

const DashboardHeader = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.disabled,
    textTransform: 'uppercase',
    letterSpacing: theme.spacing(0.1),
    marginBottom: theme.spacing(3),
    textAlign: 'center',
    fontSize: '0.875rem',     //hardcode:没有匹配样式
    fontWeight: '600',
}))

const DashboardCardsLayout = styled(Box)(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    gap: theme.spacing(2),
    marginBottom: theme.spacing(2)
}))

const CardContainer = styled(Card)<{ tone: Tone }>(({ theme, tone }) => ({
    display: 'flex',
    alignItems: 'center',
    gap: theme.spacing(2),
    borderRadius: theme.shape.borderRadius,       //样式为16px，theme里只有20px
    border: `1px solid ${theme.palette[tone].main}`,
    padding: theme.spacing(2.5),
    backgroundColor: theme.palette.background.default,
    transition: 'all 0.3 ease',

    '&:hover': {
        transform: 'translateX(4px)',      //hardcode 
    }
}));

const CardIconContainer = styled(Box)(({ theme }) => ({
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0,
    width: theme.spacing(6),
    height: theme.spacing(6),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    borderRadius: '12px',   //hardcode

    '& svg': {
        fontSize: '1.5rem',
        color: theme.palette.primary.main,
    },
}));

const CardTextContainer = styled(Box)({
    flexGrow: '1',     //hardcode
});

const CardTitle = styled(Typography)(({ theme }) => ({
    //button
    marginBottom: theme.spacing(0.5),
}))

const CardDescription = styled(Typography)(({ theme }) => ({
    fontSize: '0.8125rem',  //hardcode
    color: theme.palette.text.disabled,
}))

const CardBadgeContainer = styled(Box)<{ tone: Tone }>(({ theme, tone }) => ({
    display: 'flex',
    alignItems: 'center',
    gap: theme.spacing(0.75),
    padding: theme.spacing(0.75, 1.5),
    borderRadius: '8px',   //hardcode
    fontSize: '0.8125rem',     //hardcode: theme里没有匹配样式
    fontWeight: '600',
    whiteSpace: 'nowrap',
    backgroundColor: alpha(theme.palette[tone].main, 0.1),
    color: theme.palette[tone].main,

    '& svg': {
        fontSize: '1rem',
        verticalAlign: 'middle',
    },
}))


const DashboardBottomNote = styled(Typography)(({ theme }) => ({
    textAlign: 'center',
    fontSize: '0.8125rem',   //hardcode
    color: theme.palette.text.disabled,
    fontStyle: 'italic',
}))

const RightInfoContainer = styled(Box)(({ theme }) => ({
    color: theme.palette.text.primary,
    lineHeight: '1.6',    //hardcode
}))

const SectionTitle = styled(Typography)(({ theme }) => ({
    fontSize: '1.875rem',
    fontWeight: '800',
    lineHeight: '1.2',
    marginBottom: theme.spacing(3),
}))

const SectionSubTitle = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
    marginBottom: theme.spacing(3),
    lineHeight: '1.7',    //hardcode
}))

const ChainList = styled('ul')(({ theme }) => ({
    listStyle: 'none',
    marginBottom: theme.spacing(3),
    padding: 0,
}))


const ChainItem = styled('li')(({ theme }) => ({
    display: 'flex',
    alignItems: 'flex-start',
    gap: theme.spacing(1.5),
    padding: theme.spacing(1.5, 0),
}))

const ChainText = styled(Box)({
    display: 'flex',
    flexDirection: 'column',
})

const ChainTitle = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.primary,
    fontWeight: 'bolder',
}))

const ChainDescription = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.primary,
}))

const ChainCheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
    color: theme.palette.success.main,
    fontSize: 'inherit',
    flexShrink: 0,
    marginTop: theme.spacing(0.25),
}));

const AwardsContainer = styled(Box)(({ theme }) => ({
    //button
    color: theme.palette.text.secondary,
    padding: theme.spacing(2),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    borderRadius: '12px',     //hardcode
    borderLeft: `3px solid ${theme.palette.primary.main}`,
    marginBottom: theme.spacing(3),
}))

const AwardsTitle = styled(Typography)(({ theme }) => ({
    display: 'inline',
    color: theme.palette.primary.main,
    fontWeight: 'bolder',
}))


interface DashboardCardData {
    id: string;
    tone: Tone,
    title: string,
    description: string,
    badgeIcon: React.ComponentType<SvgIconProps>,
    badgeInfo: string,
}

const DashbordCard = ({ data }: { data: DashboardCardData }) => {
    const { tone, badgeIcon: IconComponent, title, description, badgeInfo } = data;

    return (
        <CardContainer elevation={0} tone={tone}>
            <CardIconContainer aria-hidden='true'>
                <StoreOutlined />
            </CardIconContainer>
            <CardTextContainer>
                <CardTitle variant='button'>{title}</CardTitle>
                <CardDescription>{description}</CardDescription>
            </CardTextContainer>
            <CardBadgeContainer tone={tone}>
                <IconComponent />
                {badgeInfo}
            </CardBadgeContainer>
        </CardContainer>
    )
}

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
                        <DashboardHeader>{content.dashboardHeader}</DashboardHeader>
                        <DashboardCardsLayout>
                            {cards.map((card) => (
                                <DashbordCard key={card.id} data={card} />
                            ))}
                        </DashboardCardsLayout>
                        <DashboardBottomNote>{content.dashboardNote}</DashboardBottomNote>
                    </LeftDashboardContainer>
                    <RightInfoContainer>
                        <SectionTitle>{content.title}</SectionTitle>
                        <SectionSubTitle>{content.subtitle}</SectionSubTitle>
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



}
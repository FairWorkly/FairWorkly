import React from "react";
import Typography, { type TypographyProps } from "@mui/material/Typography";
import { alpha, styled } from "@mui/material/styles";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import {
    PaymentsOutlined,
    DescriptionOutlined,
    SentimentDissatisfied,
    Gavel,
} from "@mui/icons-material";
import type { SvgIconProps } from "@mui/material/SvgIcon";
import type { Theme } from "@mui/material/styles";
import { Card, type CardProps } from "@mui/material";


type PaletteKey = keyof Theme["palette"];
type Tone = Extract<
    PaletteKey,
    "primary" | "error" | "warning" | "info"
>;


const Section = styled("section")(({ theme }) => ({
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(12, 0),
}));

const ContentWrapper = styled("div")(({ theme }) => ({
    maxWidth: 1280,
    margin: "0 auto",
    padding: theme.spacing(0, 4),
}));

const SectionHeader = styled("header")(({ theme }) => ({
    textAlign: "center",
    marginBottom: theme.spacing(8),
}));

const SectionLabel = styled("div")(({ theme }) => ({
    display: "inline-flex",
    alignItems: "center",
    gap: theme.spacing(1),
    padding: theme.spacing(0.75, 2),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    color: theme.palette.primary.main,
    borderRadius: theme.shape.borderRadius,
    fontSize: "0.8125rem",
    fontWeight: 600,
    textTransform: "uppercase",
    letterSpacing: "0.5px",
    marginBottom: theme.spacing(2),
}));

const LabelIcon = styled("div")({
    fontSize: "inherit",
    lineHeight: 0.2,
    verticalAlign: "middle",
});

const HeaderContainer = styled("header")(({ theme }) => ({
    marginTop: theme.spacing(3),
}));

const MainHeading = styled(Typography)<TypographyProps>(({ theme }) => ({
    marginBottom: theme.spacing(2),
}));

const SubHeading = styled(Typography)<TypographyProps>(({ theme }) => ({
    margin: "0 auto",
    color: theme.palette.text.secondary,
}));


const CardsGrid = styled("div")(({ theme }) => ({
    display: "grid",
    gap: theme.spacing(3),
    [theme.breakpoints.up("sm")]: {
        gridTemplateColumns: "repeat(2,  1fr)",
    },
    [theme.breakpoints.up("md")]: {
        gridTemplateColumns: "repeat(4,  1fr)",
    },
}));


const ProblemCardContainer = styled(Card)<CardProps>(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    border: `1px solid ${theme.palette.divider}`,
    padding: theme.spacing(4),
    transition: "all 0.3s ease",
    textAlign: "center",

    "&:hover": {
        transform: "translateY(-8px)",
        boxShadow: theme.shadows[4],
    },
}));

const CardIconContainer = styled("div")<{ tone: Tone }>(({ theme, tone }) => ({
    width: theme.spacing(7),
    height: theme.spacing(7),
    borderRadius: theme.spacing(1.75),
    backgroundColor: alpha(theme.palette[tone].main, 0.1),
    color: theme.palette[tone].main,

    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    margin: `0 auto ${theme.spacing(2)}`,

    "& svg": {
        fontSize: "1.75rem",
      },
}));

const CardValueTitle = styled(Typography)<TypographyProps & { tone: Tone }>(({ theme, tone }) => ({
    color: theme.palette[tone].main,
    marginBottom: theme.spacing(1),
}));

const CardTextTitle = styled(Typography)<TypographyProps>(({ theme }) => ({
    fontweight: 600,
    marginBottom: theme.spacing(1),
}));

const CardDescriptionText = styled(Typography)<TypographyProps>(({ theme }) => ({
    color: theme.palette.text.secondary,
}));


interface CardData {
    id: string;
    icon: React.ComponentType<SvgIconProps>;
    value: string;
    label: string;
    description: string;
    tone: Tone;
}

function ProblemCard({ data }: { data: CardData }) {
    const {tone, icon: IconComponent, label, value, description} = data;
    return (
        <ProblemCardContainer component="article">
            <CardIconContainer tone={tone} aria-hidden:true>
                <IconComponent />
            </CardIconContainer>

            <CardValueTitle variant="h2" component="p" tone={tone}>{value}</CardValueTitle>
            <CardTextTitle variant="body1" component="h3">{label}</CardTextTitle>
            <CardDescriptionText variant="body2" component="p">{description}</CardDescriptionText>
        </ProblemCardContainer>
    );
}



export const ProblemSection: React.FC = () => {

    const content = {
        label: "THE PROBLEM",
        title: "Australian SMEs Face Real Compliance Risks",
        subtitle:
            "Complex award systems and changing regulations make compliance a nightmare",
    };

    const cards: CardData[] = [
        {
            id: "wage-underpayments",
            icon: PaymentsOutlined,
            value: "$1.35B",
            label: "Wage Underpayments",
            description: "Recovered by Fair Work in 2023-24",
            tone: "error",
        },
        {
            id: "modern-awards",
            icon: DescriptionOutlined,
            value: "122",
            label: "Modern Awards",
            description: "Complex rules and variations",
            tone: "warning",
        },
        {
            id: "smes-not-confident",
            icon: SentimentDissatisfied,
            value: "73%",
            label: "SMEs Not Confident",
            description: "Too complex to DIY",
            tone: "primary",
        },
        {
            id: "fwo-recovery",
            icon: Gavel,
            value: "$532M",
            label: "FWO Recovery",
            description: "Enforcement intensifying",
            tone: "info",
        },
    ];

    return (
        <Section>
            <ContentWrapper>
                <SectionHeader>
                    <SectionLabel>
                        <LabelIcon aria-hidden>
                            <WarningAmberIcon fontSize="inherit" />
                        </LabelIcon>
                        {content.label}
                    </SectionLabel>

                    <HeaderContainer>
                        <MainHeading variant="h2" component="h2">{content.title}</MainHeading>
                        <SubHeading variant="body1" component="p">{content.subtitle}</SubHeading>
                    </HeaderContainer>
                </SectionHeader>

                <CardsGrid>
                    {cards.map((card) => (
                        <ProblemCard key={card.id} data={card} />
                    ))}
                </CardsGrid>

            </ContentWrapper>
        </Section>
    );
};

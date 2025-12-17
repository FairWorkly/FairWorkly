import Box, { type BoxProps } from "@mui/material/Box";
import { styled } from "@mui/material/styles";
import Typography, { type TypographyProps } from "@mui/material/Typography";
import WarningAmberIcon from '@mui/icons-material/WarningAmber';
import {
    PaymentsOutlined,
    DescriptionOutlined,
    SentimentDissatisfied,
    Gavel,
} from "@mui/icons-material";
import { ContentWrapper, SectionContainer, SectionHeader, SectionLabel } from "./SectionComponents";
import type { SvgIconProps } from "@mui/material/SvgIcon";
import { tokens } from "@/app/providers/ThemeProvider";

interface ProblemCardData {
    id: string;
    icon: React.ComponentType<SvgIconProps>;
    value: string;
    label: string;
    description: string;
    iconBgColor: string;
    valueColor: string;
};

const PROBLEM_CARDS: ProblemCardData[] = [
    {
        id: "wage-underpayments",
        icon: PaymentsOutlined,
        value: "$1.35B",
        label: "Wage Underpayments",
        description: "Recovered by Fair Work in 2023-24",
        iconBgColor: "#EF44441A",
        valueColor: "#DC2626",
    },
    {
        id: "modern-awards",
        icon: DescriptionOutlined,
        value: "122",
        label: "Modern Awards",
        description: "Complex rules and variations",
        iconBgColor: "#F973161A",
        valueColor: "#D97706",
    },
    {
        id: "smes-not-confident",
        icon: SentimentDissatisfied,
        value: "73%",
        label: "SMEs Not Confident",
        description: "Too complex to DIY",
        iconBgColor: "#E0E7FF",
        valueColor: "#6366F1",
    },
    {
        id: "fwo-recovery",
        icon: Gavel,
        value: "$532M",
        label: "FWO Recovery",
        description: "Enforcement intensifying",
        iconBgColor: "#06B6D41A",
        valueColor: "#0891B2",
    },
];


const CardsGrid = styled(Box)<BoxProps>(({ theme }) => ({
    display: "grid",
    alignItems: "stretch",
    gridTemplateColumns: "1fr",
    gap: theme.spacing(3),
    [theme.breakpoints.up("sm")]: {
        gridTemplateColumns: "repeat(2, minmax(180px,1fr))",
    },
    [theme.breakpoints.up("md")]: {
        gridTemplateColumns: "repeat(4, minmax(180px,1fr))",
        gap: theme.spacing(2.5),
    },
}));

const ProblemCard = styled(Box)<BoxProps>(({ theme }) => ({
    backgroundColor: tokens.colors.white,
    borderRadius: tokens.borderRadius.large,
    border: `1px solid ${tokens.colors.gray100}`,
    padding: theme.spacing(4, 3),
    textAlign: "center",
    boxShadow: tokens.cardShadow,
    transition: tokens.transition,
    height: "100%",
    display: "flex",
    flexDirection: "column",

    "&:hover": {
        transform: "translateY(-4px)",
        boxShadow: tokens.cardHoverShadow,
        borderColor: tokens.colors.gray200,
    },

    [theme.breakpoints.down("sm")]: {
        padding: theme.spacing(3, 2.5),
    },
}));


const IconContainer = styled(Box, {
    shouldForwardProp: (prop) => prop !== "bgColor",
})<BoxProps & { bgColor: string }>(({ bgColor, theme }) => ({
    width: theme.spacing(8),
    height: theme.spacing(8),
    borderRadius: tokens.borderRadius.circle,
    backgroundColor: bgColor,
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    margin: `0 auto ${theme.spacing(2.5)}`,
}));

const StyledIcon = styled(Box, {
    shouldForwardProp: (prop) => prop !== "iconColor",
})<BoxProps & { iconColor: string }>(({ iconColor }) => ({
    color: iconColor,
    fontSize: "2rem",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
}));


const ValueText = styled(Typography, {
    shouldForwardProp: (prop) => prop !== "valueColor",
})<TypographyProps & { valueColor: string }>(({ valueColor, theme }) => ({
    fontSize: "2.5rem",
    fontWeight: 700,
    color: valueColor,
    lineHeight: 1,
    marginBottom: theme.spacing(1.5),
}));

const LabelText = styled(Typography)<TypographyProps>(({theme})=>({
    fontSize: "1rem",
    fontWeight: 600,
    color: tokens.colors.gray900,
    marginBottom: theme.spacing(1),
    lineHeight: 1.3,

}));

const DescriptionText = styled(Typography)<TypographyProps>({
    fontSize: "0.875rem",
    color: tokens.colors.gray500,
    lineHeight: 1.5,

});


interface CardProps {
    data: ProblemCardData
}

const Card: React.FC<CardProps> = ({ data }) => {
    const Icon = data.icon;
    return (
        <ProblemCard component="article">
            <IconContainer bgColor={data.iconBgColor}>
                <StyledIcon iconColor={data.valueColor} component={Icon} aria-hidden="true"/>
            </IconContainer>

            <ValueText valueColor={data.valueColor}>{data.value}</ValueText>
            <LabelText component="h3">{data.label}</LabelText>
            <DescriptionText>{data.description}</DescriptionText>
        </ProblemCard>
    )
};

export const ProblemSection: React.FC = () => {
    return (
        <SectionContainer>
            <ContentWrapper>
                <SectionLabel icon={<WarningAmberIcon />}>THE PROBLEM</SectionLabel>
                <SectionHeader
                    title="Australian SMEs Face Real Compliance Risks"
                    subtitle="Complex award systems and changing regulations make compliance a nightmare"
                />
                <CardsGrid>
                    {PROBLEM_CARDS.map((card) => (
                        <Card key={card.id} data={card} />
                    ))}
                </CardsGrid>
            </ContentWrapper>
        </SectionContainer>
    )
};
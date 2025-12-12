import Box, { type BoxProps } from "@mui/material/Box";
import { styled } from "@mui/material/styles";
import Typography, { type TypographyProps } from "@mui/material/Typography";
import {
    AttachMoney,
    Description,
    SentimentDissatisfied,
    Gavel,
} from "@mui/icons-material";

interface ProblemCardData {
    id: number;
    icon: React.ComponentType<{ className?: string }>;
    value: string;
    label: string;
    description: string;
    iconBgColor: string;
    valueColor: string;
}


const PROBLEM_CARDS: ProblemCardData[] = [
    {
        id: 1,
        icon: AttachMoney,
        value: "$1.35B",
        label: "Wage Underpayments",
        description: "Recovered by Fair Work in 2023-24",
        iconBgColor: "#FEE2E2",
        valueColor: "#DC2626",
    },
    {
        id: 2,
        icon: Description,
        value: "122",
        label: "Modern Awards",
        description: "Complex rules and variations",
        iconBgColor: "#FEF3C7",
        valueColor: "#D97706",
    },
    {
        id: 3,
        icon: SentimentDissatisfied,
        value: "73%",
        label: "SMEs Not Confident",
        description: "Too complex to DIY",
        iconBgColor: "#E0E7FF",
        valueColor: "#6366F1",
    },
    {
        id: 4,
        icon: Gavel,
        value: "$532M",
        label: "FWO Recovery",
        description: "Enforcement intensifying",
        iconBgColor: "#CFFAFE",
        valueColor: "#0891B2",
    },
];

const SectionContainer = styled("section")(({ theme }) => ({
    backgroundColor: "#F9FAFB",
    padding: "80px 24px",
    [theme.breakpoints.up("md")]: {
        padding: "100px 48px",
    }
}));

const ContentWrapper = styled(Box)({
    maxWidth: "1280px",
    margin: "0 auto",
});

const SectionLabel = styled(Box)({
    display: "inline-flex",
    alignItems: "center",
    gap: "6px",
    backgroundColor: "#E0E7FF",
    color: "#6366F1",
    padding: "6px 16px",
    borderRadius: "20px",
    fontSize: "13px",
    fontWeight: 600,
    textTransform: "uppercase",
    letterSpacing: "0.5px",
    marginBottom: "24px",
});

const MainHeading = styled("h2")({
    fontSize: "36px",
    fontWeight: 700,
    color: "#111827",
    textAlign: "center",
    marginBottom: "12px",
    lineHeight: 1.2,

    "@media(min-width:768px)": {
        fontSize: "42px",
    },
});

const SubHeading = styled(Typography)<TypographyProps>({
    fontSize: "16px",
    color: "#6B7280",
    textAlign: "center",
    marginBottom: "48px",
    lineHeight: 1.6,

    "@media(min-width:768px)": {
        fontSize: "18px",
    },
});

const CardsGrid = styled(Box)(({ theme }) => ({
    display: "grid",
    alignItems: "stretch",
    gridTemplateColumns: "1fr",
    gap: "24px",
    [theme.breakpoints.up("sm")]: {
        gridTemplateColumns: "repeat(2, 1fr)",
    },
    [theme.breakpoints.up("md")]: {
        gridTemplateColumns: "repeat(4, 1fr)",
        gap: "20px",
    },
}));

const ProblemCard = styled(Box)<BoxProps>(({ theme }) => ({
    backgroundColor: "#FFFFFF",
    borderRadius: "16px",
    border: "1px solid #F3F4F6",
    padding: "32px 24px",
    textAlign: "center",
    boxShadow: "0 1px 3px rgba(0, 0, 0, 0.05)",
    transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
    height: "100%",
    display: "flex",
    flexDirection: "column",


    "&:hover": {
        transform: "translateY(-4px)",
        boxShadow: "0 12px 24px rgba(0, 0, 0, 0.1)",
        borderColor: "#E5E7EB"
    },

    [theme.breakpoints.down("sm")]: {
        padding: "24px 20px",
    },
}));


const IconContainer = styled(Box, {
    shouldForwardProp: (prop) => prop !== "bgColor",
})<BoxProps & { bgColor: string }>(({ bgColor }) => ({
    width: "64px",
    height: "64px",
    borderRadius: "50%",
    backgroundColor: bgColor,
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    margin: "0 auto 20px",
}));

const StyledIcon = styled(Box, {
    shouldForwardProp: (prop) => prop !== "iconColor",
})<BoxProps & { iconColor: string }>(({ iconColor }) => ({
    color: iconColor,
    fontSize: "32px",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
}));


const ValueText = styled(Typography, {
    shouldForwardProp: (prop) => prop !== "valueColor",
})<TypographyProps & { valueColor: string }>(({ valueColor }) => ({
    fontSize: "40px",
    fontWeight: 700,
    color: valueColor,
    lineHeight: 1,
    marginBottom: "12px",
}));

const LabelWrapper = styled(Box)({
    display: "flex",
    justifyContent: "center",
});

const LabelText = styled(Typography)<TypographyProps>({
    fontSize: "16px",
    fontWeight: 600,
    color: "#111827",
    marginBottom: "8px",
    lineHeight: 1.3,

});

const DescriptionText = styled(Typography)<TypographyProps>({
    fontSize: "14px",
    color: "#6B7280",
    lineHeight: 1.5,

});


interface CardProps {
    data: ProblemCardData
}

const Card: React.FC<CardProps> = ({ data }) => {
    const Icon = data.icon;
    return (
        <ProblemCard>
            <IconContainer bgColor={data.iconBgColor}>
                <StyledIcon iconColor={data.valueColor} component={Icon} />
            </IconContainer>

            <ValueText valueColor={data.valueColor}>{data.value}</ValueText>
            <LabelText>{data.label}</LabelText>
            <DescriptionText>{data.description}</DescriptionText>
        </ProblemCard>
    )
}

export const ProblemSection: React.FC = () => {
    return (
        <SectionContainer>
            <ContentWrapper>
                <LabelWrapper>
                    <SectionLabel>THE PROBLEM</SectionLabel>
                </LabelWrapper>
                <MainHeading>Australian SMEs Face Real Compliance Risks</MainHeading>
                <SubHeading>
                    Complex award systems and changing regulations make compliance a nightmare
                </SubHeading>

                <CardsGrid>
                    {PROBLEM_CARDS.map((card) => (
                        <Card key={card.id} data={card} />
                    ))}
                </CardsGrid>
            </ContentWrapper>
        </SectionContainer>
    )
}
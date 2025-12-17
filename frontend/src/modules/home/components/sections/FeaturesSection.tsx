import { styled } from "@mui/material/styles";
import { AccountBalanceWalletOutlined, ArticleOutlined, CheckCircleOutline, ShieldOutlined, SupportAgent } from '@mui/icons-material';
import Box, { type BoxProps } from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import Paper, { type PaperProps } from '@mui/material/Paper';
import { alpha } from '@mui/material/styles';
import Typography, { type TypographyProps } from '@mui/material/Typography';
import React from 'react'
import type { SvgIconProps } from "@mui/material/SvgIcon";
import { SectionContainer, ContentWrapper, SectionLabel, SectionHeader } from "./SectionComponents";
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome';
import { tokens } from "@/app/providers/ThemeProvider";


type ThemeColor = 'primary' | 'warning' | 'info' | 'success';


interface FeatureCardData {
  id: string;
  agentType: string;
  themeColor: ThemeColor;
  icon: React.ComponentType<SvgIconProps>;
  title: string;
  description: string;
  features: string[];
};

const FEATURE_CARDS: FeatureCardData[] = [
  {
    id: "compliance-agent",
    agentType: "COMPLIANCE AGENT",
    themeColor: "primary",
    icon: ShieldOutlined,
    title: "Roster Compliance Checking",
    description: "Upload your roster and get instant compliance analysis against Modern Award requirements.",
    features: [
      "Hours validation",
      "Break requirements",
      "Penality checks",
      "AI Q&A",
    ],
  },
  {
    id: "payroll-agent",
    agentType: "PAYROLL AGENT",
    themeColor: "warning",
    icon: AccountBalanceWalletOutlined,
    title: "Pay Validation & Audit",
    description: "Validate your payroll against award rates before you process, not after.",
    features: [
      'Rate validation',
      'Underpayment detect',
      'Super checks',
      'STP Phase 2',
    ],
  },
  {
    id: "document-agent",
    agentType: "DOCUMENT AGENT",
    themeColor: "info",
    icon: ArticleOutlined,
    title: "Compliant Documents",
    description: "Generate legally compliant HR documents in seconds with AI assistance.",
    features: [
      'Contracts',
      'Warning letters',
      'Termination',
      'PDF export',
    ],
  },
  {
    id: "self-service-agent",
    agentType: "SELF-SERVICE AGENT",
    themeColor: "success",
    icon: SupportAgent,
    title: "Employee Portal",
    description: "Let employees answer their own questions about pay and leave entitlements.",
    features: [
      'Leave queries',
      'Pay questions',
      'Policy lookups',
      '24/7 available',
    ],
  },

];


const CardsGrid = styled(Box)<BoxProps>(({ theme }) => ({
  display: "grid",
  alignItems: "stretch",
  gridTemplateColumns: "1fr",
  gap: theme.spacing(3),
  [theme.breakpoints.up("sm")]: {
    gridTemplateColumns: "repeat(2, minmax(330px,1fr))",
  },
}));

const FeatureCard = styled(Paper)<PaperProps>(({ theme }) => ({
  padding: theme.spacing(4),
  borderRadius: theme.spacing(2),
  border: `1px solid ${theme.palette.divider}`,
  transition: 'all 0.3s ease',

  "&:hover": {
    boxShadow: tokens.cardShadow,
    transform: "translateY(-4px)",
    borderColor: theme.palette.primary.main,
  }
}));

const CardHeader = styled(Box)<BoxProps>(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  gap: theme.spacing(2),
  marginBottom: theme.spacing(3),
}));

const IconWrapper = styled(Box, {
  shouldForwardProp: (prop) => prop !== 'themeColor',
})<BoxProps & { themeColor: ThemeColor }>(({ theme, themeColor }) => ({
  width: theme.spacing(8),
  height: theme.spacing(8),
  borderRadius: "50%",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  flexShrink: 0,

  backgroundColor: alpha(theme.palette[themeColor].main, 0.1),
  color: theme.palette[themeColor].main,
  "& svg": {
    fontSize: "2rem",
  },
}));

const HeaderContent = styled(Box)<BoxProps>(({ theme }) => ({
  flex: 1,
  display: "flex",
  flexDirection: "column",
  alignItems: "flex-start",
  gap: theme.spacing(0.5),
}));

const AgentTypeLabel = styled(Chip, {
  shouldForwardProp: (prop) => prop !== "themeColor",
})<{ themeColor: ThemeColor }>(({ theme, themeColor }) => ({
  fontSize: "0.75rem",
  fontWeight: 600,
  letterSpacing: "0.05em",
  backgroundColor: "transparent",
  color: theme.palette[themeColor].main,
  height: "auto",
  padding: 0,
  "& .MuiChip-label": {
    padding: 0,
  },
}));

const CardTitle = styled('h3')(({ theme }) => ({
  fontSize: "1.25rem",
  fontWeight: 700,
  color: theme.palette.text.primary,
  margin: 0,
  lineHeight: 1.3,
}));

const Description = styled(Typography)<TypographyProps>(({ theme }) => ({
  fontSize: "0.938rem",
  lineHeight: 1.6,
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(3),
}));

const FeaturesGrid = styled("ul")(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "repeat(2, 1fr)",
  gap: theme.spacing(1.5),
  listStyle: "none",
  padding: 0,
  margin: 0,

  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: "1fr",
  },
}));

const FeatureItem = styled("li")(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  gap: theme.spacing(1),
  fontSize: "0.875rem",
  color: theme.palette.text.secondary,
}));

const CheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
  fontSize: "1.25rem",
  color: theme.palette.success.main,
  flexShrink: 0,
}));

interface CardProps {
  data: FeatureCardData;
};

const Card: React.FC<CardProps> = ({ data }) => {
  const { themeColor, icon: IconComponent, agentType, title, description, features } = data;

  return (
    <FeatureCard elevation={0} component="article">
      <CardHeader component="header">
        <IconWrapper themeColor={themeColor} aria-hidden="true">
          <IconComponent />
        </IconWrapper>

        <HeaderContent>
          <AgentTypeLabel themeColor={themeColor} label={agentType} />
          <CardTitle>{title}</CardTitle>
        </HeaderContent>
      </CardHeader>


      <Description>{description}</Description>

      <FeaturesGrid>
        {features.map((feature) => (
          <FeatureItem key={`${agentType}-${feature}`}>
            <CheckIcon aria-hidden="true" />
            <span>{feature}</span>
          </FeatureItem>
        ))}
      </FeaturesGrid>
    </FeatureCard>
  );
}

export const FeaturesSection = () => {
  return (
    <SectionContainer bgColor="white">
      <ContentWrapper>
        <SectionLabel icon={<AutoAwesomeIcon />}>FEATURES</SectionLabel>
        <SectionHeader
          title="Four AI Agents Working For You"
          subtitle="Specialised AI that understands Australian workplace law"
        />
        <CardsGrid>
          {FEATURE_CARDS.map((card) => (
            <Card key={card.id} data={card} />
          ))}
        </CardsGrid>
      </ContentWrapper>
    </SectionContainer>
  )
};

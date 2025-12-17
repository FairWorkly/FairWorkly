import { tokens } from "@/app/providers/ThemeProvider";
import { styled } from "@mui/material/styles";
import Box, { type BoxProps } from "@mui/material/Box";
import Typography, { type TypographyProps } from "@mui/material/Typography";

export const SectionContainer = styled("section")<{ bgColor?: "white" | "gray" }>(({ theme, bgColor = "gray" }) => ({
    backgroundColor: bgColor === "white" ? tokens.colors.white : tokens.colors.gray50,
    padding: tokens.spacing.sectionPaddingMobile,

    [theme.breakpoints.up("md")]: {
        padding: tokens.spacing.sectionPaddingDesktop,
    },
}));

export const ContentWrapper = styled(Box)<BoxProps>({
    maxWidth: "1280px",
    margin: "0 auto",
});

const StyledLabel = styled(Box)<BoxProps>(({ theme }) => ({
    display: "inline-flex",
    alignItems: "center",
    gap: theme.spacing(0.75),
    backgroundColor: tokens.colors.primaryLight,
    color: tokens.colors.primary,
    padding: theme.spacing(0.75, 2),
    borderRadius: tokens.borderRadius.pill,
    fontSize: "0.8125rem",
    fontWeight: 600,
    textTransform: "uppercase",
    letterSpacing: "0.5px",
}));

const LabelWrapper = styled(Box)<BoxProps>(({ theme }) => ({
    display: "flex",
    justifyContent: "center",
    marginBottom: theme.spacing(3),
}));

const LabelIcon = styled(Box)<BoxProps>({
    display: "flex",
    alignItems: "center",
    fontSize: "1rem",
});

interface SectionLabelProps {
    children: string;
    icon?: React.ReactNode;
}

export const SectionLabel: React.FC<SectionLabelProps> = ({ children, icon }) => {
    return (
        <LabelWrapper>
            <StyledLabel>
                {icon && <LabelIcon>{icon}</LabelIcon>}
                {children}
            </StyledLabel>
        </LabelWrapper>
    );
};

const MainHeading = styled("h2")(({ theme }) => ({
    fontSize: "2.25rem",
    fontWeight: 700,
    color: tokens.colors.gray900,
    textAlign: "center",
    marginBottom: theme.spacing(1.5),
    lineHeight: 1.2,

    [theme.breakpoints.up("md")]: {
        fontSize: "2.625rem",
    },
}));

const SubHeading = styled(Typography)<TypographyProps>(({ theme }) => ({
    fontSize: "1rem",
    color: tokens.colors.gray500,
    textAlign: "center",
    marginBottom: theme.spacing(6),
    lineHeight: 1.6,

    [theme.breakpoints.up("md")]: {
        fontSize: "1.125rem",
    },
}));

interface SectionHeaderProps {
    title: string;
    subtitle: string;
}

export const SectionHeader: React.FC<SectionHeaderProps> = ({ title, subtitle }) => {
    return (
        <>
            <MainHeading>{title}</MainHeading>
            <SubHeading>{subtitle}</SubHeading>
        </>
    );
};
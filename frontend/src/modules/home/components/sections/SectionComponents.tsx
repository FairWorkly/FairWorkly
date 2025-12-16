import { tokens } from "@/app/providers/ThemeProvider";
import { styled } from "@mui/material/styles";
import Box from "@mui/material/Box";
import Typography, { type TypographyProps } from "@mui/material/Typography";

export const SectionContainer = styled("section")(({ theme }) => ({
    backgroundColor: tokens.colors.gray50,
    padding: tokens.spacing.sectionPaddingMobile,

    [theme.breakpoints.up("md")]: {
        padding: tokens.spacing.sectionPaddingDesktop,
    },
}));

export const ContentWrapper = styled(Box)({
    maxWidth: "1280px",
    margin: "0 auto",
});

const StyledLabel = styled(Box)({
    display: "inline-flex",
    alignItems: "center",
    gap: "6px",
    backgroundColor: tokens.colors.primaryLight,
    color: tokens.colors.primary,
    padding: "6px 16px",
    borderRadius: "20px",
    fontSize: "13px",
    fontWeight: 600,
    textTransform: "uppercase",
    letterSpacing: "0.5px",
});

const LabelWrapper = styled(Box)({
    display: "flex",
    justifyContent: "center",
    marginBottom: "24px",
});

interface SectionLabelProps {
    children: string;
}

export const SectionLabel: React.FC<SectionLabelProps> = ({ children }) => {
    return (
        <LabelWrapper>
            <StyledLabel>{children}</StyledLabel>
        </LabelWrapper>
    );
};

const MainHeading = styled("h2")({
    fontSize: "36px",
    fontWeight: 700,
    color: tokens.colors.gray900,
    textAlign: "center",
    marginBottom: "12px",
    lineHeight: 1.2,

    "@media(min-width:768px)": {
        fontSize: "42px",
    },
});

const SubHeading = styled(Typography)<TypographyProps>({
    fontSize: "16px",
    color: tokens.colors.gray500,
    textAlign: "center",
    marginBottom: "48px",
    lineHeight: 1.6,

    "@media(min-width:768px)": {
        fontSize: "18px",
    },
});

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
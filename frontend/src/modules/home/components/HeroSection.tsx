import { styled } from "@mui/material/styles";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import Card from "@mui/material/Card";

const Section = styled("section")(({ theme }) => ({
  maxWidth: 1200,
  margin: "0 auto",
  padding: theme.spacing(6, 2, 4),
}));

const Grid = styled("div")(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1fr",
  gap: theme.spacing(4),
  alignItems: "center",
  "@media (min-width: 900px)": {
    gridTemplateColumns: "1.05fr 0.95fr",
    gap: theme.spacing(6),
  },
}));

const Capsule = styled("div")(({ theme }) => ({
  display: "inline-flex",
  alignItems: "center",
  gap: theme.spacing(1),
  padding: theme.spacing(0.75, 1.5),
  borderRadius: 999,
  border: `1px solid ${theme.palette.divider}`,
  backgroundColor: theme.palette.background.paper,
  width: "fit-content",
}));

const Dot = styled("span")(() => ({
  width: 8,
  height: 8,
  borderRadius: 999,
  background: "linear-gradient(135deg, #34d399 0%, #22c55e 100%)",
  display: "inline-block",
}));

const Headline = styled(Typography)(({ theme }) => ({
  fontWeight: 900,
  letterSpacing: -0.8,
  lineHeight: 1.05,
  color: theme.palette.text.primary,
}));

const Accent = styled("span")(() => ({
  background: "linear-gradient(135deg, #7c4dff 0%, #ff4fd8 100%)",
  WebkitBackgroundClip: "text",
  backgroundClip: "text",
  color: "transparent",
}));

const Copy = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  maxWidth: 520,
}));

const ButtonRow = styled("div")(({ theme }) => ({
  display: "flex",
  flexWrap: "wrap",
  gap: theme.spacing(1.5),
  marginTop: theme.spacing(2),
}));

const PrimaryButton = styled(Button)(({ theme }) => ({
  textTransform: "none",
  borderRadius: 14,
  padding: theme.spacing(1.2, 2.2),
  color: theme.palette.common.white,
  background: "linear-gradient(135deg, #7c4dff 0%, #ff4fd8 100%)",
  boxShadow: "0 12px 28px rgba(124,77,255,0.22)",
  "&:hover": {
    background: "linear-gradient(135deg, #6b40ff 0%, #ff3dcc 100%)",
  },
}));

const SecondaryButton = styled(Button)(({ theme }) => ({
  textTransform: "none",
  borderRadius: 14,
  padding: theme.spacing(1.2, 2.2),
  borderColor: theme.palette.divider,
}));

const StatsRow = styled("div")(({ theme }) => ({
  display: "flex",
  gap: theme.spacing(3),
  marginTop: theme.spacing(3),
  flexWrap: "wrap",
}));

const Stat = styled("div")(() => ({
  minWidth: 120,
}));

const StatValue = styled("div")(({ theme }) => ({
  fontSize: 28,
  fontWeight: 900,
  color: "#6b40ff",
}));

const StatLabel = styled("div")(({ theme }) => ({
  fontSize: 12,
  color: theme.palette.text.secondary,
  marginTop: theme.spacing(0.5),
}));

const VisualWrap = styled("div")(() => ({
  position: "relative",
}));

const VisualCard = styled(Card)(({ theme }) => ({
  borderRadius: 18,
  overflow: "hidden",
  border: `1px solid ${theme.palette.divider}`,
  boxShadow: "0 20px 60px rgba(0,0,0,0.10)",
}));

const Img = styled("img")(() => ({
  display: "block",
  width: "100%",
  height: "auto",
}));

const ComplianceBadge = styled(Card)(({ theme }) => ({
  position: "absolute",
  left: theme.spacing(2),
  bottom: theme.spacing(2),
  borderRadius: 14,
  padding: theme.spacing(1.2, 1.5),
  display: "flex",
  alignItems: "center",
  gap: theme.spacing(1),
  boxShadow: "0 12px 30px rgba(0,0,0,0.12)",
}));

const BadgeIcon = styled("span")(() => ({
  width: 28,
  height: 28,
  borderRadius: 999,
  background: "rgba(34,197,94,0.12)",
  display: "inline-flex",
  alignItems: "center",
  justifyContent: "center",
  fontWeight: 900,
  color: "#16a34a",
}));

export default function HeroSection() {
  return (
    <Section aria-label="Hero">
      <Grid>
        <div>
          <Capsule>
            <Dot />
            <Typography variant="body2" component="span">
              Now supporting Retail, Hospitality & Clerks Awards
            </Typography>
          </Capsule>

          <Stack spacing={2.2} mt={3} sx={undefined as never}>
            <Headline variant="h2" component="h1">
              AI-Powered HR <br />
              Compliance for <br />
              <Accent>Australian SMEs</Accent>
            </Headline>

            <Copy variant="body1" component="p">
              Stop worrying about wage underpayment. FairWorkly checks your rosters and payroll against Modern Awards in seconds, not hours.
            </Copy>

            <ButtonRow>
              <PrimaryButton href="/register">Start Free Trial →</PrimaryButton>
              <SecondaryButton variant="outlined" href="#how-it-works">
                See How It Works
              </SecondaryButton>
            </ButtonRow>

            <StatsRow aria-label="Key stats">
              <Stat>
                <StatValue>500+</StatValue>
                <StatLabel>Australian SMEs</StatLabel>
              </Stat>
              <Stat>
                <StatValue>{"<3s"}</StatValue>
                <StatLabel>Check Time</StatLabel>
              </Stat>
              <Stat>
                <StatValue>99.2%</StatValue>
                <StatLabel>Accuracy</StatLabel>
              </Stat>
            </StatsRow>
          </Stack>
        </div>

        <VisualWrap aria-label="Hero visual">
          <VisualCard>
            <Img
              src="https://images.unsplash.com/photo-1521737604893-d14cc237f11d?auto=format&fit=crop&w=1400&q=80"
              alt="Team meeting"
              loading="lazy"
              referrerPolicy="no-referrer"
            />
          </VisualCard>

          <ComplianceBadge>
            <BadgeIcon>✓</BadgeIcon>
            <div>
              <Typography variant="subtitle2" component="div">
                Compliant
              </Typography>
              <Typography variant="caption" component="div" color="text.secondary">
                All checks passed
              </Typography>
            </div>
          </ComplianceBadge>
        </VisualWrap>
      </Grid>
    </Section>
  );
}

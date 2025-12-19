import { styled } from "@mui/material/styles";
import Typography from "@mui/material/Typography";

const Section = styled("section")(({ theme }) => ({
  maxWidth: 1200,
  margin: "0 auto",
  padding: theme.spacing(2, 2, 6),
}));

const Bar = styled("div")(({ theme }) => ({
  borderTop: `1px solid ${theme.palette.divider}`,
  paddingTop: theme.spacing(3),
  display: "grid",
  gridTemplateColumns: "1fr 1fr",
  gap: theme.spacing(2),
  "@media (min-width: 900px)": {
    gridTemplateColumns: "repeat(4, 1fr)",
    gap: theme.spacing(3),
  },
}));

const Item = styled("div")(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  gap: theme.spacing(1.25),
  justifyContent: "center",
  padding: theme.spacing(1.25, 1),
  color: theme.palette.text.secondary,
}));

const Icon = styled("span")(({ theme }) => ({
  width: 34,
  height: 34,
  borderRadius: 999,
  display: "inline-flex",
  alignItems: "center",
  justifyContent: "center",
  border: `1px solid ${theme.palette.divider}`,
  backgroundColor: theme.palette.background.paper,
  color: theme.palette.text.primary,
  fontWeight: 900,
}));

export default function TrustBar() {
  return (
    <Section aria-label="Trust bar">
      <Bar>
        <Item>
          <Icon>🛡</Icon>
          <Typography variant="body2" component="span">
            SOC 2 Compliant
          </Typography>
        </Item>
        <Item>
          <Icon>📍</Icon>
          <Typography variant="body2" component="span">
            Australian Hosted
          </Typography>
        </Item>
        <Item>
          <Icon>⚖</Icon>
          <Typography variant="body2" component="span">
            Fair Work Aligned
          </Typography>
        </Item>
        <Item>
          <Icon>💬</Icon>
          <Typography variant="body2" component="span">
            Local Support
          </Typography>
        </Item>
      </Bar>
    </Section>
  );
}

import { styled } from "@mui/material/styles";
import Button from "@mui/material/Button";
import Link from "@mui/material/Link";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";

const Header = styled("header")(({ theme }) => ({
  position: "sticky",
  top: 0,
  zIndex: theme.zIndex.appBar,
  background: "linear-gradient(90deg, #0f0b2e 0%, #1a1240 50%, #110b2f 100%)",
  borderBottom: `1px solid ${theme.palette.divider}`,
}));

const Bar = styled("div")(({ theme }) => ({
  maxWidth: 1200,
  margin: "0 auto",
  padding: theme.spacing(1.5, 2),
  display: "flex",
  alignItems: "center",
  justifyContent: "space-between",
  gap: theme.spacing(2),
}));

const Brand = styled("div")(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  gap: theme.spacing(1),
  color: theme.palette.common.white,
}));

const Bolt = styled("span")(() => ({
  width: 32,
  height: 32,
  borderRadius: 10,
  display: "inline-flex",
  alignItems: "center",
  justifyContent: "center",
  background: "linear-gradient(135deg, #7c4dff 0%, #ff4fd8 100%)",
  fontWeight: 800,
}));

const Nav = styled("nav")(({ theme }) => ({
  display: "none",
  alignItems: "center",
  gap: theme.spacing(3),
  "@media (min-width: 900px)": {
    display: "flex",
  },
}));

const NavLink = styled(Link)(({ theme }) => ({
  color: "rgba(255,255,255,0.85)",
  textDecoration: "none",
  fontSize: 14,
  fontWeight: 600,
  "&:hover": {
    color: theme.palette.common.white,
    textDecoration: "none",
  },
}));

const Actions = styled("div")(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  gap: theme.spacing(1.25),
}));

const GhostButton = styled(Button)(({ theme }) => ({
  color: theme.palette.common.white,
  borderColor: "rgba(255,255,255,0.25)",
  textTransform: "none",
  borderRadius: 999,
  padding: theme.spacing(0.75, 2),
}));

const GradientButton = styled(Button)(({ theme }) => ({
  textTransform: "none",
  borderRadius: 999,
  padding: theme.spacing(0.85, 2.25),
  color: theme.palette.common.white,
  background: "linear-gradient(135deg, #7c4dff 0%, #ff4fd8 100%)",
  boxShadow: "0 10px 24px rgba(124,77,255,0.25)",
  "&:hover": {
    background: "linear-gradient(135deg, #6b40ff 0%, #ff3dcc 100%)",
  },
}));

export default function Navbar() {
  return (
    <Header>
      <Bar>
        <Brand>
          <Bolt>⚡</Bolt>
          <Typography variant="h6" component="div" sx={undefined as never}>
            FairWorkly
          </Typography>
        </Brand>

        <Nav aria-label="Primary">
          <NavLink href="#features">Features</NavLink>
          <NavLink href="#how-it-works">How It Works</NavLink>
          <NavLink href="#pricing">Pricing</NavLink>
          <NavLink href="#testimonials">Testimonials</NavLink>
        </Nav>

        <Actions>
          <GhostButton variant="outlined" href="/login">
            Log In
          </GhostButton>
          <GradientButton variant="contained" href="/register">
            Start Free Trial
          </GradientButton>
        </Actions>
      </Bar>
    </Header>
  );
}

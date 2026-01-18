import {
  Box,
  Container,
  Typography,
  Link as MuiLink,
  IconButton,
  styled,
  alpha
} from '@mui/material';
import { LinkedIn, X as XIcon, Bolt } from '@mui/icons-material';

const FooterContainer = styled('footer')(({ theme }) => ({
  width: '100vw',  
  position: 'relative',
  left: '50%',
  right: '50%',
  marginLeft: '-50vw',
  marginRight: '-50vw',
  backgroundColor: theme.fairworkly.surface.navDark,
  color: alpha(theme.palette.common.white, 0.7),
  padding: theme.spacing(8, 0, 4),
}));

const FooterGrid = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '2fr 1fr 1fr 1fr',
  gap: theme.spacing(6),
  marginBottom: theme.spacing(6),
  [theme.breakpoints.down('md')]: {
    gridTemplateColumns: '1fr 1fr',
  },
  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: '1fr',
  },
}));

const BrandColumn = styled(Box)(({ theme }) => ({
  '& p': {
    fontSize: theme.typography.body2.fontSize,
    lineHeight: 1.7,
    color: alpha(theme.palette.common.white, 0.6),
  },
}));

const LogoLink = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  textDecoration: 'none',
  marginBottom: theme.spacing(2),
  cursor: 'pointer',
}));

const LogoIcon = styled(Box)(({ theme }) => ({
  width: theme.spacing(5),
  height: theme.spacing(5),
  background: theme.fairworkly.gradient.primary,
  borderRadius: theme.fairworkly.radius.sm,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.common.white,
}));

const LogoText = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h6.fontSize,
  fontWeight: theme.typography.h6.fontWeight,
  background: theme.fairworkly.gradient.brandText,
  WebkitBackgroundClip: 'text',
  WebkitTextFillColor: 'transparent',
  backgroundClip: 'text',
}));

const ColumnTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  color: theme.palette.common.white,
  textTransform: 'uppercase',
  letterSpacing: theme.typography.caption.letterSpacing,
  marginBottom: theme.spacing(3),
}));

const NavList = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1.5),
}));

const FooterLink = styled(MuiLink)(({ theme }) => ({
  color: alpha(theme.palette.common.white, 0.6),
  textDecoration: 'none',
  fontSize: theme.typography.body2.fontSize,
  transition: theme.transitions.create(['color'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    color: theme.palette.common.white,
  },
}));

const SupportEmail = styled(MuiLink)(({ theme }) => ({
  color: theme.palette.primary.main,
  textDecoration: 'none',
  '&:hover': {
    textDecoration: 'underline',
  },
}));

const BottomBar = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'space-between',
  paddingTop: theme.spacing(4),
  borderTop: `1px solid ${alpha(theme.palette.common.white, 0.1)}`,
  fontSize: theme.typography.body2.fontSize,
  color: alpha(theme.palette.common.white, 0.5),
  [theme.breakpoints.down('sm')]: {
    flexDirection: 'column',
    gap: theme.spacing(2),
    textAlign: 'center',
  },
}));

const SocialLinks = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2),
}));

const SocialButton = styled(IconButton)(({ theme }) => ({
  width: theme.spacing(5),
  height: theme.spacing(5),
  borderRadius: theme.fairworkly.radius.sm,
  backgroundColor: alpha(theme.palette.common.white, 0.1),
  color: alpha(theme.palette.common.white, 0.7),
  transition: theme.transitions.create(['all'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.common.white,
  },
}));

export function Footer() {
  return (
    <FooterContainer>
      <Container maxWidth="lg">
        <FooterGrid>
          <BrandColumn>
            <LogoLink component="a" href="/">
              <LogoIcon>
                <Bolt />
              </LogoIcon>
              <LogoText>FairWorkly</LogoText>
            </LogoLink>
            <Typography component="p" sx={{ mb: 2 }}>
              Fair Work compliance made simple for Australian SMEs.
            </Typography>
            <Typography component="p" sx={{ fontSize: '0.875rem' }}>
              <strong>Support:</strong>{' '}
              <SupportEmail href="mailto:support@fairworkly.com">
                support@fairworkly.com
              </SupportEmail>
            </Typography>
          </BrandColumn>

          <Box>
            <ColumnTitle component="h4">Product</ColumnTitle>
            <NavList>
              <FooterLink href="#features">Features</FooterLink>
              <FooterLink href="#pricing">Pricing</FooterLink>
              <FooterLink href="#faq">FAQ</FooterLink>
              <FooterLink href="mailto:support@fairworkly.com">Contact</FooterLink>
            </NavList>
          </Box>

          <Box>
            <ColumnTitle component="h4">Resources</ColumnTitle>
            <NavList>
              <FooterLink href="/templates">CSV Templates</FooterLink>
              <FooterLink
                href="https://www.fairwork.gov.au"
                target="_blank"
                rel="noopener noreferrer"
              >
                Fair Work Ombudsman
              </FooterLink>
            </NavList>
          </Box>

          <Box>
            <ColumnTitle component="h4">Legal</ColumnTitle>
            <NavList>
              <FooterLink href="/privacy">Privacy Policy</FooterLink>
              <FooterLink href="/terms">Terms of Service</FooterLink>
            </NavList>
          </Box>
        </FooterGrid>

        <BottomBar>
          <Typography component="p">
            © 2025 FairWorkly · Made in Melbourne, Australia
          </Typography>
          <SocialLinks>
            <SocialButton aria-label="Visit our LinkedIn page">
              <LinkedIn />
            </SocialButton>
            <SocialButton aria-label="Follow us on X">
              <XIcon />
            </SocialButton>
          </SocialLinks>
        </BottomBar>
      </Container>
    </FooterContainer>
  );
}
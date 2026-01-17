import {
  Box,
  Container,
  Typography,
  Grid,
  List,
  ListItem,
  Link as MuiLink,
  IconButton,
  styled,
} from '@mui/material';
import { LinkedIn, X as XIcon, Bolt } from '@mui/icons-material';

const FooterRoot = styled('footer')(({ theme }) => ({
  backgroundColor: '#1e1b4b',
  color: 'rgba(255, 255, 255, 0.7)',
  padding: theme.spacing(8, 0, 4),
}));

const FooterGrid = styled(Grid)(({ theme }) => ({
  marginBottom: theme.spacing(6),
}));

const Brand = styled(Box)(({ theme }) => ({
  '& p': {
    fontSize: '0.9375rem',
    lineHeight: 1.7,
    color: 'rgba(255, 255, 255, 0.6)',
    marginTop: theme.spacing(2),
  },
}));

const Logo = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  textDecoration: 'none',
  marginBottom: theme.spacing(2),
}));

const LogoIcon = styled(Box)(({ theme }) => ({
  width: '40px',
  height: '40px',
  background: 'linear-gradient(135deg, #6366f1, #ec4899)',
  borderRadius: '10px',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: 'white',
}));

const LogoText = styled(Typography)(({ theme }) => ({
  fontWeight: 800,
  background: 'linear-gradient(135deg, #a5b4fc, #f9a8d4)',
  WebkitBackgroundClip: 'text',
  WebkitTextFillColor: 'transparent',
  backgroundClip: 'text',
}));

const ColumnTitle = styled(Typography)(({ theme }) => ({
  fontSize: '0.875rem',
  fontWeight: 700,
  color: theme.palette.common.white,
  textTransform: 'uppercase',
  letterSpacing: '0.05em',
  marginBottom: theme.spacing(3),
}));

const LinkList = styled(List)({
  padding: 0,
  listStyle: 'none',
});

const LinkItem = styled(ListItem)({
  padding: 0,
  marginBottom: '12px',
});

const FooterLink = styled(MuiLink)(({ theme }) => ({
  color: 'rgba(255, 255, 255, 0.6)',
  textDecoration: 'none',
  fontSize: '0.9375rem',
  transition: 'color 0.2s ease',
  '&:hover': {
    color: theme.palette.common.white,
  },
}));

const Bottom = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'space-between',
  paddingTop: theme.spacing(4),
  borderTop: '1px solid rgba(255, 255, 255, 0.1)',
  fontSize: '0.875rem',
  color: 'rgba(255, 255, 255, 0.5)',
  [theme.breakpoints.down('sm')]: {
    flexDirection: 'column',
    gap: theme.spacing(2),
    textAlign: 'center',
  },
}));

const Social = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2),
}));

const SocialButton = styled(IconButton)(({ theme }) => ({
  width: '40px',
  height: '40px',
  borderRadius: '10px',
  backgroundColor: 'rgba(255, 255, 255, 0.1)',
  color: 'rgba(255, 255, 255, 0.7)',
  transition: 'all 0.2s ease',
  '&:hover': {
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.common.white,
  },
}));

interface FooterSection {
  title: string;
  links: string[];
}

export default function Footer() {
  const sections: FooterSection[] = [
    {
      title: 'Product',
      links: ['Features', 'Pricing', 'Integrations', 'API'],
    },
    {
      title: 'Company',
      links: ['About Us', 'Careers', 'Blog', 'Contact'],
    },
    {
      title: 'Legal',
      links: ['Privacy Policy', 'Terms of Service', 'Security'],
    },
  ];

  return (
    <FooterRoot>
      <Container maxWidth="lg">
        <FooterGrid container spacing={6}>
          <Grid size={{ xs: 12, md: 5 }}>
            <Brand>
              <Logo component="a" href="#">
                <LogoIcon>
                  <Bolt />
                </LogoIcon>
                <LogoText variant="h5" component="span">FairWorkly</LogoText>
              </Logo>
              <Typography component="p">
                AI-powered HR compliance for Australian SMEs. Making Fair Work compliance simple,
                accurate, and affordable.
              </Typography>
            </Brand>
          </Grid>

          <Grid size={{ xs: 12, md: 7 }}>
            <Grid container spacing={6}>
              {sections.map((section, index) => (
                <Grid size={{ xs: 12, sm: 4 }} key={index}>
                  <ColumnTitle component="h4">{section.title}</ColumnTitle>
                  <LinkList>
                    {section.links.map((link, linkIndex) => (
                      <LinkItem key={linkIndex}>
                        <FooterLink href="#">{link}</FooterLink>
                      </LinkItem>
                    ))}
                  </LinkList>
                </Grid>
              ))}
            </Grid>
          </Grid>
        </FooterGrid>

        <Bottom>
          <Typography component="p">
            © 2025 FairWorkly · Made with 💜 in Melbourne, Australia
          </Typography>
          <Social>
            <SocialButton aria-label="LinkedIn">
              <LinkedIn />
            </SocialButton>
            <SocialButton aria-label="X">
              <XIcon />
            </SocialButton>
          </Social>
        </Bottom>
      </Container>
    </FooterRoot>
  );
}
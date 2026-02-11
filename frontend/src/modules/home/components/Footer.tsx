import {
  Box,
  Typography,
  Link as MuiLink,
  styled,
  alpha,
} from '@mui/material';
import { LinkedIn, Bolt } from '@mui/icons-material';

const PageSection = styled('footer')(({ theme }) => ({
  position: 'relative',
  backgroundColor: theme.fairworkly.surface.navDark,
  color: alpha(theme.palette.common.white, 0.7),
  padding: theme.spacing(8, 0, 4),
}));

const ContentContainer = styled(Box)(({ theme }) => ({
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  padding: theme.spacing(0, 4),
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

const BrandColumn = styled(Box)({});

const LogoLink = styled('a')(({ theme }) => ({
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

const BrandDescription = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  lineHeight: 1.7,
  color: alpha(theme.palette.common.white, 0.6),
  marginBottom: theme.spacing(2),
}));

const SupportLine = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  color: alpha(theme.palette.common.white, 0.6),
}));

const SupportEmail = styled(MuiLink)(({ theme }) => ({
  color: theme.palette.primary.main,
  textDecoration: 'none',
  '&:hover': {
    textDecoration: 'underline',
  },
}));

const ColumnTitle = styled('h4')(({ theme }) => ({
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  color: theme.palette.common.white,
  textTransform: 'uppercase',
  letterSpacing: theme.typography.caption.letterSpacing,
  marginBottom: theme.spacing(3),
  margin: 0,
  marginBlockEnd: theme.spacing(3),
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

const SocialButton = styled('a')(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  width: theme.spacing(5),
  height: theme.spacing(5),
  borderRadius: theme.fairworkly.radius.sm,
  backgroundColor: alpha(theme.palette.common.white, 0.1),
  color: alpha(theme.palette.common.white, 0.7),
  transition: theme.transitions.create(['background-color', 'color'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.common.white,
  },
}));

interface FooterLinkItem {
  label: string;
  href: string;
  external?: boolean;
}

interface FooterColumn {
  title: string;
  links: FooterLinkItem[];
}

interface SocialLink {
  label: string;
  href: string;
  icon: React.ComponentType;
}

const SUPPORT_EMAIL = 'support@fairworkly.com';

const content = {
  brand: 'FairWorkly',
  description: 'Fair Work compliance made simple for Australian SMEs.',
  supportLabel: 'Support:',
  copyright: '\u00A9 2025 FairWorkly \u00B7 Made in Melbourne, Australia',
};

const FOOTER_COLUMNS: FooterColumn[] = [
  {
    title: 'Product',
    links: [
      { label: 'Features', href: '#features' },
      { label: 'Pricing', href: '#pricing' },
      { label: 'FAQ', href: '#faq' },
      { label: 'Contact', href: `mailto:${SUPPORT_EMAIL}` },
    ],
  },
  {
    title: 'Resources',
    links: [
      { label: 'File Templates', href: '/templates' },
      { label: 'Fair Work Ombudsman', href: 'https://www.fairwork.gov.au', external: true },
    ],
  },
  {
    title: 'Legal',
    links: [
      { label: 'Privacy Policy', href: '/privacy' },
      { label: 'Terms of Service', href: '/terms' },
    ],
  },
];

const SOCIAL_LINKS: SocialLink[] = [
  { label: 'Visit our LinkedIn page', href: 'https://www.linkedin.com/company/fairworkly', icon: LinkedIn },
];

export function Footer() {
  return (
    <PageSection>
      <ContentContainer>
        <FooterGrid>
          <BrandColumn>
            <LogoLink href="/">
              <LogoIcon>
                <Bolt />
              </LogoIcon>
              <LogoText>{content.brand}</LogoText>
            </LogoLink>
            <BrandDescription>{content.description}</BrandDescription>
            <SupportLine>
              <strong>{content.supportLabel}</strong>{' '}
              <SupportEmail href={`mailto:${SUPPORT_EMAIL}`}>
                {SUPPORT_EMAIL}
              </SupportEmail>
            </SupportLine>
          </BrandColumn>

          {FOOTER_COLUMNS.map((column) => (
            <Box key={column.title}>
              <ColumnTitle>{column.title}</ColumnTitle>
              <NavList>
                {column.links.map((link) => (
                  <FooterLink
                    key={link.label}
                    href={link.href}
                    {...(link.external && {
                      target: '_blank',
                      rel: 'noopener noreferrer',
                    })}
                  >
                    {link.label}
                  </FooterLink>
                ))}
              </NavList>
            </Box>
          ))}
        </FooterGrid>

        <BottomBar>
          <Typography component="p">{content.copyright}</Typography>
          <SocialLinks>
            {SOCIAL_LINKS.map((social) => {
              const IconComponent = social.icon;
              return (
                <SocialButton
                  key={social.label}
                  aria-label={social.label}
                  href={social.href}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  <IconComponent />
                </SocialButton>
              );
            })}
          </SocialLinks>
        </BottomBar>
      </ContentContainer>
    </PageSection>
  );
}

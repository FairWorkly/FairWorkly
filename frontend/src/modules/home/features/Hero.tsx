import { Link as RouterLink } from 'react-router-dom'
import Box from '@mui/material/Box'
import Typography from '@mui/material/Typography'
import Stack from '@mui/material/Stack'
import { alpha } from '@mui/material/styles'
import ArrowForwardIcon from '@mui/icons-material/ArrowForward'
import VerifiedIcon from '@mui/icons-material/Verified'
import { styled } from '@/styles/styled'

const HeroSection = styled(Box)(({ theme }) => ({
  paddingTop: theme.spacing(18),
  paddingBottom: theme.spacing(10),
  position: 'relative',
}))

const HeroContainer = styled(Box)(({ theme }) => ({
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  padding: theme.spacing(0, 4),
}))

const HeroGrid = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '1fr 1fr',
  gap: theme.spacing(8),
  alignItems: 'center',
  [theme.breakpoints.down('lg')]: {
    gridTemplateColumns: '1fr',
    textAlign: 'center',
  },
}))

const HeroContent = styled(Box)(({ theme }) => ({
  position: 'relative',
  zIndex: 2,
  [theme.breakpoints.down('lg')]: {
    order: 1,
  },
}))

const HeroBadge = styled(Box)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  padding: theme.spacing(1, 2),
  background: theme.palette.background.paper,
  border: `1px solid ${theme.palette.divider}`,
  borderRadius: theme.fairworkly.radius.pill,
  marginBottom: theme.spacing(3),
  boxShadow: theme.fairworkly.shadow.sm,
}))

const PulseDot = styled(Box)(({ theme }) => ({
  width: theme.spacing(1),
  height: theme.spacing(1),
  background: theme.palette.success.main,
  borderRadius: theme.fairworkly.radius.pill,
  animation: 'pulse 2000ms infinite',
  '@keyframes pulse': {
    '0%, 100%': { opacity: 1, transform: 'scale(1)' },
    '50%': { opacity: 0.7, transform: 'scale(1.2)' },
  },
}))

const HeroTitle = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(3),
  [theme.breakpoints.down('lg')]: {
    fontSize: theme.typography.h2.fontSize,
  },
  [theme.breakpoints.down('md')]: {
    fontSize: theme.typography.h3.fontSize,
  },
}))

const Highlight = styled(Box)(({ theme }) => ({
  background: theme.fairworkly.gradient.primary,
  WebkitBackgroundClip: 'text',
  WebkitTextFillColor: 'transparent',
  backgroundClip: 'text',
  display: 'inline',
}))

const HeroSubtitle = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(4),
}))

const HeroActions = styled(Stack)(({ theme }) => ({
  marginBottom: theme.spacing(5),
  [theme.breakpoints.down('lg')]: {
    justifyContent: 'center',
  },
  [theme.breakpoints.down('sm')]: {
    flexDirection: 'column',
    alignItems: 'stretch',
    width: '100%',
    maxWidth: theme.spacing(40),
    margin: '0 auto',
    marginBottom: theme.spacing(5),
  },
}))

const PrimaryLink = styled(RouterLink)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  gap: theme.spacing(1),
  padding: theme.spacing(2, 4),
  borderRadius: theme.shape.borderRadius,
  textDecoration: 'none',
  boxSizing: 'border-box',
  ...theme.typography.button,
  background: theme.fairworkly.gradient.primary,
  color: theme.palette.common.white,
  boxShadow: theme.fairworkly.shadow.primaryButton,
  transition: theme.transitions.create(['box-shadow']),
  '&:hover': {
    boxShadow: theme.fairworkly.shadow.primaryButtonHover,
  },
  [theme.breakpoints.down('sm')]: {
    width: '100%',
  },
}))

const SecondaryButton = styled('button')(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  gap: theme.spacing(1),
  padding: theme.spacing(2, 4),
  borderRadius: theme.shape.borderRadius,
  boxSizing: 'border-box',
  ...theme.typography.button,
  background: theme.fairworkly.effect.primaryGlow,
  color: theme.palette.primary.main,
  border: `1px solid ${theme.fairworkly.effect.primaryBorder}`,
  appearance: 'none',
  cursor: 'pointer',
  transition: theme.transitions.create(['background', 'border-color']),
  '&:hover': {
    background: theme.fairworkly.effect.primaryGlowHover,
    borderColor: theme.palette.primary.main,
  },
  [theme.breakpoints.down('sm')]: {
    width: '100%',
  },
}))

const HeroImageWrapper = styled(Box)(({ theme }) => ({
  position: 'relative',
  zIndex: 1,
  [theme.breakpoints.down('lg')]: {
    order: 0,
    maxWidth: theme.breakpoints.values.sm,
    margin: theme.spacing(0, 'auto', 4),
  },
}))

const HeroImage = styled('img')(({ theme }) => ({
  width: '100%',
  borderRadius: theme.fairworkly.radius.xl,
  boxShadow: theme.fairworkly.shadow.xl,
}))

const ImageBadge = styled(Box)(({ theme }) => ({
  position: 'absolute',
  bottom: theme.spacing(-2.5),
  left: theme.spacing(-2.5),
  background: theme.palette.background.paper,
  borderRadius: theme.fairworkly.radius.lg,
  padding: theme.spacing(2, 2.5),
  boxShadow: theme.fairworkly.shadow.lg,
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  [theme.breakpoints.down('md')]: {
    bottom: theme.spacing(-1),
    left: theme.spacing(1),
  },
}))

const BadgeIcon = styled(Box)(({ theme }) => ({
  width: theme.spacing(5.5),
  height: theme.spacing(5.5),
  borderRadius: theme.fairworkly.radius.sm,
  background: alpha(theme.palette.success.main, 0.1),
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.success.main,
}))

const BadgeContent = styled(Stack)({})

const BadgeTitle = styled(Typography)({})

const BadgeSubtitle = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.disabled,
}))

export function Hero() {
  const scrollToPricing = () => {
    document.getElementById('pricing')?.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }

  return (
    <HeroSection>
      <HeroContainer>
        <HeroGrid>
          <HeroContent>
            <HeroBadge>
              <PulseDot />
              <Typography variant="body2" color="text.secondary">
                Now supporting Retail, Hospitality & Clerks Awards
              </Typography>
            </HeroBadge>
            <HeroTitle variant="h1">
              Fair Work Compliance <Highlight>Made Simple</Highlight>
            </HeroTitle>
            <HeroSubtitle variant="body1">
              Upload payslip and roster CSV from any system. Get instant Fair
              Work validation. Avoid $50K+ penalties with AI-powered compliance
              checking.
            </HeroSubtitle>
            <HeroActions direction="row" spacing={2}>
              <PrimaryLink to="/login?signup=true">
                Start Free Trial
                <ArrowForwardIcon />
              </PrimaryLink>
              <SecondaryButton type="button" onClick={scrollToPricing}>
                View Pricing
              </SecondaryButton>
            </HeroActions>
          </HeroContent>

          <HeroImageWrapper>
            <HeroImage
              src="https://images.unsplash.com/photo-1556761175-5973dc0f32e7?w=800&q=80"
              alt="Australian small business team"
            />
            <ImageBadge>
              <BadgeIcon>
                <VerifiedIcon />
              </BadgeIcon>
              <BadgeContent>
                <BadgeTitle variant="subtitle1">Compliant</BadgeTitle>
                <BadgeSubtitle variant="caption">All checks passed</BadgeSubtitle>
              </BadgeContent>
            </ImageBadge>
          </HeroImageWrapper>
        </HeroGrid>
      </HeroContainer>
    </HeroSection>
  )
}

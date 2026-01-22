import { alpha } from '@mui/material/styles'
import Box from '@mui/material/Box'
import Stack from '@mui/material/Stack'
import Typography from '@mui/material/Typography'
import ScheduleIcon from '@mui/icons-material/Schedule'
import VerifiedIcon from '@mui/icons-material/Verified'
import SmartToyIcon from '@mui/icons-material/SmartToy'
import SupportAgentIcon from '@mui/icons-material/SupportAgent'
import BoltIcon from '@mui/icons-material/Bolt'
import { styled } from '@/styles/styled'

const LogoSection = styled(Stack)(({ theme }) => ({
  flexDirection: 'row',
  alignItems: 'center',
  gap: theme.spacing(0.75),
}))

const LogoIcon = styled(Box)(({ theme }) => ({
  width: theme.spacing(6),
  height: theme.spacing(6),
  borderRadius: theme.shape.borderRadius,
  background: theme.fairworkly.gradient.primary,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.common.white,
}))

const LogoIconContent = styled(BoltIcon)(({ theme }) => ({
  fontSize: theme.typography.h3.fontSize,
}))

const LogoText = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h3.fontSize,
  fontWeight: theme.typography.h3.fontWeight,
  background: theme.fairworkly.gradient.brandText,
  backgroundClip: 'text',
  WebkitBackgroundClip: 'text',
  WebkitTextFillColor: 'transparent',
}))

const BrandingContent = styled(Box)(() => ({
  position: 'relative',
  zIndex: 1,
}))

const BrandingTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h2.fontSize,
  fontWeight: theme.typography.h1.fontWeight,
  lineHeight: theme.typography.h2.lineHeight,
  marginBottom: theme.spacing(1),
  marginTop: 0,
  color: theme.palette.common.white,
}))

const TitleHighlight = styled('span')(({ theme }) => ({
  background: theme.fairworkly.gradient.primary,
  backgroundClip: 'text',
  WebkitBackgroundClip: 'text',
  WebkitTextFillColor: 'transparent',
}))

const BrandingSubtitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  fontWeight: theme.typography.body1.fontWeight,
  color: alpha(theme.palette.common.white, 0.7),
  lineHeight: theme.typography.body1.lineHeight,
  maxWidth: theme.spacing(45),
  margin: 0,
}))

const FeatureList = styled(Stack)(({ theme }) => ({
  listStyle: 'none',
  padding: 0,
  margin: 0,
  marginTop: theme.spacing(1.5),
}))

const FeatureItem = styled(Stack)(({ theme }) => ({
  flexDirection: 'row',
  alignItems: 'center',
  gap: theme.spacing(0.75),
  paddingTop: theme.spacing(0.625),
  paddingBottom: theme.spacing(0.625),
}))

const FeatureText = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  color: alpha(theme.palette.common.white, 0.8),
}))

const FeatureIcon = styled(Box)(({ theme }) => ({
  width: theme.spacing(4.5),
  height: theme.spacing(4.5),
  borderRadius: theme.spacing(1),
  background: alpha(theme.palette.primary.main, 0.2),
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.fairworkly.color.brandLight,
  flexShrink: 0,
}))

const FeatureIconContent = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.h6.fontSize,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
}))

const features = [
  { icon: ScheduleIcon, text: '2-minute validation' },
  { icon: VerifiedIcon, text: '3 core Modern Awards' },
  { icon: SmartToyIcon, text: '3 AI compliance agents' },
  { icon: SupportAgentIcon, text: 'Melbourne-based support' },
]

export function AuthBranding() {
  return (
    <>
      <LogoSection>
        <LogoIcon>
          <LogoIconContent />
        </LogoIcon>
        <LogoText>FairWorkly</LogoText>
      </LogoSection>

      <BrandingContent>
        <BrandingTitle>
          AI Compliance
          <br />
          Made <TitleHighlight>Simple</TitleHighlight>
        </BrandingTitle>
        <BrandingSubtitle>
          Join hundreds of Australian SMEs using FairWorkly to stay compliant
          with Modern Awards.
        </BrandingSubtitle>

        <FeatureList>
          {features.map(({ icon: Icon, text }) => (
            <FeatureItem key={text}>
              <FeatureIcon>
                <FeatureIconContent>
                  <Icon fontSize="inherit" />
                </FeatureIconContent>
              </FeatureIcon>
              <FeatureText>{text}</FeatureText>
            </FeatureItem>
          ))}
        </FeatureList>
      </BrandingContent>
    </>
  )
}

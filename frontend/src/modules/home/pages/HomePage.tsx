import Box from '@mui/material/Box'
import Typography from '@mui/material/Typography'
import { styled } from '@/styles/styled'
import { Navbar } from '../features/Navbar'
import { Hero } from '../features/Hero'
import { TrustBar } from '../features/TrustBar'

const HomePageRoot = styled(Box)({
  minHeight: '100vh',
})

const HomePageBackground = styled(Box)(({ theme }) => ({
  position: 'fixed',
  top: 0,
  left: 0,
  width: '100%',
  height: '100%',
  backgroundImage: `
    linear-gradient(${theme.fairworkly.effect.gridLine} 1px, transparent 1px),
    linear-gradient(90deg, ${theme.fairworkly.effect.gridLine} 1px, transparent 1px) `,
  backgroundSize: '60px 60px',
  zIndex: -1,
  pointerEvents: 'none',
}))

const HomePageSection = styled(Box)(({ theme }) => ({
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  padding: theme.spacing(10, 4),
  scrollMarginTop: theme.spacing(12),
}))

const HomePageSectionHeading = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(1),
}))

const HomePageSectionBody = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
}))

export function HomePage() {
  return (
    <HomePageRoot>
      <HomePageBackground />
      <Navbar />
      <Hero />
      <TrustBar />
      <HomePageSection id="features">
        <HomePageSectionHeading variant="h4">Features</HomePageSectionHeading>
        <HomePageSectionBody variant="body1">
          Coming soon...
        </HomePageSectionBody>
      </HomePageSection>
      <HomePageSection id="pricing">
        <HomePageSectionHeading variant="h4">Pricing</HomePageSectionHeading>
        <HomePageSectionBody variant="body1">
          Coming soon...
        </HomePageSectionBody>
      </HomePageSection>
      <HomePageSection id="faq">
        <HomePageSectionHeading variant="h4">FAQ</HomePageSectionHeading>
        <HomePageSectionBody variant="body1">
          Coming soon...
        </HomePageSectionBody>
      </HomePageSection>
    </HomePageRoot>
  )
}

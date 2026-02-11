import Box from '@mui/material/Box'
import { styled } from '@/styles/styled'
import { Navbar } from '../features/Navbar'
import { Hero } from '../features/Hero'
import { TrustBar } from '../features/TrustBar'
import { FeaturesSection } from '../components/FeaturesSection'
import { PricingSection } from '../components/PricingSection'
import { FaqSection } from '../components/FaqSection'
import { Footer } from '../components/Footer'

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

export function HomePage() {
  return (
    <HomePageRoot>
      <HomePageBackground />
      <Navbar />
      <Hero />
      <TrustBar />
      <FeaturesSection />
      <PricingSection />
      <FaqSection />
      <Footer />
    </HomePageRoot>
  )
}

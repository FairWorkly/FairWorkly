import { useState, useEffect } from 'react'
import { Link as RouterLink } from 'react-router-dom'
import Box from '@mui/material/Box'
import Stack from '@mui/material/Stack'
import Menu from '@mui/material/Menu'
import MenuItem from '@mui/material/MenuItem'
import IconButton from '@mui/material/IconButton'
import { alpha } from '@mui/material/styles'
import BoltIcon from '@mui/icons-material/Bolt'
import ArrowForwardIcon from '@mui/icons-material/ArrowForward'
import MenuIcon from '@mui/icons-material/Menu'
import { styled } from '@/styles/styled'

const NavRoot = styled(Box)<{ scrolled: boolean }>(({ theme, scrolled }) => ({
  position: 'fixed',
  top: 0,
  left: 0,
  right: 0,
  zIndex: theme.zIndex.appBar,
  padding: scrolled ? theme.spacing(1.25, 0) : theme.spacing(1.75, 0),
  background: theme.fairworkly.surface.navDark,
  transition: theme.transitions.create(['padding', 'box-shadow'], {
    duration: theme.transitions.duration.short,
  }),
  ...(scrolled && {
    boxShadow: theme.fairworkly.shadow.navScrolled,
  }),
}))

const NavContainer = styled(Box)(({ theme }) => ({
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  padding: theme.spacing(0, 4),
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'space-between',
  position: 'relative',
  [theme.breakpoints.down('sm')]: {
    padding: theme.spacing(0, 2),
  },
}))

const LogoLink = styled(RouterLink)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  textDecoration: 'none',
}))

const LogoIcon = styled(Box)(({ theme }) => ({
  width: theme.spacing(5),
  height: theme.spacing(5),
  background: theme.fairworkly.gradient.primary,
  borderRadius: theme.fairworkly.radius.sm,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.common.white,
  flexShrink: 0,
  [theme.breakpoints.down('sm')]: {
    width: theme.spacing(4),
    height: theme.spacing(4),
  },
}))

const LogoText = styled(Box)(({ theme }) => ({
  ...theme.typography.h4,
  background: theme.fairworkly.gradient.brandText,
  WebkitBackgroundClip: 'text',
  WebkitTextFillColor: 'transparent',
  backgroundClip: 'text',
  [theme.breakpoints.down('sm')]: {
    fontSize: theme.typography.h6.fontSize,
  },
}))

const NavList = styled(Stack)(({ theme }) => ({
  position: 'absolute',
  left: '50%',
  transform: 'translateX(-50%)',
  [theme.breakpoints.down('md')]: {
    display: 'none',
  },
}))

const NavLink = styled(Box)(({ theme }) => ({
  color: alpha(theme.palette.common.white, 0.85),
  textDecoration: 'none',
  ...theme.typography.body1,
  fontWeight: theme.typography.fontWeightMedium,
  transition: theme.transitions.create('color'),
  cursor: 'pointer',
  '&:hover': {
    color: theme.fairworkly.color.brandLight,
  },
}))

const NavActions = styled(Stack)(({ theme }) => ({
  flexShrink: 0,
  [theme.breakpoints.down('sm')]: {
    gap: theme.spacing(1),
  },
}))

const MobileMenuButton = styled(IconButton)(({ theme }) => ({
  color: alpha(theme.palette.common.white, 0.9),
  background: alpha(theme.palette.common.white, 0.08),
  marginLeft: theme.spacing(1),
  '&:hover': {
    background: alpha(theme.palette.common.white, 0.18),
  },
  [theme.breakpoints.up('md')]: {
    display: 'none',
  },
}))

const GhostLink = styled(RouterLink)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(1.25, 2),
  borderRadius: theme.shape.borderRadius,
  textDecoration: 'none',
  whiteSpace: 'nowrap',
  ...theme.typography.button,
  background: alpha(theme.palette.common.white, 0.1),
  color: alpha(theme.palette.common.white, 0.9),
  transition: theme.transitions.create(['background', 'color']),
  '&:hover': {
    background: alpha(theme.palette.common.white, 0.2),
    color: theme.palette.common.white,
  },
  [theme.breakpoints.down('sm')]: {
    padding: theme.spacing(1, 1.5),
    fontSize: theme.typography.caption.fontSize,
  },
}))

const PrimaryLink = styled(RouterLink)(({ theme }) => ({
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  gap: theme.spacing(1),
  padding: theme.spacing(1.25, 2),
  borderRadius: theme.shape.borderRadius,
  textDecoration: 'none',
  whiteSpace: 'nowrap',
  ...theme.typography.button,
  background: theme.fairworkly.gradient.primary,
  color: theme.palette.common.white,
  boxShadow: theme.fairworkly.shadow.primaryButton,
  transition: theme.transitions.create(['box-shadow']),
  '&:hover': {
    boxShadow: theme.fairworkly.shadow.primaryButtonHover,
  },
  [theme.breakpoints.down('sm')]: {
    padding: theme.spacing(1, 1.5),
    fontSize: theme.typography.caption.fontSize,
  },
}))

interface NavbarProps {
  onScrollToSection?: (sectionId: string) => void
}

export function Navbar({ onScrollToSection }: NavbarProps) {
  const [scrolled, setScrolled] = useState(false)
  const [menuAnchorEl, setMenuAnchorEl] = useState<null | HTMLElement>(null)
  const isMenuOpen = Boolean(menuAnchorEl)

  useEffect(() => {
    const SCROLL_THRESHOLD = 50
    const handleScroll = () => {
      setScrolled(window.scrollY > SCROLL_THRESHOLD)
    }
    window.addEventListener('scroll', handleScroll)
    return () => window.removeEventListener('scroll', handleScroll)
  }, [])

  const handleNavClick = (sectionId: string) => {
    if (onScrollToSection) {
      onScrollToSection(sectionId)
    } else {
      document.getElementById(sectionId)?.scrollIntoView({ behavior: 'smooth', block: 'start' })
    }
  }

  const handleOpenMenu = (event: React.MouseEvent<HTMLElement>) => {
    setMenuAnchorEl(event.currentTarget)
  }

  const handleCloseMenu = () => {
    setMenuAnchorEl(null)
  }

  const handleMobileNavClick = (sectionId: string) => {
    handleNavClick(sectionId)
    handleCloseMenu()
  }

  return (
    <NavRoot scrolled={scrolled}>
      <NavContainer>
        <LogoLink to="/">
          <LogoIcon>
            <BoltIcon />
          </LogoIcon>
          <LogoText>FairWorkly</LogoText>
        </LogoLink>

        <NavList direction="row" spacing={5}>
          <NavLink onClick={() => handleNavClick('features')}>Features</NavLink>
          <NavLink onClick={() => handleNavClick('pricing')}>Pricing</NavLink>
          <NavLink onClick={() => handleNavClick('faq')}>FAQ</NavLink>
        </NavList>

        <NavActions direction="row" spacing={2}>
          <GhostLink to="/login">Log In</GhostLink>
          <PrimaryLink to="/login?signup=true">
            Start Free Trial
            <ArrowForwardIcon />
          </PrimaryLink>
          <MobileMenuButton
            aria-label="Open navigation menu"
            aria-controls={isMenuOpen ? 'home-nav-menu' : undefined}
            aria-haspopup="true"
            aria-expanded={isMenuOpen ? 'true' : undefined}
            onClick={handleOpenMenu}
          >
            <MenuIcon />
          </MobileMenuButton>
        </NavActions>
      </NavContainer>
      <Menu
        id="home-nav-menu"
        anchorEl={menuAnchorEl}
        open={isMenuOpen}
        onClose={handleCloseMenu}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        keepMounted
      >
        <MenuItem onClick={() => handleMobileNavClick('features')}>Features</MenuItem>
        <MenuItem onClick={() => handleMobileNavClick('pricing')}>Pricing</MenuItem>
        <MenuItem onClick={() => handleMobileNavClick('faq')}>FAQ</MenuItem>
      </Menu>
    </NavRoot>
  )
}

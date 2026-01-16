import Box from '@mui/material/Box'
import Stack from '@mui/material/Stack'
import Typography from '@mui/material/Typography'
import SecurityIcon from '@mui/icons-material/Security'
import LocationOnIcon from '@mui/icons-material/LocationOn'
import GavelIcon from '@mui/icons-material/Gavel'
import SupportAgentIcon from '@mui/icons-material/SupportAgent'
import { styled } from '@/styles/styled'

const TrustSection = styled(Box)(({ theme }) => ({
  padding: theme.spacing(6, 0),
  background: theme.palette.background.paper,
  borderTop: `1px solid ${theme.palette.divider}`,
  borderBottom: `1px solid ${theme.palette.divider}`,
}))

const TrustContainer = styled(Box)(({ theme }) => ({
  maxWidth: theme.fairworkly.layout.containerMaxWidth,
  margin: '0 auto',
  padding: theme.spacing(0, 4),
}))

const TrustContent = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'center',
  gap: theme.spacing(8),
  [theme.breakpoints.down('md')]: {
    display: 'grid',
    gridTemplateColumns: 'max-content',
    justifyContent: 'center',
    gap: theme.spacing(3),
  },
}))

const TrustItem = styled(Stack)(({ theme }) => ({
  color: theme.palette.text.disabled,
  '& .MuiSvgIcon-root': {
    fontSize: theme.spacing(3),
    color: theme.palette.success.main,
    flexShrink: 0,
  },
}))

const trustItems = [
  { icon: SecurityIcon, label: 'SOC 2 Compliant' },
  { icon: LocationOnIcon, label: 'Australian Hosted' },
  { icon: GavelIcon, label: 'Fair Work Aligned' },
  { icon: SupportAgentIcon, label: 'Local Support' },
]

export function TrustBar() {
  return (
    <TrustSection>
      <TrustContainer>
        <TrustContent>
          {trustItems.map((item) => (
            <TrustItem key={item.label} direction="row" spacing={1.5} alignItems="center">
              <item.icon />
              <Typography variant="body1">{item.label}</Typography>
            </TrustItem>
          ))}
        </TrustContent>
      </TrustContainer>
    </TrustSection>
  )
}

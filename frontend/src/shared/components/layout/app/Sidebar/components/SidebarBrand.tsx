import { Bolt as BoltIcon } from '@mui/icons-material'
import { BrandRow, BrandTitle, LogoBadge } from '../Sidebar.styles'

export function SidebarBrand() {
  return (
    <BrandRow>
      <LogoBadge aria-label="FairWorkly logo">
        <BoltIcon />
      </LogoBadge>

      <BrandTitle variant="h6">FairWorkly</BrandTitle>
    </BrandRow>
  )
}

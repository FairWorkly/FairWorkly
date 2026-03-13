import { VerifiedUser } from '@mui/icons-material'
import { BrandRow, BrandTitle, LogoBadge } from '../Sidebar.styles'

export function SidebarBrand() {
  return (
    <BrandRow>
      <LogoBadge aria-label="FairWorkly logo">
        <VerifiedUser />
      </LogoBadge>

      <BrandTitle variant="h6">FairWorkly</BrandTitle>
    </BrandRow>
  )
}

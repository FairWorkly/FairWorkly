import { useNavigate } from 'react-router-dom'
import { Circle as CircleIcon, SmartToy as SmartToyOutlinedIcon } from '@mui/icons-material'
import {
  FairBotBody,
  FairBotCard as StyledFairBotCard,
  FairBotDescription,
  FairBotHeader,
  FairBotIcon,
  FairBotStatusChip,
  FairBotTitle,
} from '../Sidebar.styles'

export function FairBotCard() {
  const navigate = useNavigate()

  const handleClick = () => {
    navigate('/fairbot')
  }

  return (
    <StyledFairBotCard
      onClick={handleClick}
      role="button"
      tabIndex={0}
      aria-label="Open FairBot AI Assistant"
    >
      <FairBotIcon aria-hidden="true">
        <SmartToyOutlinedIcon />
      </FairBotIcon>

      <FairBotHeader>
        <FairBotTitle variant="subtitle1">FairBot AI</FairBotTitle>
        <FairBotStatusChip
          size="small"
          icon={<CircleIcon />}
          label="ACTIVE"
          aria-label="FairBot status active"
        />
      </FairBotHeader>

      <FairBotBody>
        <FairBotDescription variant="body2" color="text.secondary">
          Ask FairBot about award rules, payroll, rosters, and required employment documents — or
          upload files to start a compliance check.
        </FairBotDescription>
      </FairBotBody>
    </StyledFairBotCard>
  )
}

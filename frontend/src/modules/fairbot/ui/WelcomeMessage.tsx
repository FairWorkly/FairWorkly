import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import CheckCircleOutline from '@mui/icons-material/CheckCircleOutline'
import { FAIRBOT_LABELS } from '../constants/fairbot.constants'

const MessageCard = styled('div')(({ theme }) => ({
  borderRadius: theme.shape.borderRadius,
  padding: theme.spacing(1.5),
  backgroundColor: theme.palette.background.paper,
  border: `1px solid ${theme.palette.divider}`,
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1.5),
}))

const BulletList = styled('ul')(({ theme }) => ({
  margin: 0,
  paddingLeft: theme.spacing(1.5),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
}))

const BulletItem = styled('li')(({ theme }) => ({
  display: 'flex',
  alignItems: 'flex-start',
  gap: theme.spacing(1),
}))

const BulletIcon = styled(CheckCircleOutline)(({ theme }) => ({
  color: theme.palette.primary.main,
}))

export const WelcomeMessage = () => {
  return (
    <MessageCard>
      <Typography variant="subtitle1">{FAIRBOT_LABELS.WELCOME_TITLE}</Typography>
      <BulletList>
        {FAIRBOT_LABELS.WELCOME_BULLETS.map((item, index) => (
          <BulletItem key={`${item}-${index}`}>
            <BulletIcon fontSize="small" />
            <Typography variant="body2">{item}</Typography>
          </BulletItem>
        ))}
      </BulletList>
      <Typography variant="body2">{FAIRBOT_LABELS.PROMPT_QUESTION}</Typography>
    </MessageCard>
  )
}

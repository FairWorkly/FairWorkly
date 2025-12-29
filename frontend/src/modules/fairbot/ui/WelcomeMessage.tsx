import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import CheckCircleOutline from '@mui/icons-material/CheckCircleOutline'
import { FAIRBOT_LABELS, FAIRBOT_LAYOUT } from '../constants/fairbot.constants'

const MessageCard = styled('div')(({ theme }) => ({
  borderRadius: theme.shape.borderRadius,
  padding: `${FAIRBOT_LAYOUT.MESSAGE_SECTION_GAP}px`,
  backgroundColor: theme.palette.background.paper,
  border: `1px solid ${theme.palette.divider}`,
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_SECTION_GAP}px`,
}))

const BulletList = styled('ul')({
  margin: 0,
  paddingLeft: `${FAIRBOT_LAYOUT.MESSAGE_SECTION_GAP}px`,
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_STACK_GAP}px`,
})

const BulletItem = styled('li')({
  display: 'flex',
  alignItems: 'flex-start',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_STACK_GAP}px`,
})

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

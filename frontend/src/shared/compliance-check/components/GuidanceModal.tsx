import React from 'react'
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Stack,
  alpha,
  styled,
} from '@mui/material'
import LightbulbOutlinedIcon from '@mui/icons-material/LightbulbOutlined'

const ModalTitle = styled(DialogTitle)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1.5),
  paddingBottom: theme.spacing(1),
}))

const TitleIconBox = styled(Box)(({ theme }) => ({
  width: theme.spacing(5),
  height: theme.spacing(5),
  backgroundColor: alpha(theme.palette.primary.main, 0.1),
  borderRadius: theme.fairworkly.radius.sm,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.primary.main,
}))

const ModalContent = styled(DialogContent)(({ theme }) => ({
  paddingTop: theme.spacing(2),
}))

const Description = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(3),
  fontWeight: theme.typography.body1.fontWeight,
  color: theme.palette.text.primary,
}))

const StepsList = styled(Stack)(({ theme }) => ({
  marginBottom: theme.spacing(3),
}))

const StepItem = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2),
  alignItems: 'flex-start',
}))

const StepBullet = styled(Box)(({ theme }) => ({
  marginTop: theme.spacing(0.5),
  minWidth: theme.spacing(0.75),
  height: theme.spacing(0.75),
  backgroundColor: theme.palette.primary.main,
  borderRadius: theme.fairworkly.radius.pill,
}))

const StepText = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.body2.fontWeight,
  lineHeight: 1.6,
}))

const AfterNote = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(2),
  fontWeight: theme.typography.caption.fontWeight,
  color: theme.palette.text.primary,
}))

const ModalActions = styled(DialogActions)(({ theme }) => ({
  padding: theme.spacing(3),
  paddingTop: 0,
}))

const CloseButton = styled(Button)(({ theme }) => ({
  borderColor: theme.palette.divider,
  color: theme.palette.text.secondary,
  fontWeight: theme.typography.button.fontWeight,
  paddingLeft: theme.spacing(4),
  paddingRight: theme.spacing(4),
  '&:hover': {
    borderColor: theme.palette.text.disabled,
    backgroundColor: theme.palette.background.default,
  },
}))

interface GuidanceModalProps {
  open: boolean
  onClose: () => void
  title?: string
  description?: string
  steps?: string[]
  afterStepsNote?: string
  footnote?: string
}

const defaultSteps = [
  "Review the affected employee's pay settings in your payroll system",
  'Ensure the correct Award and classification are applied',
  'Confirm that pay rates and penalty settings match the applicable Award',
]

export const GuidanceModal: React.FC<GuidanceModalProps> = ({
  open,
  onClose,
  title = 'How to Fix',
  description = 'This guidance provides general steps to help you review and correct the issue.',
  steps = defaultSteps,
  afterStepsNote = 'After making updates, re-run payroll and upload the updated data to verify the issue is resolved.',
  footnote = 'Steps may vary depending on your payroll system.',
}) => {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <ModalTitle>
        <TitleIconBox>
          <LightbulbOutlinedIcon fontSize="small" />
        </TitleIconBox>
        <Typography variant="h6">{title}</Typography>
      </ModalTitle>
      <ModalContent>
        <Description variant="body1">{description}</Description>

        <StepsList spacing={2}>
          {steps.map((step, i) => (
            <StepItem key={i}>
              <StepBullet />
              <StepText variant="body2" color="text.secondary">
                {step}
              </StepText>
            </StepItem>
          ))}
        </StepsList>

        <AfterNote variant="body2">{afterStepsNote}</AfterNote>

        <Typography variant="caption" color="text.disabled">
          {footnote}
        </Typography>
      </ModalContent>
      <ModalActions>
        <CloseButton variant="outlined" onClick={onClose}>
          Close
        </CloseButton>
      </ModalActions>
    </Dialog>
  )
}

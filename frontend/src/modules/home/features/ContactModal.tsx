import React from 'react'
import {
  Dialog,
  Box,
  Typography,
  IconButton,
  styled,
  alpha,
} from '@mui/material'
import { Close, MailOutline } from '@mui/icons-material'

const content = {
  title: 'Contact Sales',
  description: "Interested in Enterprise? Let's talk!",
  label: 'Email us at:',
  email: 'support@fairworkly.com',
  note: 'We typically respond within 24 hours.',
}

const StyledDialog = styled(Dialog)(({ theme }) => ({
  '& .MuiBackdrop-root': {
    backgroundColor: alpha(theme.palette.common.black, 0.6),
  },
  '& .MuiDialog-paper': {
    borderRadius: theme.spacing(3),
    maxWidth: theme.spacing(62.5),
    boxShadow: theme.fairworkly.shadow.xl,
    overflow: 'visible',
  },
}))

const CloseButton = styled(IconButton)(({ theme }) => ({
  position: 'absolute',
  right: theme.spacing(1.5),
  top: theme.spacing(1.5),
  color: theme.palette.text.disabled,
  transition: theme.transitions.create(['color'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    color: theme.palette.text.primary,
    backgroundColor: 'transparent',
  },
}))

const ModalHeader = styled(Box)(({ theme }) => ({
  textAlign: 'center',
  padding: theme.spacing(6, 4, 2),
}))

const MailIcon = styled(MailOutline)(({ theme }) => ({
  fontSize: theme.spacing(6),
  color: theme.palette.primary.main,
  marginBottom: theme.spacing(2),
}))

const ModalTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h3.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  color: theme.palette.text.primary,
}))

const ModalBody = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2, 5, 6),
  textAlign: 'center',
}))

const ModalDescription = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(3),
  lineHeight: theme.typography.body1.lineHeight,
  fontSize: theme.typography.body1.fontSize,
}))

const ContactInfoBox = styled(Box)(({ theme }) => ({
  background: theme.palette.background.default,
  padding: theme.spacing(3),
  borderRadius: theme.spacing(2),
  marginBottom: theme.spacing(2),
}))

const ContactLabel = styled(Typography)(({ theme }) => ({
  display: 'block',
  marginBottom: theme.spacing(1.5),
  color: theme.palette.text.primary,
  fontSize: theme.typography.body1.fontSize,
  fontWeight: theme.typography.fontWeightSemiBold,
}))

const EmailLink = styled('a')(({ theme }) => ({
  display: 'inline-block',
  fontSize: theme.typography.h5.fontSize,
  fontWeight: theme.typography.fontWeightSemiBold,
  color: theme.palette.primary.main,
  textDecoration: 'none',
  transition: theme.transitions.create(['color', 'transform'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    color: theme.palette.primary.dark,
    transform: `translateY(${theme.spacing(-0.25)})`,
  },
}))

const ModalNote = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  color: theme.palette.text.disabled,
  marginBottom: 0,
}))

interface ContactModalProps {
  open: boolean
  onClose: () => void
}

export const ContactModal: React.FC<ContactModalProps> = ({
  open,
  onClose,
}) => {
  return (
    <StyledDialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <CloseButton onClick={onClose} aria-label="close">
        <Close />
      </CloseButton>

      <ModalHeader>
        <MailIcon />
        <ModalTitle>{content.title}</ModalTitle>
      </ModalHeader>

      <ModalBody>
        <ModalDescription>{content.description}</ModalDescription>

        <ContactInfoBox>
          <ContactLabel>{content.label}</ContactLabel>
          <EmailLink href={`mailto:${content.email}`}>
            {content.email}
          </EmailLink>
        </ContactInfoBox>

        <ModalNote>{content.note}</ModalNote>
      </ModalBody>
    </StyledDialog>
  )
}

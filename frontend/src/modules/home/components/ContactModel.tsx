import React from 'react';
import {
    Dialog,
    Box,
    Typography,
    IconButton,
    styled,
} from '@mui/material';
import { Close, MailOutline } from '@mui/icons-material';


const StyledDialog = styled(Dialog)(({ theme }) => ({
    '& .MuiBackdrop-root': {
        backgroundColor: 'rgba(0, 0, 0, 0.6)',
    },
    '& .MuiDialog-paper': {
        borderRadius: theme.spacing(3),
        maxWidth: '500px',
        boxShadow: theme.fairworkly.shadow.xl,
        overflow: 'visible',
    },
}));

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
}));

const ModalHeader = styled(Box)(({ theme }) => ({
    textAlign: 'center',
    padding: theme.spacing(6, 4, 2),
}));

const MailIcon = styled(MailOutline)(({ theme }) => ({
    fontSize: theme.spacing(6),
    color: theme.palette.primary.main,
    marginBottom: theme.spacing(2),
}));

const ModalTitle = styled(Typography)(({ theme }) => ({
    fontSize: theme.typography.h3.fontSize,
    fontWeight: 700,
    color: theme.palette.text.primary,
}));

const ModalBody = styled(Box)(({ theme }) => ({
    padding: theme.spacing(2, 5, 6),
    textAlign: 'center',
}));

const ModalDescription = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
    marginBottom: theme.spacing(3),
    lineHeight: theme.typography.body1.lineHeight,
    fontSize: theme.typography.body1.fontSize,
}));

const ContactInfoBox = styled(Box)(({ theme }) => ({
    background: theme.palette.background.default,
    padding: theme.spacing(3),
    borderRadius: theme.spacing(2),
    marginBottom: theme.spacing(2),
}));

const ContactLabel = styled(Typography)(({ theme }) => ({
    display: 'block',
    marginBottom: theme.spacing(1.5),
    color: theme.palette.text.primary,
    fontSize: theme.typography.body1.fontSize,
    fontWeight: 600,
}));

const EmailLink = styled('a')(({ theme }) => ({
    display: 'inline-block',
    fontSize: theme.typography.h5.fontSize,
    fontWeight: 600,
    color: theme.palette.primary.main,
    textDecoration: 'none',
    transition: theme.transitions.create(['color', 'transform'], {
        duration: theme.transitions.duration.short,
    }),
    '&:hover': {
        color: theme.palette.primary.dark,
        transform: 'translateY(-2px)',
    },
}));

const ModalNote = styled(Typography)(({ theme }) => ({
    fontSize: theme.typography.body2.fontSize,
    color: theme.palette.text.disabled,
    marginBottom: 0,
}));



interface ContactModalProps {
    open: boolean;
    onClose: () => void;
}

export const ContactModal: React.FC<ContactModalProps> = ({ open, onClose }) => {
    return (
        <StyledDialog
            open={open}
            onClose={onClose}
            maxWidth="sm"
            fullWidth
        >
            <CloseButton onClick={onClose} aria-label="close">
                <Close />
            </CloseButton>

            <ModalHeader>
                <MailIcon />
                <ModalTitle>Contact Sales</ModalTitle>
            </ModalHeader>

            <ModalBody>
                <ModalDescription>
                    Interested in Enterprise? Let's talk!
                </ModalDescription>

                <ContactInfoBox>
                    <ContactLabel>Email us at:</ContactLabel>
                    <EmailLink href="mailto:support@fairworkly.com">
                        support@fairworkly.com
                    </EmailLink>
                </ContactInfoBox>

                <ModalNote>
                    We typically respond within 24 hours.
                </ModalNote>
            </ModalBody>
        </StyledDialog>
    );
};
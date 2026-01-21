import React from 'react';
import {
  Box,
  Container,
  Typography,
  Card,
  Chip,
  Link as MuiLink,
  styled,
} from '@mui/material';
import {
  HelpOutline,
  Computer,
  EventOutlined,
  VerifiedOutlined,
  ScheduleOutlined,
  DescriptionOutlined,
  BuildOutlined,
  Gavel,
  CloudDownloadOutlined,
} from '@mui/icons-material';

const FaqContainer = styled('section')(({ theme }) => ({
  padding: theme.spacing(12, 0),
  backgroundColor: theme.palette.background.default,
}));

const SectionHeader = styled(Box)(({ theme }) => ({
  textAlign: 'center',
  marginBottom: theme.spacing(8),
}));

const SectionLabel = styled(Chip)(({ theme }) => ({
  marginBottom: theme.spacing(2),
  padding: theme.spacing(0.5, 2),
  fontSize: theme.typography.caption.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  textTransform: 'uppercase',
  letterSpacing: theme.typography.caption.letterSpacing,
  backgroundColor: theme.fairworkly.effect.primaryGlow,
  color: theme.palette.primary.main,
  borderRadius: theme.fairworkly.radius.pill,
  '& .MuiChip-icon': {
    color: theme.palette.primary.main,
  },
}));

const SectionTitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.h2.fontSize,
  fontWeight: theme.typography.h2.fontWeight,
  marginBottom: theme.spacing(2),
  color: theme.palette.text.primary,
  [theme.breakpoints.down('sm')]: {
    fontSize: theme.typography.h3.fontSize,
  },
}));

const SectionSubtitle = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body1.fontSize,
  color: theme.palette.text.secondary,
  maxWidth: '600px',
  margin: '0 auto',
}));

const FaqGrid = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: 'repeat(2, 1fr)',
  gap: theme.spacing(4),
  [theme.breakpoints.down('md')]: {
    gridTemplateColumns: '1fr',
  },
}));

const FaqCard = styled(Card)(({ theme }) => ({
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.fairworkly.radius.lg,
  padding: theme.spacing(4),
  border: `1px solid ${theme.palette.divider}`,
  transition: theme.transitions.create(['all'], {
    duration: theme.transitions.duration.standard,
  }),
  '&:hover': {
    borderColor: theme.palette.primary.main,
    boxShadow: theme.fairworkly.shadow.md,
  },
}));

const FaqQuestion = styled(Box)(({ theme }) => ({
  fontSize: theme.typography.h6.fontSize,
  fontWeight: theme.typography.fontWeightBold,
  color: theme.palette.text.primary,
  marginBottom: theme.spacing(2),
  display: 'flex',
  alignItems: 'flex-start',
  gap: theme.spacing(1.5),
}));

const FaqAnswer = styled(Typography)(({ theme }) => ({
  fontSize: theme.typography.body2.fontSize,
  lineHeight: 1.7,
  color: theme.palette.text.secondary,
  '& strong': {
    color: theme.palette.text.primary,
    fontWeight: theme.typography.fontWeightBold,
  },
}));

const FaqNote = styled(Box)(({ theme }) => ({
  backgroundColor: theme.fairworkly.effect.primaryGlow,
  padding: theme.spacing(2),
  borderRadius: theme.fairworkly.radius.md,
  marginTop: theme.spacing(2),
  fontSize: theme.typography.body2.fontSize,
  borderLeft: `3px solid ${theme.palette.primary.main}`,
  color: theme.palette.text.secondary,
}));

const FaqLink = styled(MuiLink)(({ theme }) => ({
  color: theme.palette.primary.main,
  textDecoration: 'none',
  fontWeight: theme.typography.fontWeightMedium,
  '&:hover': {
    textDecoration: 'underline',
  },
}));

const TemplateButton = styled(MuiLink)(({ theme }) => ({
  display: 'inline-block',
  padding: theme.spacing(1.5, 3),
  backgroundColor: theme.fairworkly.effect.primaryGlow,
  color: theme.palette.primary.main,
  borderRadius: theme.fairworkly.radius.md,
  textDecoration: 'none',
  fontWeight: theme.typography.fontWeightBold,
  transition: theme.transitions.create(['all'], {
    duration: theme.transitions.duration.short,
  }),
  '&:hover': {
    backgroundColor: theme.fairworkly.effect.primaryGlowHover,
  },
}));

interface FaqItem {
  icon: React.ReactNode;
  question: string;
  answer: React.ReactNode;
}

export function FaqSection() {
  const faqs: FaqItem[] = [
    {
      icon: <Computer />,
      question: 'What systems does FairWorkly work with?',
      answer: (
        <>
          <strong>FairWorkly doesn't require integration.</strong> We accept standard CSV files,
          which means you can use any payroll or rostering system that can export CSV — including
          Xero, MYOB, QuickBooks, Keypay, Deputy, Tanda, and many others.
          <FaqNote>
            💡 Simply export your data as CSV and upload. No API keys, no IT setup required.
          </FaqNote>
        </>
      ),
    },
    {
      icon: <EventOutlined />,
      question: 'Do I need rostering software?',
      answer: (
        <>
          No! While we work great with Deputy and Tanda exports, you can also upload roster data
          from Excel. We provide a <strong>free template</strong> that makes it easy to convert
          your existing schedules to CSV format.
        </>
      ),
    },
    {
      icon: <VerifiedOutlined />,
      question: 'Which Awards do you cover?',
      answer: (
        <>
          We currently support the three most common Awards:
          <br />• <strong>Hospitality Industry Award 2020</strong>
          <br />• <strong>General Retail Industry Award</strong>
          <br />• <strong>Clerks Private Sector Award</strong>
        </>
      ),
    },
    {
      icon: <ScheduleOutlined />,
      question: 'How long does validation take?',
      answer: (
        <>
          Most validations complete in <strong>2-3 minutes</strong>. Upload your CSV, grab a
          coffee, and your compliance report will be ready. Larger files (100+ employees) may take
          up to 5 minutes.
        </>
      ),
    },
    {
      icon: <DescriptionOutlined />,
      question: 'Do you generate employment contracts?',
      answer: (
        <>
          No, we don't generate documents in the MVP. Our <strong>Document Agent</strong> helps you{' '}
          <strong>track mandatory documents</strong> like Fair Work Information Statement (FWIS),
          Separation Certificates, and Casual Conversion Notices — ensuring you don't miss critical
          deadlines.
        </>
      ),
    },
    {
      icon: <BuildOutlined />,
      question: 'What if I find issues in my payroll?',
      answer: (
        <>
          We provide <strong>detailed reports</strong> with expected vs actual values and
          step-by-step recommendations. You can then fix issues in your payroll system
          (Xero/MYOB/etc) and re-upload to verify.
          <FaqNote>
            📌 For complex issues, we recommend consulting the Fair Work Ombudsman or an employment
            lawyer.
          </FaqNote>
        </>
      ),
    },
    {
      icon: <Gavel />,
      question: 'Is this a substitute for legal advice?',
      answer: (
        <>
          <strong>No.</strong> FairWorkly is a compliance validation tool, not legal advice. We
          check your payroll and rosters against Fair Work rules and highlight potential issues.
          For complex situations or disputes, consult the Fair Work Ombudsman or an employment
          lawyer.
        </>
      ),
    },
    {
      icon: <CloudDownloadOutlined />,
      question: 'Do you provide CSV templates?',
      answer: (
        <>
          Yes! We provide <strong>CSV templates</strong> for both payslip and roster data to help
          you get started with FairWorkly.
          <br />
          <br />
          <TemplateButton href="/templates">View All CSV Templates →</TemplateButton>
        </>
      ),
    },
  ];

  return (
    <FaqContainer id="faq" aria-labelledby="faq-heading">
      <Container maxWidth="lg">
        <SectionHeader>
          <SectionLabel icon={<HelpOutline />} label="FAQ" />
          <SectionTitle variant="h2" component="h2" id="faq-heading">
            Frequently Asked Questions
          </SectionTitle>
          <SectionSubtitle>Everything you need to know about FairWorkly</SectionSubtitle>
        </SectionHeader>

        <FaqGrid>
          {faqs.map((faq, index) => (
            <FaqCard key={index}>
              <FaqQuestion>
                <Box sx={{ color: 'primary.main', fontSize: '1.5rem', flexShrink: 0 }}>
                  {faq.icon}
                </Box>
                <Typography component="span">{faq.question}</Typography>
              </FaqQuestion>
              <FaqAnswer>{faq.answer}</FaqAnswer>
            </FaqCard>
          ))}
        </FaqGrid>
      </Container>
    </FaqContainer>
  );
}
import { Box, Container, Typography, Chip, styled } from '@mui/material';
import {
  PersonAddOutlined,
  UploadFile,
  PsychologyOutlined,
  TaskAlt,
  EarbudsOutlined,
} from '@mui/icons-material';

const Section = styled('section')(({ theme }) => ({
  padding: theme.spacing(12, 0),
  backgroundColor: theme.palette.background.default,
}));

const Header = styled(Box)(({ theme }) => ({
  textAlign: 'center',
  marginBottom: theme.spacing(8),
}));

const Label = styled(Chip)(({ theme }) => ({
  marginBottom: theme.spacing(2),
  padding: theme.spacing(0.5, 2),
  fontSize: '0.8125rem',
  fontWeight: 600,
  textTransform: 'uppercase',
  letterSpacing: '0.05em',
  backgroundColor: 'rgba(99, 102, 241, 0.12)',
  color: theme.palette.primary.main,
  '& .MuiChip-icon': {
    color: theme.palette.primary.main,
  },
}));

const Title = styled(Typography)(({ theme }) => ({
  fontSize: '2.5rem',
  fontWeight: 800,
  marginBottom: theme.spacing(2),
  color: theme.palette.text.primary,
  [theme.breakpoints.down('sm')]: {
    fontSize: '2rem',
  },
}));

const Subtitle = styled(Typography)(({ theme }) => ({
  fontSize: '1.125rem',
  color: theme.palette.text.secondary,
  maxWidth: '600px',
  margin: '0 auto',
}));

const StepsContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  position: 'relative',
  maxWidth: '1000px',
  margin: '0 auto',
  '&::before': {
    content: '""',
    position: 'absolute',
    top: '40px',
    left: '80px',
    right: '80px',
    height: '2px',
    backgroundColor: theme.palette.divider,
    [theme.breakpoints.down('md')]: {
      display: 'none',
    },
  },
  [theme.breakpoints.down('md')]: {
    flexDirection: 'column',
    gap: theme.spacing(4),
  },
}));

const Step = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  textAlign: 'center',
  flex: 1,
  position: 'relative',
}));

const StepNumber = styled(Box)(({ theme }) => ({
  width: '80px',
  height: '80px',
  borderRadius: theme.shape.borderRadius,
  backgroundColor: theme.palette.background.paper,
  border: `2px solid ${theme.palette.divider}`,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  marginBottom: theme.spacing(3),
  position: 'relative',
  zIndex: 1,
  transition: 'all 0.3s ease',
  color: theme.palette.primary.main,
  '& .MuiSvgIcon-root': {
    fontSize: '2rem',
  },
  '&:hover': {
    borderColor: theme.palette.primary.main,
    backgroundColor: 'rgba(99, 102, 241, 0.12)',
    transform: 'scale(1.1)',
  },
}));

const StepTitle = styled(Typography)(({ theme }) => ({
  fontSize: '1.125rem',
  fontWeight: 700,
  marginBottom: theme.spacing(1),
  color: theme.palette.text.primary,
}));

const StepDesc = styled(Typography)(({ theme }) => ({
  fontSize: '0.875rem',
  color: theme.palette.text.secondary,
  maxWidth: '180px',
}));

const StepTime = styled(Typography)(({ theme }) => ({
  fontSize: '0.75rem',
  color: theme.palette.success.main,
  fontWeight: 600,
  marginTop: theme.spacing(1),
}));

export default function HowItWorksSection() {
  const steps = [
    {
      icon: <PersonAddOutlined />,
      title: 'Sign Up',
      description: 'Create account in 30 seconds',
      time: 'No credit card',
    },
    {
      icon: <UploadFile />,
      title: 'Upload Roster',
      description: 'Drag & drop CSV/Excel',
      time: 'Select Award',
    },
    {
      icon: <PsychologyOutlined />,
      title: 'AI Analysis',
      description: 'Checks award rules',
      time: 'Under 3 seconds',
    },
    {
      icon: <TaskAlt />,
      title: 'Get Results',
      description: 'Issues with fixes',
      time: 'Plain English',
    },
  ];

  return (
    <Section id="how-it-works" aria-label="How it works">
      <Container maxWidth="lg">
        <Header>
          <Label icon={<EarbudsOutlined />} label="How It Works" />
          <Title variant="h2" component="h2">
            Compliance in 4 Simple Steps
          </Title>
          <Subtitle>From upload to compliant in under 3 minutes</Subtitle>
        </Header>

        <StepsContainer>
          {steps.map((step, index) => (
            <Step key={index}>
              <StepNumber>{step.icon}</StepNumber>
              <StepTitle>{step.title}</StepTitle>
              <StepDesc>{step.description}</StepDesc>
              <StepTime>{step.time}</StepTime>
            </Step>
          ))}
        </StepsContainer>
      </Container>
    </Section>
  );
}
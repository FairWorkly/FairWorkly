import { Box, Container, Typography, Button, styled } from '@mui/material';
import { ArrowForward } from '@mui/icons-material';

const Section = styled('section')(({ theme }) => ({
  background: 'linear-gradient(135deg, #6366f1, #ec4899)',
  padding: theme.spacing(10, 0),
  textAlign: 'center',
  position: 'relative',
  overflow: 'hidden',
  '&::before': {
    content: '""',
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none'%3E%3Cg fill='%23ffffff' fill-opacity='0.05'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`,
  },
}));

const Content = styled(Box)({
  position: 'relative',
  zIndex: 1,
});

const Title = styled(Typography)(({ theme }) => ({
  fontSize: '2.5rem',
  fontWeight: 800,
  color: theme.palette.common.white,
  marginBottom: theme.spacing(2),
  [theme.breakpoints.down('sm')]: {
    fontSize: '2rem',
  },
}));

const Subtitle = styled(Typography)(({ theme }) => ({
  fontSize: '1.125rem',
  color: 'rgba(255, 255, 255, 0.85)',
  marginBottom: theme.spacing(4),
  maxWidth: '700px',
  margin: `0 auto ${theme.spacing(4)}`,
}));

const ActionButton = styled(Button)(({ theme }) => ({
  backgroundColor: theme.palette.common.white,
  color: theme.palette.primary.main,
  padding: theme.spacing(2, 4),
  fontSize: '1rem',
  fontWeight: 600,
  borderRadius: theme.spacing(2.5),
  '&:hover': {
    backgroundColor: theme.palette.common.white,
    transform: 'translateY(-2px)',
    boxShadow: theme.shadows[4],
  },
}));

export default function CtaSection() {
  return (
    <Section aria-label="Call to action">
      <Container maxWidth="lg">
        <Content>
          <Title variant="h2" component="h2">
            Ready to Simplify Compliance?
          </Title>
          <Subtitle align="center">
            Join hundreds of Australian SMEs using FairWorkly to stay compliant and confident.
          </Subtitle>
          <ActionButton variant="contained" size="large" endIcon={<ArrowForward />}>
            Start Your Free Trial
          </ActionButton>
        </Content>
      </Container>
    </Section>
  );
}
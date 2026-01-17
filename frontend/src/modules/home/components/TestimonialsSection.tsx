import {
  Box,
  Container,
  Typography,
  Card,
  Grid,
  Chip,
  styled,
} from '@mui/material';
import { Star, FormatQuoteOutlined } from '@mui/icons-material';

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

const TestimonialCard = styled(Card)(({ theme }) => ({
  padding: theme.spacing(4),
  borderRadius: theme.shape.borderRadius,
  border: `1px solid ${theme.palette.divider}`,
  transition: 'all 0.3s ease',
  height: '100%',
  display: 'flex',
  flexDirection: 'column',
  '&:hover': {
    transform: 'translateY(-4px)',
    boxShadow: theme.shadows[4],
  },
}));

const Stars = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(0.5),
  marginBottom: theme.spacing(2),
  color: '#facc15',
}));

const Quote = styled(Typography)(({ theme }) => ({
  fontSize: '1rem',
  color: theme.palette.text.secondary,
  lineHeight: 1.7,
  marginBottom: theme.spacing(3),
  flex: 1,
}));

const Author = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(2),
}));

const Avatar = styled(Box)(({ theme }) => ({
  width: '48px',
  height: '48px',
  borderRadius: '12px',
  background: 'linear-gradient(135deg, #6366f1, #ec4899)',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  color: theme.palette.common.white,
  fontWeight: 700,
}));

const AuthorName = styled(Typography)(({ theme }) => ({
  fontWeight: 600,
  color: theme.palette.text.primary,
}));

const AuthorRole = styled(Typography)(({ theme }) => ({
  fontSize: '0.8125rem',
  color: theme.palette.text.secondary,
}));

interface Testimonial {
  text: string;
  author: string;
  role: string;
  avatar: string;
}

export default function TestimonialsSection() {
  const testimonials: Testimonial[] = [
    {
      text: "FairWorkly caught a $15,000 underpayment issue in our first compliance check. The ROI was immediate.",
      author: "Sarah Mitchell",
      role: "Owner, Mitchell's Café · Melbourne",
      avatar: "SM",
    },
    {
      text: "What used to take 2 hours now takes 3 minutes. The AI understands awards better than most consultants.",
      author: "James Chen",
      role: "HR Manager, RetailCo · Sydney",
      avatar: "JC",
    },
    {
      text: "The AI Q&A is like having a Fair Work expert on call 24/7. Our managers now confidently handle compliance.",
      author: "Lisa Thompson",
      role: "Operations Director · Brisbane",
      avatar: "LT",
    },
  ];

  return (
    <Section id="testimonials" aria-label="Testimonials">
      <Container maxWidth="lg">
        <Header>
          <Label icon={<FormatQuoteOutlined/>} label="Testimonials" />
          <Title variant="h2" component="h2">
            Trusted by Australian Businesses
          </Title>
        </Header>

        <Grid container spacing={4}>
          {testimonials.map((testimonial, index) => (
            <Grid size={{ xs: 12, md: 4 }} key={index}>
              <TestimonialCard>
                <Stars>
                  {[...Array(5)].map((_, i) => (
                    <Star key={i} sx={{ fontSize: '1.25rem' }} />
                  ))}
                </Stars>
                <Quote>{testimonial.text}</Quote>
                <Author>
                  <Avatar>{testimonial.avatar}</Avatar>
                  <Box>
                    <AuthorName>{testimonial.author}</AuthorName>
                    <AuthorRole>{testimonial.role}</AuthorRole>
                  </Box>
                </Author>
              </TestimonialCard>
            </Grid>
          ))}
        </Grid>
      </Container>
    </Section>
  );
}
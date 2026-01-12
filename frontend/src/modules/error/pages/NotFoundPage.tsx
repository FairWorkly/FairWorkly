import { Box, Typography, Button } from '@mui/material'
import { Link } from 'react-router-dom'
import SearchOffIcon from '@mui/icons-material/SearchOff'

export default function NotFoundPage() {
  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        bgcolor: '#f8fafc',
      }}
    >
      <Box sx={{ textAlign: 'center', maxWidth: 500, px: 3 }}>
        <SearchOffIcon
          sx={{
            fontSize: 80,
            color: 'warning.main',
            mb: 2,
          }}
        />

        <Typography
          variant="h1"
          sx={{
            fontSize: '6rem',
            fontWeight: 700,
            color: 'text.primary',
            mb: 2,
          }}
        >
          404
        </Typography>

        <Typography
          variant="h5"
          sx={{
            color: 'text.secondary',
            mb: 1,
          }}
        >
          Page Not Found
        </Typography>

        <Typography
          variant="body1"
          sx={{
            color: 'text.secondary',
            mb: 4,
          }}
        >
          The page you're looking for doesn't exist.
        </Typography>

        <Button
          component={Link}
          to="/"
          variant="contained"
          size="large"
          sx={{
            bgcolor: '#6366f1',
            '&:hover': {
              bgcolor: '#5558e3',
            },
          }}
        >
          Back to Home
        </Button>
      </Box>
    </Box>
  )
}

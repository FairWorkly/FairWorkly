import { useNavigate, useParams } from 'react-router-dom'
import { CircularProgress, Typography, Alert, Button, Box } from '@mui/material'
import SmartToyOutlinedIcon from '@mui/icons-material/SmartToyOutlined'
import { ComplianceResults } from '@/shared/compliance-check'
import { useValidateRoster } from '../hooks'
import { LoadingContainer, ErrorContainer } from '../ui/RosterResults.styles'
import { useAuth } from '@/modules/auth/hooks/useAuth'

export function RosterResults() {
  const navigate = useNavigate()
  const { rosterId } = useParams<{ rosterId: string }>()
  const { user } = useAuth()

  const { complianceData, validationId, isLoading, isError, errorMessage } =
    useValidateRoster(rosterId)

  if (isLoading) {
    return (
      <LoadingContainer>
        <CircularProgress />
        <Typography variant="body1" color="text.secondary">
          Running compliance checks...
        </Typography>
      </LoadingContainer>
    )
  }

  if (isError || !complianceData) {
    return (
      <ErrorContainer>
        <Alert
          severity="error"
          action={
            <Button color="inherit" onClick={() => navigate('/roster/upload')}>
              Try Again
            </Button>
          }
        >
          {errorMessage}
        </Alert>
      </ErrorContainer>
    )
  }

  return (
    <>
      {user?.role === 'admin' && rosterId && (
        <Box display="flex" justifyContent="flex-end" mb={2}>
          <Button
            variant="outlined"
            startIcon={<SmartToyOutlinedIcon />}
            onClick={() =>
              navigate(
                validationId
                  ? `/fairbot?intent=roster&rosterId=${rosterId}&validationId=${validationId}`
                  : `/fairbot?intent=roster&rosterId=${rosterId}`,
              )
            }
          >
            Ask FairBot
          </Button>
        </Box>
      )}
      <ComplianceResults
        metadata={complianceData.metadata}
        summary={complianceData.summary}
        categories={complianceData.categories}
        onNewValidation={() => navigate('/roster/upload')}
        onNavigateBack={() => navigate('/roster/upload')}
        breadcrumbLabel="Roster"
        periodLabel="Roster period"
        resultType="roster"
      />
    </>
  )
}

import { useNavigate, useParams } from 'react-router-dom'
import { CircularProgress, Typography, Alert, Button } from '@mui/material'
import { ComplianceResults } from '@/shared/compliance-check'
import { useValidateRoster } from '../hooks'
import { LoadingContainer, ErrorContainer } from '../ui/RosterResults.styles'

export function RosterResults() {
  const navigate = useNavigate()
  const { rosterId } = useParams<{ rosterId: string }>()

  const { complianceData, isLoading, isError, errorMessage } =
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
  )
}

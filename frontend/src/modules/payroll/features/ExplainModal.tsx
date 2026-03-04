import React from 'react'
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  CircularProgress,
  Alert,
  Paper,
  alpha,
} from '@mui/material'
import LightbulbOutlinedIcon from '@mui/icons-material/LightbulbOutlined'
import DescriptionOutlinedIcon from '@mui/icons-material/DescriptionOutlined'
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline'
import { styled } from '@/styles/styled'
import type { ExplainResult } from '../types'

// --- Styled components ---

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
  maxHeight: '70vh',
  overflowY: 'auto',
  paddingTop: theme.spacing(2),
}))

const LoadingBox = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  padding: theme.spacing(6, 2),
  gap: theme.spacing(3),
  textAlign: 'center',
}))

const ContentStack = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2.5),
}))

const SectionCard = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(2.5),
  borderRadius: theme.fairworkly.radius.sm,
}))

const SectionHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  gap: theme.spacing(1),
  marginBottom: theme.spacing(1.5),
}))

const PreLineText = styled(Typography)({
  whiteSpace: 'pre-line',
})

const SourceItem = styled(Box)(({ theme }) => ({
  backgroundColor: theme.palette.action.hover,
  borderRadius: theme.fairworkly.radius.sm,
  padding: theme.spacing(1.5),
}))

const SourcesStack = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
}))

const ModalActions = styled(DialogActions)(({ theme }) => ({
  padding: theme.spacing(1, 3, 3),
  paddingRight: theme.spacing(5),
}))

const ActionBtn = styled(Button)(({ theme }) => ({
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

// --- Helpers ---

const formatPage = (page?: number | null): string => {
  if (typeof page !== 'number' || Number.isNaN(page)) return ''
  return `, p.${page + 1}`
}

const truncate = (text?: string | null, maxLength = 140): string | null => {
  if (!text) return null
  if (text.length <= maxLength) return text
  return text.slice(0, maxLength) + '...'
}

// --- Component ---

interface ExplainModalProps {
  open: boolean
  onClose: () => void
  onRetry: () => void
  result: ExplainResult | null
  isLoading: boolean
  error: string | null
  employeeName: string
}

export const ExplainModal: React.FC<ExplainModalProps> = ({
  open,
  onClose,
  onRetry,
  result,
  isLoading,
  error,
  employeeName,
}) => {
  const showWarning = result?.warning != null
  const showSuccess = result != null && !showWarning && !error

  const title = isLoading ? 'Analyzing Issue...' : 'How to Fix'

  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <ModalTitle>
        <TitleIconBox>
          <LightbulbOutlinedIcon fontSize="small" />
        </TitleIconBox>
        <Typography variant="h6">{title}</Typography>
      </ModalTitle>

      <ModalContent>
        {/* State A: Loading */}
        {isLoading && (
          <LoadingBox>
            <CircularProgress />
            <Typography variant="body2" color="text.secondary">
              Generating AI explanation for {employeeName}&apos;s issue...
              <br />
              This may take up to 30 seconds.
            </Typography>
          </LoadingBox>
        )}

        {/* State B: Error */}
        {!isLoading && error && (
          <Box>
            <Alert severity="error">{error}</Alert>
            <Typography variant="body2" color="text.secondary" mt={2}>
              Please try again later. If the issue persists, contact support.
            </Typography>
          </Box>
        )}

        {/* State D: Warning */}
        {!isLoading && showWarning && (
          <Box>
            <Alert severity="warning">{result!.warning}</Alert>
            <Typography variant="body2" color="text.secondary" mt={2}>
              This is an informational warning. No action is required.
            </Typography>
          </Box>
        )}

        {/* State C: Success */}
        {!isLoading && showSuccess && (
          <ContentStack>
            {result!.detailedExplanation && (
              <SectionCard variant="outlined">
                <SectionHeader>
                  <DescriptionOutlinedIcon fontSize="small" color="action" />
                  <Typography variant="subtitle2">
                    Detailed Explanation
                  </Typography>
                </SectionHeader>
                <PreLineText variant="body2" color="text.secondary">
                  {result!.detailedExplanation}
                </PreLineText>
              </SectionCard>
            )}

            {result!.recommendation && (
              <SectionCard variant="outlined">
                <SectionHeader>
                  <CheckCircleOutlineIcon fontSize="small" color="success" />
                  <Typography variant="subtitle2">Recommendation</Typography>
                </SectionHeader>
                <PreLineText variant="body2" color="text.secondary">
                  {result!.recommendation}
                </PreLineText>
              </SectionCard>
            )}

            {(result!.sources?.length ?? 0) > 0 && (
              <Box>
                <Typography variant="subtitle2" mb={1}>
                  Sources ({result!.sources.length})
                </Typography>
                <SourcesStack>
                  {result!.sources.slice(0, 5).map((src, i) => (
                    <SourceItem key={`${src.source}-${src.page ?? i}`}>
                      <Typography variant="caption" fontWeight={600}>
                        {i + 1}. {src.source}
                        {formatPage(src.page)}
                      </Typography>
                      {truncate(src.content) && (
                        <Typography
                          variant="caption"
                          display="block"
                          color="text.secondary"
                        >
                          {truncate(src.content)}
                        </Typography>
                      )}
                    </SourceItem>
                  ))}
                </SourcesStack>
              </Box>
            )}

            {result!.model && (
              <Typography variant="caption" color="text.disabled">
                Model: {result!.model}
              </Typography>
            )}
          </ContentStack>
        )}
      </ModalContent>

      <ModalActions>
        {!isLoading && error && (
          <ActionBtn variant="outlined" onClick={onRetry}>
            Try Again
          </ActionBtn>
        )}
        <ActionBtn variant="outlined" onClick={onClose}>
          {isLoading ? 'Cancel' : 'Close'}
        </ActionBtn>
      </ModalActions>
    </Dialog>
  )
}

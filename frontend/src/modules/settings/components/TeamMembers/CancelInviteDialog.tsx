import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from '@mui/material'

interface CancelInviteDialogProps {
  open: boolean
  memberName: string
  isCancelling: boolean
  onConfirm: () => void
  onCancel: () => void
}

export function CancelInviteDialog({
  open,
  memberName,
  isCancelling,
  onConfirm,
  onCancel,
}: CancelInviteDialogProps) {
  return (
    <Dialog open={open} onClose={isCancelling ? undefined : onCancel} maxWidth="xs" fullWidth>
      <DialogTitle>Cancel Invitation</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to cancel the invitation for <strong>{memberName}</strong>?
          They will be removed from the team.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel} variant="outlined" color="inherit" disabled={isCancelling}>
          Keep
        </Button>
        <Button
          onClick={onConfirm}
          variant="contained"
          color="error"
          disabled={isCancelling}
          startIcon={isCancelling ? <CircularProgress size={16} /> : undefined}
        >
          {isCancelling ? 'Cancelling...' : 'Cancel Invitation'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from '@mui/material'

interface DeactivateDialogProps {
  open: boolean
  memberName: string
  isUpdating: boolean
  onConfirm: () => void
  onCancel: () => void
}

export function DeactivateDialog({
  open,
  memberName,
  isUpdating,
  onConfirm,
  onCancel,
}: DeactivateDialogProps) {
  return (
    <Dialog open={open} onClose={isUpdating ? undefined : onCancel} maxWidth="xs" fullWidth>
      <DialogTitle>Deactivate Team Member</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to deactivate <strong>{memberName}</strong>?
          They will no longer be able to log in.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel} variant="outlined" color="inherit" disabled={isUpdating}>
          Cancel
        </Button>
        <Button
          onClick={onConfirm}
          variant="contained"
          color="error"
          disabled={isUpdating}
          startIcon={isUpdating ? <CircularProgress size={16} /> : undefined}
        >
          {isUpdating ? 'Deactivating...' : 'Deactivate'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

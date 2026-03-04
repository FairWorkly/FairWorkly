import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from '@mui/material'

interface RoleChangeDialogProps {
  open: boolean
  memberName: string
  newRole: string
  isUpdating: boolean
  onConfirm: () => void
  onCancel: () => void
}

export function RoleChangeDialog({
  open,
  memberName,
  newRole,
  isUpdating,
  onConfirm,
  onCancel,
}: RoleChangeDialogProps) {
  return (
    <Dialog open={open} onClose={isUpdating ? undefined : onCancel} maxWidth="xs" fullWidth>
      <DialogTitle>Change Role</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to change <strong>{memberName}</strong>&apos;s role
          to <strong>{newRole}</strong>?
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel} variant="outlined" color="inherit" disabled={isUpdating}>
          Cancel
        </Button>
        <Button
          onClick={onConfirm}
          variant="contained"
          color="primary"
          disabled={isUpdating}
          startIcon={isUpdating ? <CircularProgress size={16} /> : undefined}
        >
          {isUpdating ? 'Updating...' : 'Change Role'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

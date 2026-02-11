import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
} from '@mui/material'

interface Props {
  open: boolean
  memberName: string
  onConfirm: () => void
  onCancel: () => void
}

export function DeactivateDialog({ open, memberName, onConfirm, onCancel }: Props) {
  return (
    <Dialog open={open} onClose={onCancel}>
      <DialogTitle>Deactivate Member</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to deactivate {memberName}? They will lose access to the system.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Cancel</Button>
        <Button onClick={onConfirm} color="error" variant="contained">
          Deactivate
        </Button>
      </DialogActions>
    </Dialog>
  )
}

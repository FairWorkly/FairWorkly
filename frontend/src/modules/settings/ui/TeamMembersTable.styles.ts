import { TableContainer, TableCell, TableRow, IconButton } from '@mui/material'
import { styled } from '@/styles/styled'

export const StyledTableContainer = styled(TableContainer)(({ theme }) => ({
  marginTop: theme.spacing(3),
  borderRadius: theme.shape.borderRadius,
  border: `1px solid ${theme.palette.divider}`,
}))

export const HeaderCell = styled(TableCell)(({ theme }) => ({
  backgroundColor: theme.palette.grey[50],
  fontWeight: 600,
  color: theme.palette.text.primary,
}))

export const StyledTableRow = styled(TableRow)(({ theme }) => ({
  '&:last-child td, &:last-child th': {
    border: 0,
  },
  '&:hover': {
    backgroundColor: theme.palette.action.hover,
  },
}))

export const ActionButton = styled(IconButton)(({ theme }) => ({
  color: theme.palette.text.secondary,
  '&:hover': {
    color: theme.palette.error.main,
    backgroundColor: theme.palette.error.light,
  },
}))

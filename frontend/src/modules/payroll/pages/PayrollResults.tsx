import React from 'react'
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'

export const PayrollResults: React.FC = () => {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Payroll Results
      </Typography>
      <Typography variant="body1">
        This page will display issues, risk levels and suggestions based on the
        payroll check.
      </Typography>
    </Box>
  )
}

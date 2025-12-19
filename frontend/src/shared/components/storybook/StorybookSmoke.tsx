import { Stack, Typography, Button } from '@mui/material'

export function StorybookSmoke() {
  return (
    <Stack spacing={2} sx={{ p: 2 }}>
      <Typography variant="h5">Storybook is working ✅</Typography>
      <Button variant="contained">MUI Button</Button>
    </Stack>
  )
}

import type { PropsWithChildren } from 'react'
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material'

const theme = createTheme()

const AppThemeProvider = ({ children }: PropsWithChildren) => {
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            {children}
        </ThemeProvider>
    )
}

export default AppThemeProvider

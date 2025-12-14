import React from 'react'
import {
  ThemeProvider as MuiThemeProvider,
  createTheme,
} from '@mui/material/styles'
import CssBaseline from '@mui/material/CssBaseline'

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#9c27b0',
    },
    // 添加的
    success: {
      main: "#10B981",
    },
    warning: {
      main: "#F59E0B",
    },
    info:{
      main: "#3B82F6",
    },
    text: {
      primary: "#111827",
      secondary: "#6B7280",
    },
    divider: "#E5E7EB",
  },
  spacing: 8,
})

interface ThemeProviderProps {
  children: React.ReactNode
}

export const ThemeProvider: React.FC<ThemeProviderProps> = ({ children }) => {
  return (
    <MuiThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </MuiThemeProvider>
  )
}

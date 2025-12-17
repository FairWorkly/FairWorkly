import React from 'react'
import {
  ThemeProvider as MuiThemeProvider,
  createTheme,
} from '@mui/material/styles'
import CssBaseline from '@mui/material/CssBaseline'


const colors = {
  primary: "#6366F1",
  primaryLight: "#E0E7FF",
  success: "#10B981",
  warning: "#F59E0B",
  error: "#EF4444",
  info: "#3B82F6",

  gray50: "#F9FAFB",
  gray100: "#F3F4F6",
  gray200: "#E5E7EB",
  gray500: "#6B7280",
  gray700: "#374151",
  gray900: "#111827",

  white: "#FFFFFF",
};


const theme = createTheme({
  palette: {
    primary: {
      main: colors.primary,
      light: colors.primaryLight,
    },
    success: {
      main: colors.success,
    },
    warning: {
      main: colors.warning,
    },
    info: {
      main: colors.info,
    },
    text: {
      primary: colors.gray900,
      secondary: colors.gray500,
    },
    divider: colors.gray200,
  },
  breakpoints: {
    values: {
      xs: 0,
      sm: 640,
      md: 768,
      lg: 1024,
      xl: 1280,
    },
  },
});

export const tokens = {
  colors,
  spacing: {
    sectionPaddingMobile: "80px 24px",
    sectionPaddingDesktop: "100px 48px",
    cardPadding: "32px 24px",
    cardPaddingMobile: "24px 20px",
  },
  borderRadius: {
    small: "8px",
    medium: "12px",
    large: "16px",
    pill: "20px",
    circle: "50%",
  },
  transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
  cardShadow: "0 1px 3px rgba(0, 0, 0, 0.05)",
  cardHoverShadow: "0 12px 24px rgba(0, 0, 0, 0.1)",
  imageShadow: "0 25px 50px -12px rgba(0, 0, 0, 0.15)",
};

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

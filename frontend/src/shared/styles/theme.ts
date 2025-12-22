import { createTheme, type Shadows } from '@mui/material/styles'

export const theme = createTheme({
  palette: {
    primary: {
      main: '#6366f1',
      light: '#818cf8',
      dark: '#4f46e5',
    },
    secondary: {
      main: '#ec4899',
    },
    error: {
      main: '#ef4444',
      light: '#fecaca',
    },
    warning: {
      main: '#f97316',
      light: '#fed7aa',
    },
    success: {
      main: '#10b981',
      light: '#a7f3d0',
    },
    info: {
      main: '#06b6d4',
      light: '#a5f3fc',
    },
    background: {
      default: '#f8fafc',
      paper: '#ffffff',
    },
    text: {
      primary: '#1e293b',
      secondary: '#64748b',
      disabled: '#94a3b8',
    },
    divider: '#e2e8f0',
  },

  typography: {
    fontFamily:
      'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
    h1: {
      fontSize: '3.5rem',
      fontWeight: 900,
      lineHeight: 1.1,
    },
    h2: {
      fontSize: '2.5rem',
      fontWeight: 800,
      lineHeight: 1.2,
    },
    h3: {
      fontSize: '2rem',
      fontWeight: 700,
      lineHeight: 1.3,
    },
    h4: {
      fontSize: '1.5rem',
      fontWeight: 600,
      lineHeight: 1.4,
    },
    h5: {
      fontSize: '1.25rem',
      fontWeight: 600,
      lineHeight: 1.5,
    },
    h6: {
      fontSize: '1rem',
      fontWeight: 600,
      lineHeight: 1.6,
    },
    body1: {
      fontSize: '1rem',
      lineHeight: 1.6,
    },
    body2: {
      fontSize: '0.875rem',
      lineHeight: 1.6,
    },
    button: {
      fontSize: '0.9375rem',
      fontWeight: 600,
      textTransform: 'none',
    },
  },

  spacing: 8,

  // 基础圆角（最常用的）
  shape: {
    borderRadius: 20, // 卡片默认圆角
  },

  // 阴影系统（和HTML完全一致）
  shadows: [
    'none', // 0
    '0 1px 2px rgba(0,0,0,0.05)', // 1 - sm
    '0 4px 6px -1px rgba(0,0,0,0.1)', // 2 - md
    '0 10px 25px -5px rgba(0,0,0,0.1)', // 3 - lg
    '0 20px 40px -10px rgba(0,0,0,0.15)', // 4 - xl
    '0 4px 15px rgba(99, 102, 241, 0.4)', // 5 - primary button
    '0 4px 20px rgba(0,0,0,0.3)', // 6 - navbar scrolled
    '0 8px 25px rgba(99, 102, 241, 0.5)', // 7 - primary button hover
    ...Array(17).fill('0 1px 3px rgba(0,0,0,0.12)'),
  ] as Shadows,

  components: {
    MuiButton: {
      defaultProps: {
        disableElevation: true,
      },
      styleOverrides: {
        root: {
          borderRadius: 10, // 按钮用10px
          padding: '0.75rem 1.5rem',
          fontSize: '0.9375rem',
          fontWeight: 600,
        },
        sizeLarge: {
          padding: '1rem 2rem',
          fontSize: '1rem',
        },
        containedPrimary: {
          boxShadow: '0 4px 15px rgba(99, 102, 241, 0.4)', // 紫色阴影
          '&:hover': {
            boxShadow: '0 8px 25px rgba(99, 102, 241, 0.5)',
          },
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 20, // 卡片用20px
          boxShadow: '0 4px 6px -1px rgba(0,0,0,0.1)',
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          borderRadius: 12, // Paper用12px
        },
      },
    },
    MuiBadge: {
      styleOverrides: {
        badge: {
          borderRadius: 9999, // 完全圆形
        },
      },
    },
  },
})
import { createTheme } from '@mui/material/styles'
import type { Theme } from '@mui/material/styles'

export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#6366f1',
      light: '#818cf8',
      dark: '#4f46e5',
      contrastText: '#ffffff',
    },
    secondary: {
      main: '#ec4899',
      contrastText: '#ffffff',
    },
    success: { main: '#10b981', dark: '#047857' },
    warning: { main: '#f97316', dark: '#c2410c' },
    error: { main: '#ef4444', dark: '#b91c1c' },

    background: {
      default: '#f8fafc',
      paper: '#ffffff',
    },

    divider: '#e2e8f0',

    text: {
      primary: '#1e293b',
      secondary: '#64748b',
      disabled: '#94a3b8',
    },
  },

  shape: {
    borderRadius: 12,
  },

  typography: {
    fontFamily: [
      'Inter',
      'system-ui',
      '-apple-system',
      'Segoe UI',
      'Roboto',
      'Arial',
    ].join(','),

    h1: {
      fontSize: 56,
      fontWeight: 900,
      letterSpacing: '-0.03em',
      lineHeight: 1.1,
    },
    h2: {
      fontSize: 40,
      fontWeight: 800,
      letterSpacing: '-0.02em',
      lineHeight: 1.15,
    },
    h3: {
      fontSize: 30,
      fontWeight: 800,
      letterSpacing: '-0.02em',
      lineHeight: 1.2,
    },
    h4: {
      fontSize: 22,
      fontWeight: 700,
      letterSpacing: '-0.01em',
      lineHeight: 1.25,
    },

    subtitle1: { fontSize: 16, fontWeight: 700, lineHeight: 1.55 },
    subtitle2: { fontSize: 14, fontWeight: 700, lineHeight: 1.55 },

    body1: { fontSize: 16, fontWeight: 500, lineHeight: 1.65 },
    body2: { fontSize: 14, fontWeight: 500, lineHeight: 1.65 },

    caption: { fontSize: 12, fontWeight: 600, lineHeight: 1.4 },
    overline: {
      fontSize: 11,
      fontWeight: 800,
      letterSpacing: '0.08em',
      textTransform: 'uppercase',
      lineHeight: 1.4,
    },

    button: { textTransform: 'none', fontWeight: 700 },
  },

  /**
   * 只保留“品牌级、跨页面一致”的材料
   * 其它（radius/shadow/layout/grid）回归 MUI 或放 feature/ui
   */
  fairworkly: {
    gradient: {
      primary: 'linear-gradient(135deg, #6366f1, #ec4899)',
      brandText: 'linear-gradient(135deg, #a5b4fc, #f9a8d4)',
    },
    effect: {
      primaryGlow: 'rgba(99, 102, 241, 0.12)',
      gridLine: 'rgba(99, 102, 241, 0.03)',
    },
    surface: {
      navDark: '#1e1b4b',
    },
    layout: {
      containerMaxWidth: 0
    }
  },

  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: ({ theme }: { theme: Theme }) => ({
          backgroundColor: theme.palette.background.default,
          // 把 grid 背景移到 marketing/auth 的 CssBaseline（feature 级）
          // 这里保留轻量的“氛围”即可（否则 app 内页也一直有网格）
          backgroundImage: `
            radial-gradient(ellipse 80% 50% at 20% 0%, rgba(99,102,241,0.08) 0%, transparent 50%),
            radial-gradient(ellipse 60% 40% at 80% 100%, rgba(236,72,153,0.06) 0%, transparent 50%)
          `,
          backgroundAttachment: 'fixed',
          overflowX: 'hidden',
        }),
      },
    },

    MuiOutlinedInput: {
      styleOverrides: {
        root: ({ theme }) => ({
          borderRadius: theme.shape.borderRadius,
          background: theme.palette.background.paper,
          transition: 'box-shadow 0.2s ease, border-color 0.2s ease',
          '&.Mui-focused': {
            boxShadow: `0 0 0 3px ${theme.fairworkly.effect.primaryGlow}`,
          },
        }),
        notchedOutline: ({ theme }) => ({
          borderColor: theme.palette.divider,
        }),
      },
    },

    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: {
        root: ({ theme }) => ({
          borderRadius: theme.shape.borderRadius,
          padding: theme.spacing(1.25, 2),
          transition:
            'transform 0.2s ease, box-shadow 0.2s ease, background 0.2s ease',
        }),
      },
    },

    MuiPaper: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: ({ theme }) => ({
          borderRadius: theme.shape.borderRadius, // 24-ish；需要更大就放 feature/ui
          backgroundImage: 'none',
        }),
      },
    },

    MuiBackdrop: {
      styleOverrides: {
        root: {
          backgroundColor: 'rgba(0, 0, 0, 0.5)',
          backdropFilter: 'blur(4px)',
        },
      },
    },

    MuiDialog: {
      styleOverrides: {
        paper: ({ theme }) => ({
          borderRadius: theme.shape.borderRadius,
          boxShadow: theme.shadows[24],
        }),
      },
    },
  },
})

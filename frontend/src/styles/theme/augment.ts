import type {} from '@mui/material/styles'

export type FairworklyTokens = {
  gradient: {
    primary: string
    brandText: string
  }
  effect: {
    primaryGlow: string
    gridLine: string
  }
  surface: {
    navDark: string
  }
  layout: {
    containerMaxWidth: number
  }
}

declare module '@mui/material/styles' {
  interface Theme {
    fairworkly: FairworklyTokens
  }

  interface ThemeOptions {
    fairworkly: FairworklyTokens
  }

  interface CssVarsTheme {
    fairworkly?: FairworklyTokens
  }
}

export {}





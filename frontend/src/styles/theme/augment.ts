import type {} from '@mui/material/styles'

export type FairworklyTokens = {
  radius: {
    sm: number
    md: number
    lg: number
    xl: number
    pill: number
  }

  shadow: {
    sm: string
    md: string
    lg: string
    xl: string
  }
  gradient: {
    primary: string
    brandText: string
  }
  color: {
    brandLight: string
    brandPink: string
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
    sidebarWidth: number
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

  interface TypographyVariants {
    uiBadge: React.CSSProperties
    uiLabel: React.CSSProperties
  }

  interface TypographyVariantsOptions {
    uiBadge?: React.CSSProperties
    uiLabel?: React.CSSProperties
  }
}

export {}

import type { ReactNode } from 'react'
import { PublicLayout } from './PublicLayout'

export interface PublicAuthLayoutProps {
  branding?: ReactNode
}

/**
 * Authentication pages layout (Login, Register, etc.)
 * Uses split variant - left branding panel + right form container
 */
export function PublicAuthLayout({ branding }: PublicAuthLayoutProps) {
  return <PublicLayout variant="split" branding={branding} />
}

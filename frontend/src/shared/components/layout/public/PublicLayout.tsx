import * as React from 'react'
import { Outlet } from 'react-router-dom'
import {
  PublicShell,
  PublicBranding,
  PublicBrandingFooter,
  PublicSplitMain,
  PublicSplitContainer,
  PublicCenterMain,
} from './PublicLayout.styles'

type PublicLayoutVariant = 'split' | 'center'

export interface PublicLayoutProps {
  variant?: PublicLayoutVariant
  branding?: React.ReactNode
}

export function PublicLayout({
  variant = 'center',
  branding,
}: PublicLayoutProps) {
  if (variant === 'split') {
    return (
      <PublicShell>
        <PublicBranding>
          {branding}
          <PublicBrandingFooter>
            © 2025 FairWorkly · Made in Melbourne, Australia
          </PublicBrandingFooter>
        </PublicBranding>

        <PublicSplitMain>
          <PublicSplitContainer>
            <Outlet />
          </PublicSplitContainer>
        </PublicSplitMain>
      </PublicShell>
    )
  }

  return (
    <PublicShell>
      <PublicCenterMain>
        <Outlet />
      </PublicCenterMain>
    </PublicShell>
  )
}

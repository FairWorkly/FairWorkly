import { type RouteObject } from 'react-router-dom'
import { PublicMarketingLayout } from '@/shared/components/layout/public/PublicMarketingLayout'
import { PublicAuthLayout } from '@/shared/components/layout/public/PublicAuthLayout'
import { AuthBranding } from '@/modules/auth'

export const publicRoutes: RouteObject[] = [
  // Marketing pages (full-width center layout)
  {
    element: <PublicMarketingLayout />,
    children: [
      {
        path: '/',
        lazy: async () => {
          const { HomePage } = await import('@/modules/home/pages/HomePage')
          return { Component: HomePage }
        },
      },
    ],
  },
  // Authentication pages (split layout with branding)
  {
    element: <PublicAuthLayout branding={<AuthBranding />} />,
    children: [
      {
        path: '/login',
        lazy: async () => {
          const { LoginPage } = await import('@/modules/auth/pages/LoginPage')
          return { Component: LoginPage }
        },
      },
      {
        path: '/accept-invite',
        lazy: async () => {
          const { AcceptInvitePage } = await import(
            '@/modules/auth/pages/AcceptInvitePage'
          )
          return { Component: AcceptInvitePage }
        },
      },
      {
        path: '/reset-password',
        lazy: async () => {
          const { ResetPasswordPage } = await import(
            '@/modules/auth/pages/ResetPasswordPage'
          )
          return { Component: ResetPasswordPage }
        },
      },
    ],
  },
]

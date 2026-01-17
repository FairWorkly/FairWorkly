import { type RouteObject } from 'react-router-dom'
import { PublicMarketingLayout } from '@/shared/components/layout/public/PublicMarketingLayout'
import { PublicAuthLayout } from '@/shared/components/layout/public/PublicAuthLayout'
import { AuthBranding } from '@/modules/auth'
import { HomePage } from '@/modules/home/pages/HomePage'
import { LoginPage } from '@/modules/auth/pages/LoginPage'
import { TemplatesPage } from '@/modules/home/pages/TemplatesPage'

export const publicRoutes: RouteObject[] = [
  // Marketing pages (full-width center layout)
  {
    element: <PublicMarketingLayout />,
    children: [
      {
        path: '/',
        element: <HomePage />,
      },
      {
        path: '/templates',
        element: <TemplatesPage />,
      },
    ],
  },
  // Authentication pages (split layout with branding)
  {
    element: <PublicAuthLayout branding={<AuthBranding />} />,
    children: [
      {
        path: '/login',
        element: <LoginPage />,
      },
    ],
  },
]

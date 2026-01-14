import { type RouteObject } from 'react-router-dom'
import { PublicLayout } from '@/shared/components/layout/public/PublicLayout'
import { HomePage } from '@/modules/home/pages/HomePage'
import { LoginPage } from '@/modules/auth/pages/LoginPage'
import { TemplatesPage } from '@/modules/home/pages/TemplatesPage'

export const publicRoutes: RouteObject[] = [
  {
    element: <PublicLayout />,
    children: [
      {
        path: '/',
        element: <HomePage />,
      },
      {
        path: '/login',
        element: <LoginPage />,
      },
      {
        path: '/templates',
        element: <TemplatesPage />,
      },
    ],
  },
]

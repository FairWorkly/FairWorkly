import { type RouteObject } from 'react-router-dom'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'
import { RoleBasedRoute } from '@/shared/components/guards/RoleBasedRoute'
import { MainLayout } from '@/shared/components/layout/app/MainLayout'
import { FairBotPage } from '@/modules/fairbot/pages/FairBotPage'

export const fairbotRoutes: RouteObject[] = [
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [
          // Admin only - FairBot compliance tool
          {
            element: <RoleBasedRoute allow={['admin']} />,
            children: [
              {
                path: '/fairbot',
                element: <FairBotPage />,
              },
            ],
          },
        ],
      },
    ],
  },
]

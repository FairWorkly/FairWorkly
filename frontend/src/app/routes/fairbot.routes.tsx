import { type RouteObject } from 'react-router-dom'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'
import { RoleBasedRoute } from '@/shared/components/guards/RoleBasedRoute'
import { MainLayout } from '@/shared/components/layout/app/MainLayout'
import { FairBotChat } from '@/modules/fairbot/pages/FairBotChat'

export const fairbotRoutes: RouteObject[] = [
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [
          // Admin + Manager - FairBot compliance tool
          {
            element: <RoleBasedRoute allow={['admin', 'manager']} />,
            children: [
              {
                path: '/fairbot',
                element: <FairBotChat />,
              },
            ],
          },
        ],
      },
    ],
  },
]

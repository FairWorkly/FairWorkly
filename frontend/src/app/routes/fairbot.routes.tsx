import { type RouteObject } from 'react-router-dom'
import { RequireAuth } from './RequireAuth'
import { RoleBasedRoute } from '@/shared/components/guards/RoleBasedRoute'
import { MainLayout } from '@/shared/components/layout/app/MainLayout'
import { FairBotChat } from '@/modules/fairbot/pages/FairBotChat'

export const fairbotRoutes: RouteObject[] = [
  {
    element: <RequireAuth />,
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
                element: <FairBotChat />,
              },
            ],
          },
        ],
      },
    ],
  },
]

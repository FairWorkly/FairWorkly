import { type RouteObject } from 'react-router-dom'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'
import { MainLayout } from '@/shared/components/layout/MainLayout'
import { FairBotChat } from '@/modules/fairbot/pages/FairBotChat'

export const fairbotRoutes: RouteObject[] = [
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [
          {
            path: '/fairbot',
            element: <FairBotChat />,
          },
        ],
      },
    ],
  },
]

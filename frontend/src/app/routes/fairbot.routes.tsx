import { type RouteObject } from 'react-router-dom'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'
import { FairBotChat } from '@/modules/fairbot/pages/FairBotChat'

export const fairbotRoutes: RouteObject[] = [
  {
    path: '/fairbot',
    element: (
      <ProtectedRoute>
        <FairBotChat />
      </ProtectedRoute>
    ),
  },
]

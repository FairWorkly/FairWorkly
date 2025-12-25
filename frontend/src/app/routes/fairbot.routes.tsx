import { type RouteObject } from 'react-router-dom'
// import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'
import { FairBotChat } from '@/modules/fairbot/pages/FairBotChat'

export const fairbotRoutes: RouteObject[] = [
  {
    path: '/fairbot',
    element: (
      // here the ProtectedRoute is disable for the sake of development
      // to test the ProtectedRoute
      // enable <ProtectedRoute>

      // <ProtectedRoute>
      <FairBotChat />
      // </ProtectedRoute>
    ),
  },
]

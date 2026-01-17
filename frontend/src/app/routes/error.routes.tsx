import { type RouteObject } from 'react-router-dom'
import NotFoundPage from '@/modules/error/pages/NotFoundPage'
import { ForbiddenPage } from '@/modules/error/pages/ForbiddenPage'

export const errorRoutes: RouteObject[] = [
  {
    path: '/403',
    element: <ForbiddenPage />,
  },
  {
    path: '*',
    element: <NotFoundPage />,
  },
]

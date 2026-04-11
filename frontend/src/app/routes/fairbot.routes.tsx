import { type RouteObject } from 'react-router-dom'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'
import { RoleBasedRoute } from '@/shared/components/guards/RoleBasedRoute'
import { MainLayout } from '@/shared/components/layout/app/MainLayout'

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
                lazy: async () => {
                  const { FairBotPage } = await import(
                    '@/modules/fairbot/pages/FairBotPage'
                  )
                  return { Component: FairBotPage }
                },
              },
              {
                path: '/debate',
                lazy: async () => {
                  const { DebatePage } = await import(
                    '@/modules/fairbot/features/debate'
                  )
                  return { Component: DebatePage }
                },
              },
            ],
          },
        ],
      },
    ],
  },
]

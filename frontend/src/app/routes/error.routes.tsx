import { type RouteObject } from 'react-router-dom'

export const errorRoutes: RouteObject[] = [
  {
    path: '/403',
    lazy: async () => {
      const { ForbiddenPage } = await import('@/modules/error/pages/ForbiddenPage')
      return { Component: ForbiddenPage }
    },
  },
  {
    path: '*',
    lazy: async () => {
      const module = await import('@/modules/error/pages/NotFoundPage')
      return { Component: module.default }
    },
  },
]

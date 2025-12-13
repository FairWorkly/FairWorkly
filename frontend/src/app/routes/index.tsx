import { Navigate, useRoutes, type RouteObject } from 'react-router-dom'
import { MainLayout } from '@/shared/components/layout/MainLayout'

// Import route configs
import { publicRoutes } from './public.routes'
import { fairbotRoutes } from './fairbot.routes'
import { toolRoutes } from './tools.routes'

const routes: RouteObject[] = [
  ...publicRoutes, // Don't need to login，No MainLayout

  {
    element: <MainLayout />, // Layout Route, Protected Routes,need to login
    children: [
      ...fairbotRoutes, // /fairbot
      ...toolRoutes, // /payroll, /compliance, /documents, /employees
    ],
  },

  {
    path: '*',
    element: <Navigate to="/" replace />, // 404 Fallback
  },
]

export const AppRoutes: React.FC = () => {
  return useRoutes(routes)
}

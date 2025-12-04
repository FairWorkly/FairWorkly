import React from 'react'
import type { RouteObject } from 'react-router-dom'
import { LoginPage } from '../../modules/auth/pages/LoginPage'

export const authRoutes: RouteObject[] = [
  {
    path: '/login',
    element: <LoginPage />,
  },
  // add /register /forgot-password in the future
]

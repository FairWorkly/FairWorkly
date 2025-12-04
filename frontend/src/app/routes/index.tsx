import React from 'react'
import { useRoutes, type RouteObject } from 'react-router-dom'

import { homeRoutes } from './home.routes'
import { authRoutes } from './auth.routes'
import { complianceRoutes } from './compliance.routes'
import { documentsRoutes } from './documents.routes'
import { payrollRoutes } from './payroll.routes'
import { employeeRoutes } from './employee.routes'

const routes: RouteObject[] = [
  ...homeRoutes,
  ...authRoutes,
  ...complianceRoutes,
  ...documentsRoutes,
  ...payrollRoutes,
  ...employeeRoutes,
]

export const AppRoutes: React.FC = () => {
  const element = useRoutes(routes)
  return element
}

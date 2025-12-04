import React from 'react'
import type { RouteObject } from 'react-router-dom'
import { PayrollHome } from '../../modules/payroll/pages/PayrollHome'

export const payrollRoutes: RouteObject[] = [
  {
    path: '/payroll',
    element: <PayrollHome />,
  },
]

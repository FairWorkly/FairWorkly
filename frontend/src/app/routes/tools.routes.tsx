import { Navigate, type RouteObject } from 'react-router-dom'
import { MainLayout } from '@/shared/components/layout/app/MainLayout'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'

// Payroll pages
import { PayrollUpload } from '@/modules/payroll/pages/PayrollUpload'
import { PayrollResults } from '@/modules/payroll/pages/PayrollResults'

// Compliance pages
import { RosterUpload } from '@/modules/roster/pages/RosterUpload'
import { RosterResults } from '@/modules/roster/pages/RosterResults'

// Documents pages
import { DocumentTemplates } from '@/modules/documents/pages/DocumentTemplates'
import { GenerateDocument } from '@/modules/documents/pages/GenerateDocument'
import { DocumentLibrary } from '@/modules/documents/pages/DocumentLibrary'

// Settings pages
import { Settings } from '@/modules/settings/pages/Settings'

import { RoleBasedRoute } from '@/shared/components/guards/RoleBasedRoute'

export const toolRoutes: RouteObject[] = [
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [
          // Admin only - Payroll
          {
            element: <RoleBasedRoute allow={['admin']} />,
            children: [
              {
                path: '/payroll',
                children: [
                  { path: 'upload', element: <PayrollUpload /> },
                  { path: 'results', element: <PayrollResults /> },
                  { index: true, element: <Navigate to="upload" replace /> },
                ],
              },
            ],
          },

          // Admin + Manager - Roster
          {
            element: <RoleBasedRoute allow={['admin', 'manager']} />,
            children: [
              {
                path: '/roster',
                children: [
                  { path: 'upload', element: <RosterUpload /> },
                  { path: 'results', element: <RosterResults /> },
                  { index: true, element: <Navigate to="upload" replace /> },
                ],
              },
            ],
          },

          // Admin only - Documents
          {
            element: <RoleBasedRoute allow={['admin']} />,
            children: [
              {
                path: '/documents',
                children: [
                  { path: 'templates', element: <DocumentTemplates /> },
                  { path: 'generate', element: <GenerateDocument /> },
                  { path: 'library', element: <DocumentLibrary /> },
                  { index: true, element: <Navigate to="templates" replace /> },
                ],
              },
            ],
          },

          // Admin only - Settings
          {
            element: <RoleBasedRoute allow={['admin']} />,
            children: [
              {
                path: '/settings',
                element: <Settings />,
              },
            ],
          },
        ],
      },
    ],
  },
]

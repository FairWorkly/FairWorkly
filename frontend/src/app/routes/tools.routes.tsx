import { Navigate, type RouteObject } from 'react-router-dom'
import { MainLayout } from '@/shared/components/layout/app/MainLayout'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'
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
                  {
                    path: 'upload',
                    lazy: async () => {
                      const { PayrollUpload } = await import(
                        '@/modules/payroll/pages/PayrollUpload'
                      )
                      return { Component: PayrollUpload }
                    },
                  },
                  {
                    path: 'results',
                    lazy: async () => {
                      const { PayrollResults } = await import(
                        '@/modules/payroll/pages/PayrollResults'
                      )
                      return { Component: PayrollResults }
                    },
                  },
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
                  {
                    path: 'upload',
                    lazy: async () => {
                      const { RosterUpload } = await import(
                        '@/modules/roster/pages/RosterUpload'
                      )
                      return { Component: RosterUpload }
                    },
                  },
                  {
                    path: 'results/:rosterId',
                    lazy: async () => {
                      const { RosterResults } = await import(
                        '@/modules/roster/pages/RosterResults'
                      )
                      return { Component: RosterResults }
                    },
                  },
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
                  {
                    path: 'templates',
                    lazy: async () => {
                      const { DocumentTemplates } = await import(
                        '@/modules/documents/pages/DocumentTemplates'
                      )
                      return { Component: DocumentTemplates }
                    },
                  },
                  {
                    path: 'generate',
                    lazy: async () => {
                      const { GenerateDocument } = await import(
                        '@/modules/documents/pages/GenerateDocument'
                      )
                      return { Component: GenerateDocument }
                    },
                  },
                  {
                    path: 'library',
                    lazy: async () => {
                      const { DocumentLibrary } = await import(
                        '@/modules/documents/pages/DocumentLibrary'
                      )
                      return { Component: DocumentLibrary }
                    },
                  },
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
                lazy: async () => {
                  const { Settings } = await import(
                    '@/modules/settings/pages/Settings'
                  )
                  return { Component: Settings }
                },
              },
            ],
          },
        ],
      },
    ],
  },
]

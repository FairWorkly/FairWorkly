import { Navigate, type RouteObject } from 'react-router-dom'
import { MainLayout } from '@/shared/components/layout/app/MainLayout'
import { ProtectedRoute } from '@/shared/components/guards/ProtectedRoute'

// Payroll pages
import { PayrollUpload } from '@/modules/payroll/pages/PayrollUpload'
import { PayrollResults } from '@/modules/payroll/pages/PayrollResults'

// Compliance pages
import { RosterUpload } from '@/modules/compliance/pages/RosterUpload'
import { RosterResults } from '@/modules/compliance/pages/RosterResults'

// Documents pages
import { DocumentTemplates } from '@/modules/documents/pages/DocumentTemplates'
import { GenerateDocument } from '@/modules/documents/pages/GenerateDocument'
import { DocumentLibrary } from '@/modules/documents/pages/DocumentLibrary'
import { RoleBasedRoute } from '@/shared/components/guards/RoleBasedRoute'

export const toolRoutes: RouteObject[] = [
  {
    // First Layer：handle loading, Auth, Deep Linking
    element: <ProtectedRoute />,
    children: [
      {
        // Second Layer: MainLayout ( Sidebar, Topbar, backgroud)
        element: <MainLayout />,
        children: [
          {
            path: '/payroll',
            children: [
              { path: 'upload', element: <PayrollUpload /> },
              { path: 'results', element: <PayrollResults /> },
              // this Navigate only for UX, when users access to /payroll then direct to upload page
              { index: true, element: <Navigate to="upload" replace /> },
            ],
          },

          {
            path: '/compliance',
            children: [
              { path: 'roster-upload', element: <RosterUpload /> },
              { path: 'roster-results', element: <RosterResults /> },
            ],
          },

          {
            path: '/documents',
            children: [
              { path: 'templates', element: <DocumentTemplates /> },
              { path: 'generate', element: <GenerateDocument /> },
              { path: 'library', element: <DocumentLibrary /> },
            ],
          },

          {
            element: <RoleBasedRoute allowedRoles={['admin']} />,
            children: [
              {
                path: '/settings/system',
                element: <div>Admin Only System Settings</div>,
              },
            ],
          },
        ],
      },
    ],
  },
]

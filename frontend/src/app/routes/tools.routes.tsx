import { type RouteObject, Navigate } from 'react-router-dom'
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

// Employee pages
import { MyProfile } from '@/modules/employee/pages/MyProfile'

export const toolRoutes: RouteObject[] = [
  // ==================== Payroll Agent ====================
  {
    path: '/payroll',
    children: [
      {
        index: true,
        element: <Navigate to="upload" replace />,
      },
      {
        path: 'upload',
        element: (
          <ProtectedRoute requiredModule="payroll">
            <PayrollUpload />
          </ProtectedRoute>
        ),
      },
      {
        path: 'results/:id',
        element: (
          <ProtectedRoute requiredModule="payroll">
            <PayrollResults />
          </ProtectedRoute>
        ),
      },
    ],
  },

  // ==================== Compliance Agent ====================
  {
    path: '/compliance',
    children: [
      {
        index: true,
        element: <Navigate to="upload" replace />,
      },
      {
        path: 'upload',
        element: (
          <ProtectedRoute requiredModule="compliance">
            <RosterUpload />
          </ProtectedRoute>
        ),
      },
      {
        path: 'results/:id',
        element: (
          <ProtectedRoute requiredModule="compliance">
            <RosterResults />
          </ProtectedRoute>
        ),
      },
    ],
  },

  // ==================== Documents Agent ====================
  {
    path: '/documents',
    children: [
      {
        index: true,
        element: (
          <ProtectedRoute requiredModule="documents">
            <DocumentTemplates />
          </ProtectedRoute>
        ),
      },
      {
        path: 'generate/:templateId',
        element: (
          <ProtectedRoute requiredModule="documents">
            <GenerateDocument />
          </ProtectedRoute>
        ),
      },
      {
        path: 'library',
        element: (
          <ProtectedRoute requiredModule="documents">
            <DocumentLibrary />
          </ProtectedRoute>
        ),
      },
    ],
  },

  // ==================== Employee Management ====================
  {
    path: 'my-profile',
    element: (
      <ProtectedRoute>
        <MyProfile />
      </ProtectedRoute>
    ),
  },
]

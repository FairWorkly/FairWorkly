import React from 'react'
import { Routes, Route } from 'react-router-dom'
import ComplianceRoutes from './compliance.routes.tsx'

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/compliance/*" element={<ComplianceRoutes />} />
            <Route path="*" element={<h1>404 not found</h1>} />
        </Routes>
    )
}

export default AppRoutes

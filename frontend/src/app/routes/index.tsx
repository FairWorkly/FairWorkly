import React from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import ComplianceRoutes from './compliance.routes.tsx'

const AppRoutes = () => {
    return (
        <Router>
            <Routes>
                <Route path="/compliance" element={<ComplianceRoutes />} />
                <Route path="*" element={<h1>404 not found</h1>} />
            </Routes>
        </Router>
    )
}

export default AppRoutes
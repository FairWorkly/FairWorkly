import React from 'react'
import { Routes, Route } from 'react-router-dom'
import ComplianceHome from '../../modules/compliance/pages/ComplianceHome.tsx'
import ComplianceQA from '../../modules/compliance/pages/ComplianceQA.tsx'
import RosterCheck from '../../modules/compliance/pages/RosterCheck.tsx'
import RosterResults from '../../modules/compliance/pages/RosterResults.tsx'

const ComplianceRoutes = () => {
    return (
        <Routes>
            <Route index element={<ComplianceHome />} />
            <Route path="qa" element={<ComplianceQA />} />
            <Route path="roster-check" element={<RosterCheck />} />
            <Route path="roster-results" element={<RosterResults />} />
        </Routes>
    )
}

export default ComplianceRoutes

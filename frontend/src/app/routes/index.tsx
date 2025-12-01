import React from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import ComplianceRoutes from './compliance.routes.tsx'

const AppRoutes = () => {
    return (
        <Router>
            <Routes>
                <Route path="/compliance" element={<ComplianceRoutes />} />
            </Routes>
        </Router>
    )
}

export default AppRoutes
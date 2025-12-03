import React from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import ComplianceRoutes from './compliance.routes.tsx'
import MainLayout from '../../shared/components/layout/MainLayout.tsx'

const AppRoutes = () => {
    return (
        <Router>
            <MainLayout>
                <Routes>
                    <Route path="/compliance/*" element={<ComplianceRoutes />} />
                    <Route path="*" element={<h1>404 not found</h1>} />
                </Routes>
            </MainLayout>
        </Router>
    )
}

export default AppRoutes

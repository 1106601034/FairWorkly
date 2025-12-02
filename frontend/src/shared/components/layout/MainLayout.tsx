import React from 'react'
import Sidebar from './Sidebar.tsx'
import Topbar from './Topbar.tsx'

type MainLayoutProps = {
    children: React.ReactNode
}

const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
    return (
        <div>
            <Sidebar />
            <div>
                <Topbar />
                <main>
                    {children}
                </main>
            </div>
        </div>
    )
}

export default MainLayout

import React from 'react'
import { NavLink } from 'react-router-dom'

const Sidebar = () => {
    return (
        <aside aria-label="Sidebar">
            <div>
                <div>Compliance</div>
                <nav>
                    <NavLink to="/compliance/qa" >
                        Compliance Q&A
                    </NavLink>
                </nav>
            </div>
        </aside>
    )
}

export default Sidebar

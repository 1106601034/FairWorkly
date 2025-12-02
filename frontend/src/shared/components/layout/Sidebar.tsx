import React from 'react'
import { NavLink } from 'react-router-dom'

const Sidebar = () => {
    return (
        <nav aria-label="Sidebar">
            <ul>
                <li>
                    <NavLink to="/compliance/qa">Compliance Q&A</NavLink>
                </li>
            </ul>
        </nav>
    )
}

export default Sidebar

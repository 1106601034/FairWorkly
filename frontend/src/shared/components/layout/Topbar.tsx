import React from 'react'
import { NavLink } from 'react-router-dom'

const Topbar = () => {
    return (
        <header>
            <nav aria-label="Top navigation">
                <NavLink to="/compliance/qa">Compliance Q&A</NavLink>
            </nav>
        </header>
    )
}

export default Topbar

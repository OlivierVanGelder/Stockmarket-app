import { Navigate } from 'react-router-dom'
import React from 'react'

interface ProtectedRouteProps {
    children: React.ReactNode
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
    const token = sessionStorage.getItem('token')

    if (!token) {
        return <Navigate to="/login" />
    }

    return <>{children}</> // Ensure valid JSX is returned
}

export default ProtectedRoute

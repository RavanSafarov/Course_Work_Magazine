import { Navigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export const PrivateRoute = ({ children, adminOnly = false, managerOnly = false }) => {
    const { user, isAdmin, isManager, loading } = useAuth();

    if (loading)
        return <div>Loading...</div>;
    if (!user)
        return <Navigate to="/auth" />;

    if (adminOnly && !isAdmin())
        return <Navigate to="/" />;
    if (managerOnly && !isManager() && !isAdmin())
        return <Navigate to="/" />;

    return children;
};
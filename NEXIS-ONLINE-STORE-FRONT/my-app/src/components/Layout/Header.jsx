import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export const Header = () => {
    const {user, logout, isAdmin, isManager} = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <header className="header">
            <div className="header-inner">
                <Link to="/" className="logo">Nexis Store
                </Link>
                <nav className="menu">
                    {user ? (
                        <>
                            <Link to="/">Home</Link>
                            <Link to="/cart">Cart</Link>
                            <Link to="/orders">Orders</Link>
                            <Link to="/profile">Profile</Link>
                            {isAdmin() && <Link to="/admin">Admin</Link>}

                            {isManager() && <Link to="/manager">Manager</Link>}
                            <button onClick={handleLogout} className="btn btn-danger">
                                Logout
                            </button>
                        </>
                    ) : null
                    }
                </nav>
            </div>
        </header>
    );
}
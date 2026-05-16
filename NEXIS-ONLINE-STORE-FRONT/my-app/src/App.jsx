import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { Layout } from './components/Layout/Layout';
import { PrivateRoute } from './components/Layout/PrivateRoute';
import { Home } from './pages/Home';
import { Auth } from './pages/Auth';
import { CartPage } from './pages/CartPage';
import { Orders } from './pages/Orders';
import { Profile } from './pages/Profile';
import { Admin } from './pages/Admin';
import {Product} from "./pages/Product.jsx";
import { Manager } from './pages/Manager';
import './App.css';
import './index.css'

const RequireAuth = ({ children }) => {
    const { user, loading } = useAuth();
    if (loading) return <p>Loading...</p>;
    if (!user) return <Navigate to="/auth" />;
    return children;
};

function App() {
    return (
        <AuthProvider>
            <Router>
                <Layout>
                    <Routes>
                        <Route path="/auth" element={<Auth />} />
                        <Route
                            path="/"
                            element={
                                <RequireAuth>
                                    <Home />
                                </RequireAuth>
                            }
                        />
                        <Route
                            path="/cart"
                            element={
                                <PrivateRoute>
                                    <CartPage />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/orders"
                            element={
                                <PrivateRoute>
                                    <Orders />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/profile"
                            element={
                                <PrivateRoute>
                                    <Profile />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/product/:id"
                            element={
                                <PrivateRoute>
                                    <Product />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/admin"
                            element={
                                <PrivateRoute adminOnly>
                                    <Admin />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/manager"
                            element={
                                <PrivateRoute managerOnly>
                                    <Manager />
                                </PrivateRoute>
                            }
                        />
                    </Routes>
                </Layout>
            </Router>
        </AuthProvider>
    );
}

export default App;
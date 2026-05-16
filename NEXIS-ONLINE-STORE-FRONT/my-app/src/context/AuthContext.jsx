import { createContext, useState, useEffect, useContext } from 'react';
import { authService } from '../services/auth';
import { setAuthRequest } from '../services/api';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [errorModal, setErrorModal] = useState({
        isOpen: false,
        title: '',
        message: ''
    });

    useEffect(() => {
        const init = () => {
            const token = localStorage.getItem('accessToken');
            const saved = localStorage.getItem('user');
            if (token && saved) {
                try {
                    setUser(JSON.parse(saved));
                } catch {
                    localStorage.removeItem('accessToken');
                    localStorage.removeItem('refreshToken');
                    localStorage.removeItem('user');
                }
            }
            setLoading(false);
        };
        init();
    }, []);

    const showError = (title, message) => {
        setErrorModal({
            isOpen: true,
            title,
            message
        });
    };

    const closeErrorModal = () => {
        setErrorModal({
            isOpen: false,
            title: '',
            message: ''
        });
    };

    const login = async (email, password) => {
        setAuthRequest(true);

        try {
            const res = await authService.login({ email, password });
            const data = res.data?.data;

            if (!data) {
                showError(
                    'Login Error',
                    'Invalid email or password. Please check your credentials.'
                );
                return { success: false, error: 'No data received' };
            }

            localStorage.setItem('accessToken', data.accessToken);
            localStorage.setItem('refreshToken', data.refreshToken);
            localStorage.setItem('user', JSON.stringify(data));

            setUser(data);
            return { success: true, data };

        } catch (error) {
            console.error('Login error:', error);

            const status = error.response?.status;

            if (status === 400) {
                showError(
                    'Login Error',
                    'Invalid email or password. Please check your credentials.'
                );
            } else {
                showError(
                    'Error',
                    'Unable to login. Please try again later.'
                );
            }

            return { success: false, error };
        } finally {
            setAuthRequest(false);
        }
    };

    const register = async (formData) => {
        setAuthRequest(true);

        try {
            const res = await authService.register(formData);
            const data = res.data.data;

            const userData = {
                ...data,
                name:
                    formData.name ||
                    formData.userName ||
                    data.name ||
                    data.email?.split('@')[0]
            };

            localStorage.setItem('accessToken', data.accessToken);
            localStorage.setItem('refreshToken', data.refreshToken);
            localStorage.setItem('user', JSON.stringify(userData));

            setUser(userData);
            return { success: true, data: userData };

        } catch (error) {
            console.error('Register error:', error);

            const status = error.response?.status;

            if (status === 409) {
                showError('Email Taken', 'A user with this email already exists.');
            } else {
                showError('Error', 'Registration failed. Please try again later.');
            }

            return { success: false, error };
        } finally {
            setAuthRequest(false);
        }
    };

    const logout = () => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        setUser(null);
    };

    const isAdmin = () => user?.roles?.includes('Admin') || false;
    const isManager = () => user?.roles?.includes('Manager') || false;

    const value = {
        user,
        setUser,
        login,
        register,
        logout,
        isAdmin,
        isManager,
        loading,
        errorModal,
        closeErrorModal,
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
            {errorModal.isOpen && (
                <>
                    <div
                        onClick={closeErrorModal}
                        style={{
                            position: 'fixed',
                            top: 0,
                            left: 0,
                            right: 0,
                            bottom: 0,
                            backgroundColor: 'rgba(0, 0, 0, 0.6)',
                            backdropFilter: 'blur(4px)',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            zIndex: 9999,
                            animation: 'fadeIn 0.2s ease-out',
                        }}
                    >
                        <div
                            onClick={(e) => e.stopPropagation()}
                            style={{
                                backgroundColor: 'white',
                                padding: '32px',
                                borderRadius: '16px',
                                maxWidth: '420px',
                                width: '90%',
                                boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.25)',
                                animation: 'slideUp 0.3s ease-out',
                            }}
                        >
                            <div style={{
                                textAlign: 'center',
                                marginBottom: '20px',
                                animation: 'shake 0.5s ease-in-out',
                            }}>
                                <div style={{
                                    width: '64px',
                                    height: '64px',
                                    backgroundColor: '#fef2f2',
                                    borderRadius: '50%',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    margin: '0 auto',
                                    border: '2px solid #fecaca',
                                }}>
                                    <svg
                                        width="32"
                                        height="32"
                                        viewBox="0 0 24 24"
                                        fill="none"
                                        stroke="#dc2626"
                                        strokeWidth="2.5"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    >
                                        <circle cx="12" cy="12" r="10"></circle>
                                        <line x1="12" y1="8" x2="12" y2="12"></line>
                                        <line x1="12" y1="16" x2="12.01" y2="16"></line>
                                    </svg>
                                </div>
                            </div>

                            <h3 style={{
                                margin: '0 0 12px 0',
                                color: '#111827',
                                fontSize: '20px',
                                fontWeight: '700',
                                textAlign: 'center',
                            }}>
                                {errorModal.title}
                            </h3>

                            <p style={{
                                margin: '0 0 24px 0',
                                color: '#6b7280',
                                lineHeight: '1.6',
                                textAlign: 'center',
                                fontSize: '15px',
                            }}>
                                {errorModal.message}
                            </p>

                            <button
                                onClick={closeErrorModal}
                                style={{
                                    width: '100%',
                                    padding: '14px',
                                    backgroundColor: '#dc2626',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '10px',
                                    cursor: 'pointer',
                                    fontSize: '15px',
                                    fontWeight: '600',
                                    transition: 'all 0.2s ease',
                                    boxShadow: '0 4px 6px -1px rgba(220, 38, 38, 0.3)',
                                }}
                                onMouseOver={(e) => {
                                    e.target.style.backgroundColor = '#b91c1c';
                                    e.target.style.transform = 'translateY(-1px)';
                                    e.target.style.boxShadow = '0 6px 8px -1px rgba(220, 38, 38, 0.4)';
                                }}
                                onMouseOut={(e) => {
                                    e.target.style.backgroundColor = '#dc2626';
                                    e.target.style.transform = 'translateY(0)';
                                    e.target.style.boxShadow = '0 4px 6px -1px rgba(220, 38, 38, 0.3)';
                                }}
                            >
                                Got it
                            </button>
                        </div>
                    </div>

                    <style>{`
                        @keyframes fadeIn {
                            from { opacity: 0; }
                            to { opacity: 1; }
                        }
                        @keyframes slideUp {
                            from { 
                                opacity: 0;
                                transform: translateY(20px) scale(0.95);
                            }
                            to { 
                                opacity: 1;
                                transform: translateY(0) scale(1);
                            }
                        }
                        @keyframes shake {
                            0%, 100% { transform: translateX(0); }
                            10%, 30%, 50%, 70%, 90% { transform: translateX(-4px); }
                            20%, 40%, 60%, 80% { transform: translateX(4px); }
                        }
                    `}</style>
                </>
            )}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
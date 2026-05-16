import api from './api';

export const authService = {
    login: (data) => api.post('/User/login', data),
    register: (data) => api.post('/User/register', data),
    getProfile: () => api.get('/User/profile'),
    updateProfile: (data) => api.put('/User/profile', data),
    refresh: (token) => api.post('/User/refresh', { refreshToken: token }),
    revoke: (token) => api.post('/User/revoke', { refreshToken: token }),
    updateBalance: (balance) => api.patch('/User/balance', { Balance: balance })
};
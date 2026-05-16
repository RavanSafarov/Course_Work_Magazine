import axios from 'axios';

const api = axios.create({
    baseURL: '/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

let isAuthRequest = false;

export const setAuthRequest = (value) => {
    isAuthRequest = value;
};

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        const status = error.response?.status;

        if (isAuthRequest && status === 500) {
            error.response.status = 401;
            error.response.data = {
                message: 'Invalid email or password'
            };
            return Promise.reject(error);
        }

        if (status === 401 && !isAuthRequest) {
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('user');
            window.location.href = '/auth';
        }

        return Promise.reject(error);
    }
);

export default api;
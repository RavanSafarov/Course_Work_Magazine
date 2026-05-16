import api from './api';

export const orderService = {
    getAll: () => api.get('/Order/all'),
    checkout: (data) => api.post('/Order/checkout', data),
    updateStatus: (id, status) => api.patch(`/Order/${id}/status?newStatus=${status}`),
    archive: (id) => api.patch(`/Order/${id}/archive`),
};
import api from './api';

export const productService = {
    getAll: () => api.get('/Product/all'),
    getById: (id) => api.get(`/Product/${id}`),
    create: (data) => api.post('/Product', data),
    update: (id, data) => api.put(`/Product/${id}`, data),
    delete: (id) => api.delete(`/Product/${id}`),
};
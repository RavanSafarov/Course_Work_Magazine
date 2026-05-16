import api from './api';

export const cartService = {
    getAll: () => api.get('/Basket'),
    add: (productId, quantity) => api.post('/Basket', { ProductId: productId, Quantity: quantity }),
    update: (id, ProductId, Quantity) => api.put(`/Basket/${id}`, { ProductId: ProductId, Quantity: Quantity }),
    delete: (id) => api.delete(`/Basket/${id}`),
};
import { useState, useEffect } from 'react';
import { productService } from '../services/product';
import { categoryService } from '../services/category';
import { orderService } from '../services/order';
import { OrderDetailsModal } from '../components/Orders/OrderDetailsModal';
import { Input } from '../components/UI/Input';
import { Button } from '../components/UI/Button';
import { Message } from '../components/UI/Message';

const ORDER_STATUSES = {
    0: { label: 'Created', color: 'warning', next: 1, action: 'Send' },
    1: { label: 'Sent', color: 'primary', next: 2, action: 'Receive' },
    2: { label: 'Received', color: 'info', next: 3, action: 'Mark Paid' },
    3: { label: 'Paid', color: 'success', next: null, action: null },
    4: { label: 'Cancelled', color: 'danger', next: null, action: null },
    5: { label: 'Rejected', color: 'danger', next: null, action: null },
};

const STORAGE_KEY = 'manager_order_statuses';
const ARCHIVE_KEY = 'manager_archived_orders';

const ConfirmModal = ({ isOpen, title, message, onConfirm, onCancel, confirmText = 'Confirm', cancelText = 'Cancel', type = 'danger' }) => {
    if (!isOpen) return null;

    return (
        <div className="modal-overlay" onClick={onCancel}>
            <div className="modal" onClick={e => e.stopPropagation()}>
                <h3 className="subtitle">{title}</h3>
                <p style={{ marginBottom: '20px', color: '#64748b' }}>{message}</p>
                <div className="flex gap" style={{ justifyContent: 'flex-end' }}>
                    <Button variant="secondary" onClick={onCancel}>{cancelText}</Button>
                    <Button variant={type} onClick={onConfirm}>{confirmText}</Button>
                </div>
            </div>
        </div>
    );
};

export const Manager = () => {
    const [activeTab, setActiveTab] = useState('products');
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [editingProduct, setEditingProduct] = useState(null);
    const [productForm, setProductForm] = useState({
        nameOfProduct: '',
        description: '',
        price: '',
        imageUrl: '',
        categoryId: '',
        sellerId: 1,
    });
    const [imagePreview, setImagePreview] = useState('');
    const [uploadingImage, setUploadingImage] = useState(false);
    const [orders, setOrders] = useState([]);
    const [selectedOrder, setSelectedOrder] = useState(null);
    const [orderFilter, setOrderFilter] = useState('all');
    const [archivedOrders, setArchivedOrders] = useState([]);
    const [message, setMessage] = useState(null);
    const [loading, setLoading] = useState(false);

    const [confirmModal, setConfirmModal] = useState({
        isOpen: false,
        title: '',
        message: '',
        onConfirm: () => {},
        type: 'danger'
    });

    useEffect(() => {
        loadData();
        const savedArchive = JSON.parse(localStorage.getItem(ARCHIVE_KEY) || '[]');
        setArchivedOrders(savedArchive);
    }, []);

    const showConfirm = (title, message, onConfirm, type = 'danger') => {
        setConfirmModal({
            isOpen: true,
            title,
            message,
            onConfirm: () => {
                onConfirm();
                setConfirmModal(prev => ({ ...prev, isOpen: false }));
            },
            type
        });
    };

    const closeConfirm = () => {
        setConfirmModal(prev => ({ ...prev, isOpen: false }));
    };

    const loadData = async () => {
        setLoading(true);
        try {
            const [prodRes, catRes, ordRes] = await Promise.all([
                productService.getAll(),
                categoryService.getAll(),
                orderService.getAll(),
            ]);

            setProducts(prodRes.data.data || []);
            setCategories(catRes.data.data || []);

            const freshOrders = ordRes.data.data || [];
            const savedStatuses = JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}');

            const statusMap = {
                'Created': 0,
                'Sent': 1,
                'Received': 2,
                'Paid': 3,
                'Cancelled': 4,
                'Rejected': 5
            };

            const mergedOrders = freshOrders.map(order => {
                const serverStatusNum = statusMap[order.status] ?? order.status;
                const localStatus = savedStatuses[order.id];

                if (localStatus !== undefined && localStatus !== serverStatusNum) {
                    savedStatuses[order.id] = serverStatusNum;
                    return { ...order, status: serverStatusNum };
                }

                if (localStatus === undefined) {
                    savedStatuses[order.id] = serverStatusNum;
                    return { ...order, status: serverStatusNum };
                }

                return { ...order, status: localStatus };
            });

            localStorage.setItem(STORAGE_KEY, JSON.stringify(savedStatuses));
            setOrders(mergedOrders);
        } catch (err) {
            console.error('Failed to load data:', err);
            setMessage({ type: 'error', text: 'Failed to load data' });
        } finally {
            setLoading(false);
        }
    };

    const handleImageUpload = async (e) => {
        const file = e.target.files[0];
        if (!file) return;

        if (!file.type.startsWith('image/')) {
            setMessage({ type: 'error', text: 'Only images allowed' });
            return;
        }

        if (file.size > 5 * 1024 * 1024) {
            setMessage({ type: 'error', text: 'Max file size is 5MB' });
            return;
        }

        const reader = new FileReader();
        reader.onloadend = () => setImagePreview(reader.result);
        reader.readAsDataURL(file);

        setUploadingImage(true);
        const formData = new FormData();
        formData.append('image', file);

        try {
            const token = localStorage.getItem('accessToken');
            const res = await fetch('/api/Product/upload-image', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                body: formData,
            });
            const data = await res.json();

            if (data.imageUrl) {
                setProductForm(prev => ({ ...prev, imageUrl: data.imageUrl }));
                setMessage({ type: 'success', text: 'Image uploaded' });
            }
        } catch (err) {
            setMessage({ type: 'error', text: 'Failed to upload image' });
        } finally {
            setUploadingImage(false);
        }
        setTimeout(() => setMessage(null), 3000);
    };

    const handleProductSubmit = async (e) => {
        e.preventDefault();
        try {
            const data = {
                ...productForm,
                price: Number(productForm.price),
            };

            if (editingProduct) {
                await productService.update(editingProduct.id, data);
                setMessage({ type: 'success', text: 'Product updated' });
            } else {
                await productService.create(data);
                setMessage({ type: 'success', text: 'Product created' });
            }

            resetProductForm();
            loadData();
        } catch (err) {
            console.error('Failed to save product:', err);
            setMessage({ type: 'error', text: 'Failed to save product' });
        }
        setTimeout(() => setMessage(null), 3000);
    };

    const editProduct = (product) => {
        setEditingProduct(product);
        setProductForm({
            nameOfProduct: product.nameOfProduct,
            description: product.description,
            price: product.price,
            imageUrl: product.imageUrl,
            categoryId: product.categoryId,
            sellerId: product.sellerId || 1,
        });
        setImagePreview(product.imageUrl || '');
    };

    const resetProductForm = () => {
        setEditingProduct(null);
        setProductForm({
            nameOfProduct: '',
            description: '',
            price: '',
            imageUrl: '',
            categoryId: '',
            sellerId: 1,
        });
        setImagePreview('');
    };

    const saveStatusToStorage = (orderId, status) => {
        const savedStatuses = JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}');
        savedStatuses[orderId] = status;
        localStorage.setItem(STORAGE_KEY, JSON.stringify(savedStatuses));
    };

    const handleStatusChange = async (orderId, newStatus) => {
        if (newStatus === undefined || newStatus === null)
            return;

        try {
            await orderService.updateStatus(orderId, newStatus);
            saveStatusToStorage(orderId, newStatus);

            setOrders(prev => prev.map(order =>
                order.id === orderId ? { ...order, status: newStatus } : order
            ));

            setMessage({ type: 'success', text: `Status updated to ${ORDER_STATUSES[newStatus].label}` });
        } catch (err) {
            console.error('Failed to update status:', err);
            setMessage({ type: 'error', text: 'Failed to update status' });
        }
        setTimeout(() => setMessage(null), 3000);
    };

    const handleCancelOrder = (orderId) => {
        showConfirm(
            'Cancel Order',
            'Cancel this order?',
            async () => {
                try {
                    await orderService.updateStatus(orderId, 4);
                    saveStatusToStorage(orderId, 4);

                    setOrders(prev => prev.map(order =>
                        order.id === orderId ? { ...order, status: 4 } : order
                    ));

                    setMessage({ type: 'success', text: 'Order cancelled' });
                } catch (err) {
                    console.error('Failed to cancel:', err);
                    setMessage({ type: 'error', text: 'Failed to cancel order' });
                }
                setTimeout(() => setMessage(null), 3000);
            }
        );
    };

    const handleRejectOrder = (orderId) => {
        showConfirm(
            'Reject Order',
            'Reject this order?',
            async () => {
                try {
                    await orderService.updateStatus(orderId, 5);
                    saveStatusToStorage(orderId, 5);

                    setOrders(prev => prev.map(order =>
                        order.id === orderId ? { ...order, status: 5 } : order
                    ));

                    setMessage({ type: 'success', text: 'Order rejected' });
                } catch (err) {
                    console.error('Failed to reject:', err);
                    setMessage({ type: 'error', text: 'Failed to reject order' });
                }
                setTimeout(() => setMessage(null), 3000);
            }
        );
    };

    const handleDeleteFromArchive = (orderId) => {
        showConfirm(
            'Delete Permanently',
            'Permanently delete this archived order?',
            () => {
                const newArchive = archivedOrders.filter(o => o.id !== orderId);
                localStorage.setItem(ARCHIVE_KEY, JSON.stringify(newArchive));
                setArchivedOrders(newArchive);
                setMessage({ type: 'success', text: 'Order permanently deleted' });
                setTimeout(() => setMessage(null), 3000);
            }
        );
    };

    const handleArchive = (orderId) => {
        showConfirm(
            'Archive Order',
            'Archive this order?',
            async () => {
                try {
                    const orderToArchive = orders.find(o => o.id === orderId);
                    if (!orderToArchive)
                        return;

                    const archivedOrder = {
                        ...orderToArchive,
                        archivedAt: new Date().toISOString(),
                        originalStatus: orderToArchive.status
                    };

                    const currentArchive = JSON.parse(localStorage.getItem(ARCHIVE_KEY) || '[]');
                    const newArchive = [archivedOrder, ...currentArchive];
                    localStorage.setItem(ARCHIVE_KEY, JSON.stringify(newArchive));
                    setArchivedOrders(newArchive);

                    const savedStatuses = JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}');
                    delete savedStatuses[orderId];
                    localStorage.setItem(STORAGE_KEY, JSON.stringify(savedStatuses));

                    setOrders(prev => prev.filter(order => order.id !== orderId));

                    try {
                        await orderService.archive(orderId);
                    } catch (e) {
                        console.log('API archive not available, using local only');
                    }

                    setMessage({ type: 'success', text: 'Order archived' });
                } catch (err) {
                    console.error('Failed to archive:', err);
                    setMessage({ type: 'error', text: 'Failed to archive' });
                }
                setTimeout(() => setMessage(null), 3000);
            }
        );
    };

    const handleClearArchive = () => {
        showConfirm(
            'Clear Archive',
            'Clear all archived orders? This cannot be undone!',
            () => {
                localStorage.removeItem(ARCHIVE_KEY);
                setArchivedOrders([]);
                setMessage({ type: 'success', text: 'Archive cleared' });
                setTimeout(() => setMessage(null), 3000);
            }
        );
    };

    const getStatusValue = (status) => {
        if (typeof status === 'number')
            return status;
        if (typeof status === 'string') {
            const map = {
                'created': 0,
                'sent': 1,
                'received': 2,
                'paid': 3,
                'cancelled': 4,
                'rejected': 5
            };
            return map[status.toLowerCase()] ?? parseInt(status, 10);
        }
        return 0;
    };

    const getStatusConfig = (status) => {
        const val = getStatusValue(status);
        return ORDER_STATUSES[val] || ORDER_STATUSES[0];
    };

    const filteredOrders = orders.filter(order => {
        const status = getStatusValue(order.status);
        switch (orderFilter) {
            case 'active':
                return status === 0 || status === 1 || status === 2;
            case 'cancelled':
                return status === 4;
            case 'rejected':
                return status === 5;
            case 'paid':
                return status === 3;
            default:
                return true;
        }
    });

    return (
        <div>
            <h1 className="title">Manager Panel</h1>
            {message && <Message type={message.type}>{message.text}</Message>}

            <ConfirmModal
                isOpen={confirmModal.isOpen}
                title={confirmModal.title}
                message={confirmModal.message}
                onConfirm={confirmModal.onConfirm}
                onCancel={closeConfirm}
                type={confirmModal.type}
            />

            <div className="admin-tabs">
                <button
                    className={`admin-tab ${activeTab === 'products' ? 'active' : ''}`}
                    onClick={() => setActiveTab('products')}
                >
                    Products
                </button>
                <button
                    className={`admin-tab ${activeTab === 'orders' ? 'active' : ''}`}
                    onClick={() => setActiveTab('orders')}
                >
                    Orders ({orders.length})
                </button>
                <button
                    className={`admin-tab ${activeTab === 'archive' ? 'active' : ''}`}
                    onClick={() => setActiveTab('archive')}
                >
                    Archive ({archivedOrders.length})
                </button>
            </div>

            {activeTab === 'products' && (
                <div>
                    <div className="card mb">
                        <h3 className="subtitle">{editingProduct ? 'Edit Product' : 'Add Product'}</h3>

                        {(imagePreview || productForm.imageUrl) && (
                            <img
                                src={imagePreview || productForm.imageUrl}
                                alt="Preview"
                                style={{ maxWidth: '200px', maxHeight: '200px', marginBottom: '16px', borderRadius: '8px', objectFit: 'cover' }}
                            />
                        )}

                        <form onSubmit={handleProductSubmit} className="form">
                            <div className="form-row">
                                <Input
                                    placeholder="Product Name"
                                    value={productForm.nameOfProduct}
                                    onChange={e => setProductForm({...productForm, nameOfProduct: e.target.value})}
                                    required
                                />
                                <Input
                                    type="number"
                                    placeholder="Price"
                                    value={productForm.price}
                                    onChange={e => setProductForm({...productForm, price: e.target.value})}
                                    required
                                />
                            </div>

                            <div className="form-row">
                                <select
                                    className="input"
                                    value={productForm.categoryId}
                                    onChange={e => setProductForm({...productForm, categoryId: e.target.value})}
                                    required
                                >
                                    <option value="">Select Category</option>
                                    {categories.map(cat => (
                                        <option key={cat.id} value={cat.id}>{cat.nameOfCategory}</option>
                                    ))}
                                </select>

                                <label className="btn btn-secondary" style={{ cursor: 'pointer', display: 'inline-flex', alignItems: 'center', gap: '8px' }}>
                                    {uploadingImage ? 'Uploading...' : (productForm.imageUrl ? 'Change Image' : 'Upload Image')}
                                    <input
                                        type="file"
                                        accept="image/*"
                                        onChange={handleImageUpload}
                                        hidden
                                        disabled={uploadingImage}
                                    />
                                </label>
                            </div>

                            <textarea
                                className="input"
                                placeholder="Description"
                                value={productForm.description}
                                onChange={e => setProductForm({...productForm, description: e.target.value})}
                                rows={3}
                            />

                            <div className="flex gap">
                                <Button type="submit" variant="primary">
                                    {editingProduct ? 'Update Product' : 'Create Product'}
                                </Button>
                                {editingProduct && (
                                    <Button type="button" variant="secondary" onClick={resetProductForm}>
                                        Cancel
                                    </Button>
                                )}
                            </div>
                        </form>
                    </div>

                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                            <tr>
                                <th>Image</th>
                                <th>Name</th>
                                <th>Price</th>
                                <th>Category</th>
                                <th>Actions</th>
                            </tr>
                            </thead>
                            <tbody>
                            {products.map(product => (
                                <tr key={product.id}>
                                    <td>
                                        {product.imageUrl ? (
                                            <img src={product.imageUrl} alt="" style={{width: '50px', height: '50px', objectFit: 'cover', borderRadius: '4px'}} />
                                        ) : (
                                            <span style={{color: '#94a3b8'}}>No image</span>
                                        )}
                                    </td>
                                    <td>{product.nameOfProduct}</td>
                                    <td>${product.price}</td>
                                    <td>{product.category?.nameOfCategory}</td>
                                    <td>
                                        <div className="flex gap">
                                            <Button variant="primary" onClick={() => editProduct(product)}>Edit</Button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}

            {activeTab === 'orders' && (
                <div className="orders-admin">
                    <h2 className="subtitle">Order Management</h2>

                    <div className="flex gap mb" style={{ marginBottom: '16px', flexWrap: 'wrap' }}>
                        <select
                            className="input"
                            value={orderFilter}
                            onChange={e => setOrderFilter(e.target.value)}
                            style={{ width: 'auto' }}
                        >
                            <option value="all">All Orders ({orders.length})</option>
                            <option value="active">Active (Created/Sent/Received)</option>
                            <option value="paid">Paid</option>
                            <option value="cancelled">Cancelled</option>
                            <option value="rejected">Rejected</option>
                        </select>

                        <Button variant="secondary" onClick={loadData} disabled={loading}>
                            {loading ? 'Loading...' : 'Refresh'}
                        </Button>
                    </div>

                    <div className="orders-table">
                        <table className="table">
                            <thead>
                            <tr>
                                <th>ID</th>
                                <th>Customer</th>
                                <th>Items</th>
                                <th>Total</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                            </thead>
                            <tbody>
                            {filteredOrders.map(order => {
                                const statusVal = getStatusValue(order.status);
                                const statusConfig = getStatusConfig(order.status);
                                const isFinal = statusVal === 3 || statusVal === 4 || statusVal === 5;

                                return (
                                    <tr key={order.id}>
                                        <td>#{order.id}</td>
                                        <td>
                                            <div>{order.userName || 'User #' + order.userId}</div>
                                            <small style={{color: '#666'}}>{new Date(order.createdAt).toLocaleDateString()}</small>
                                        </td>
                                        <td>{order.orderItems?.length || 0} items</td>
                                        <td><strong>${order.totalSum}</strong></td>
                                        <td>
                                                <span className={`status-badge status-${statusConfig.color}`}>
                                                    {statusConfig.label}
                                                </span>
                                        </td>
                                        <td>
                                            <div className="flex gap" style={{ flexWrap: 'wrap', gap: '4px' }}>
                                                {statusConfig.next !== null && (
                                                    <Button
                                                        variant="success"
                                                        onClick={() => handleStatusChange(order.id, statusConfig.next)}
                                                    >
                                                        {statusConfig.action}
                                                    </Button>
                                                )}

                                                {!isFinal && (
                                                    <>
                                                        <Button
                                                            variant="danger"
                                                            onClick={() => handleCancelOrder(order.id)}
                                                        >
                                                            Cancel
                                                        </Button>
                                                        <Button
                                                            variant="danger"
                                                            onClick={() => handleRejectOrder(order.id)}
                                                            style={{ opacity: 0.8 }}
                                                        >
                                                            Reject
                                                        </Button>
                                                    </>
                                                )}

                                                <Button
                                                    variant="secondary"
                                                    onClick={() => handleArchive(order.id)}
                                                >
                                                    Archive
                                                </Button>

                                                <Button
                                                    variant="primary"
                                                    onClick={() => setSelectedOrder(order)}
                                                >
                                                    View
                                                </Button>
                                            </div>
                                        </td>
                                    </tr>
                                );
                            })}
                            </tbody>
                        </table>

                        {filteredOrders.length === 0 && (
                            <div style={{textAlign: 'center', padding: '40px', color: '#666'}}>
                                No orders found for selected filter
                            </div>
                        )}
                    </div>

                    {selectedOrder && (
                        <OrderDetailsModal
                            order={selectedOrder}
                            onClose={() => setSelectedOrder(null)}
                            isAdmin={true}
                        />
                    )}
                </div>
            )}

            {activeTab === 'archive' && (
                <div className="archive-admin">
                    <h2 className="subtitle">Archived Orders</h2>

                    <div className="flex gap mb"
                         style={{marginBottom: '16px', justifyContent: 'space-between', alignItems: 'center'}}>
                        <div style={{color: '#666'}}>
                            Total archived: <strong>{archivedOrders.length}</strong>
                        </div>
                        {archivedOrders.length > 0 && (
                            <Button variant="danger" onClick={handleClearArchive}>
                                Clear All Archive
                            </Button>
                        )}
                    </div>

                    <div className="archive-table">
                        <table className="table">
                            <thead>
                            <tr>
                                <th>ID</th>
                                <th>Customer</th>
                                <th>Total</th>
                                <th>Original Status</th>
                                <th>Archived Date</th>
                                <th>Actions</th>
                            </tr>
                            </thead>
                            <tbody>
                            {archivedOrders.map(order => {
                                const statusConfig = getStatusConfig(order.originalStatus || order.status);

                                return (
                                    <tr key={order.id} style={{opacity: 0.7}}>
                                        <td>#{order.id}</td>
                                        <td>
                                            <div>{order.userName || 'User #' + order.userId}</div>
                                            <small style={{color: '#666'}}>
                                                Original: {new Date(order.createdAt).toLocaleDateString()}
                                            </small>
                                        </td>
                                        <td><strong>${order.totalSum}</strong></td>
                                        <td>
                                                <span className={`status-badge status-${statusConfig.color}`}>
                                                    {statusConfig.label}
                                                </span>
                                        </td>
                                        <td>
                                            {new Date(order.archivedAt).toLocaleString()}
                                        </td>
                                        <td>
                                            <div className="flex gap" style={{flexWrap: 'wrap', gap: '4px'}}>
                                                <Button
                                                    variant="primary"
                                                    onClick={() => setSelectedOrder(order)}
                                                >
                                                    View
                                                </Button>
                                                <Button
                                                    variant="danger"
                                                    onClick={() => handleDeleteFromArchive(order.id)}
                                                >
                                                    Delete
                                                </Button>
                                            </div>
                                        </td>
                                    </tr>
                                );
                            })}
                            </tbody>
                        </table>

                        {archivedOrders.length === 0 && (
                            <div style={{textAlign: 'center', padding: '60px', color: '#666'}}>
                                <div style={{fontSize: '48px', marginBottom: '16px'}}>📦</div>
                                <h3>No archived orders</h3>
                                <p>Archived orders will appear here</p>
                            </div>
                        )}
                    </div>

                    {selectedOrder && (
                        <OrderDetailsModal
                            order={selectedOrder}
                            onClose={() => setSelectedOrder(null)}
                            isAdmin={true}
                        />
                    )}
                </div>
            )}
        </div>
    );
};
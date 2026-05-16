import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { orderService } from '../services/order';
import { productService } from '../services/product';
import { OrderDetailsModal } from '../components/Orders/OrderDetailsModal';
import { Button } from '../components/UI/Button';
import { Message } from '../components/UI/Message';

export const Orders = () => {
    const [orders, setOrders] = useState([]);
    const [recommendations, setRecommendations] = useState([]);
    const [selectedOrder, setSelectedOrder] = useState(null);
    const [loading, setLoading] = useState(true);
    const [message, setMessage] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        loadOrders();
    }, []);

    const loadOrders = async () => {
        try {
            setLoading(true);
            const res = await orderService.getAll();
            const freshOrders = res.data.data || [];

            const statusMap = {
                'Created': 0,
                'Sent': 1,
                'Received': 2,
                'Paid': 3,
                'Cancelled': 4,
                'Rejected': 5
            };

            const ordersWithNumericStatus = freshOrders.map(order => ({
                ...order,
                status: statusMap[order.status] ?? order.status
            }));

            setOrders(ordersWithNumericStatus);

            if (!freshOrders || freshOrders.length === 0) {
                loadRecommendations();
            }
        } catch (err) {
            console.error('Failed to load orders:', err);
            setMessage({ type: 'error', text: 'Failed to load orders' });
        } finally {
            setLoading(false);
        }
    };

    const loadRecommendations = async () => {
        try {
            const res = await productService.getAll();
            const allProducts = res.data.data || [];
            const shuffled = allProducts.sort(() => 0.5 - Math.random());
            setRecommendations(shuffled.slice(0, 4));
        } catch (err) {
            console.error('Failed to load recommendations:', err);
        }
    };

    const getStatusString = (status) => {
        if (typeof status === 'number') {
            const map = {
                0: 'Created',
                1: 'Sent',
                2: 'Received',
                3: 'Paid',
                4: 'Cancelled',
                5: 'Rejected'
            };
            return map[status] || 'Unknown';
        }
        return status;
    };

    const getStatusColor = (status) => {
        const statusStr = getStatusString(status);
        const colors = {
            'Created': 'warning',
            'Sent': 'primary',
            'Received': 'info',
            'Paid': 'success',
            'Cancelled': 'danger',
            'Rejected': 'danger',
        };
        return colors[statusStr] || 'secondary';
    };

    if (loading)
        return <div className="loading">Loading...</div>;

    return (
        <div>
            <h1 className="title">My Orders</h1>
            {message && <Message type={message.type}>{message.text}</Message>}

            {orders.length === 0 ? (
                <div className="empty-state">
                    <div className="empty-orders">
                        <div className="empty-icon">📦</div>
                        <h3>No orders yet</h3>
                        <p>Make your first purchase and see your orders here</p>
                        <Button variant="primary" onClick={() => navigate('/')}>
                            Start Shopping
                        </Button>
                    </div>

                    {recommendations.length > 0 && (
                        <div className="recommendations-section">
                            <h3 className="recommendations-title">
                                <span>🔥</span> Popular Now
                            </h3>
                            <div className="recommendations-grid">
                                {recommendations.map(product => (
                                    <div key={product.id} className="recommendation-card">
                                        <div className="recommendation-img-wrapper">
                                            <img
                                                src={product.imageUrl || '/placeholder.png'}
                                                alt={product.nameOfProduct}
                                                className="recommendation-img"
                                            />
                                        </div>
                                        <div className="recommendation-info">
                                            <h4 className="recommendation-name">{product.nameOfProduct}</h4>
                                            <p className="recommendation-price">${product.price}</p>
                                            <Button
                                                variant="primary"
                                                onClick={() => navigate('/')}
                                                className="btn-add-cart-sm"
                                            >
                                                View Product
                                            </Button>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            ) : (
                <div className="orders-list">
                    {orders.map(order => {
                        const displayStatus = getStatusString(order.status);
                        return (
                            <div key={order.id} className="order-card">
                                <div className="order-header">
                                    <div>
                                        <h3>Order #{order.id}</h3>
                                        <span className={`status-badge status-${getStatusColor(order.status)}`}>
                                            {displayStatus}
                                        </span>
                                    </div>
                                    <div className="order-total">${order.totalSum}</div>
                                </div>

                                <div className="order-items-preview">
                                    {order.orderItems?.slice(0, 2).map(item => (
                                        <span key={item.id}>{item.productName} × {item.quantity}</span>
                                    ))}
                                    {order.orderItems?.length > 2 && <span>+{order.orderItems.length - 2} more</span>}
                                </div>

                                <div className="order-footer">
                                    <span>{new Date(order.createdAt).toLocaleDateString()}</span>
                                    <Button variant="primary" onClick={() => setSelectedOrder(order)}>
                                        View Details
                                    </Button>
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}

            {selectedOrder && (
                <OrderDetailsModal
                    order={selectedOrder}
                    onClose={() => setSelectedOrder(null)}
                />
            )}
        </div>
    );
};
import { Button } from '../UI/Button';

export const OrderDetailsModal = ({ order, onClose, isAdmin = false, onStatusChange }) => {
    const statusSteps = [
        { key: 'Created', label: 'Order Placed', icon: '📝' },
        { key: 'Sent', label: 'Sent', icon: '🚚' },
        { key: 'Received', label: 'Received', icon: '📦' },
        { key: 'Paid', label: 'Paid', icon: '✅' },
    ];

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

    const displayStatus = getStatusString(order.status);
    const statusIndex = ['Created', 'Sent', 'Received', 'Paid'].indexOf(displayStatus);
    const currentStepIndex = statusIndex >= 0 ? statusIndex : -1;
    const progress = displayStatus === 'Cancelled' || displayStatus === 'Rejected'
        ? 0
        : statusIndex >= 0 ? ((statusIndex + 1) / statusSteps.length) * 100 : 0;

    const isFinal = () => {
        const s = typeof order.status === 'number' ? order.status : parseInt(order.status, 10);
        return s === 3 || s === 4 || s === 5;
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal modal-large" onClick={e => e.stopPropagation()}>
                <button className="modal-close" onClick={onClose}>×</button>

                <h2 className="subtitle">Order #{order.id}</h2>

                {!isFinal() && currentStepIndex >= 0 && (
                    <div className="tracking-progress">
                        <div className="progress-bar">
                            <div className="progress-fill" style={{ width: `${progress}%` }}></div>
                        </div>
                        <div className="progress-steps">
                            {statusSteps.map((step, index) => (
                                <div
                                    key={step.key}
                                    className={`progress-step ${index <= currentStepIndex ? 'active' : ''} ${index === currentStepIndex ? 'current' : ''}`}
                                >
                                    <div className="step-icon">{step.icon}</div>
                                    <div className="step-label">{step.label}</div>
                                </div>
                            ))}
                        </div>
                    </div>
                )}

                {displayStatus === 'Cancelled' && (
                    <div className="cancelled-banner" style={{
                        background: '#fee2e2',
                        color: '#dc2626',
                        padding: '12px',
                        borderRadius: '8px',
                        textAlign: 'center',
                        marginBottom: '20px',
                        fontWeight: 'bold'
                    }}>
                        ❌ Order Cancelled
                    </div>
                )}

                {displayStatus === 'Rejected' && (
                    <div className="rejected-banner" style={{
                        background: '#fef3c7',
                        color: '#d97706',
                        padding: '12px',
                        borderRadius: '8px',
                        textAlign: 'center',
                        marginBottom: '20px',
                        fontWeight: 'bold'
                    }}>
                        🚫 Order Rejected
                    </div>
                )}

                {displayStatus === 'Paid' && (
                    <div className="paid-banner" style={{
                        background: '#d1fae5',
                        color: '#059669',
                        padding: '12px',
                        borderRadius: '8px',
                        textAlign: 'center',
                        marginBottom: '20px',
                        fontWeight: 'bold'
                    }}>
                        ✅ Order Completed (Paid)
                    </div>
                )}

                <div className="order-details-section">
                    <h4>Items</h4>
                    {order.orderItems?.map(item => (
                        <div key={item.id} className="detail-item">
                            <img src={item.productImageUrl || '/placeholder.jpg'} alt="" />
                            <div>
                                <div className="detail-name">{item.productName}</div>
                                <div className="detail-qty">Qty: {item.quantity} × ${item.rate}</div>
                            </div>
                            <div className="detail-sum">${item.sum}</div>
                        </div>
                    ))}
                </div>

                <div className="order-info-grid">
                    <div>
                        <label>Order Date</label>
                        <div>{new Date(order.createdAt).toLocaleDateString()}</div>
                    </div>
                    <div>
                        <label>Status</label>
                        <div className={`status-badge status-${getStatusColor(order.status)}`}>
                            {displayStatus}
                        </div>
                    </div>
                    <div>
                        <label>Total</label>
                        <div className="detail-total">${order.totalSum}</div>
                    </div>
                    <div>
                        <label>Seller</label>
                        <div>{order.sellerName || 'Unknown'}</div>
                    </div>
                </div>

                {order.comment && (
                    <div className="order-comment">
                        <label>Comment</label>
                        <div>{order.comment}</div>
                    </div>
                )}

                <div className="modal-actions" style={{ display: 'flex', gap: '10px', marginTop: '20px', flexWrap: 'wrap' }}>
                    {isAdmin && onStatusChange && !isFinal() && (
                        <Button variant="primary" onClick={() => onStatusChange(order)}>
                            Manage Status
                        </Button>
                    )}

                    <Button variant="secondary" onClick={onClose}>
                        Close
                    </Button>
                </div>
            </div>
        </div>
    );
};
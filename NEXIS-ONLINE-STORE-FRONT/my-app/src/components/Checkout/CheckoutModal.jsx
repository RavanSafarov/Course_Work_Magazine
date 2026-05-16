import { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { orderService } from '../../services/order';
import { Button } from '../UI/Button';
import { Input } from '../UI/Input';
import { useNavigate } from 'react-router-dom';

export const CheckoutModal = ({ items, total, onClose }) => {
    const { user, setUser } = useAuth();
    const navigate = useNavigate();
    const currentBalance = user?.balance ?? user?.Balance ?? 0;
    const [step, setStep] = useState(1);
    const [loading, setLoading] = useState(false);
    const [comment, setComment] = useState('');
    const [paymentMethod, setPaymentMethod] = useState('balance');
    const [errorModal, setErrorModal] = useState(null);
    const [successModal, setSuccessModal] = useState(null);

    const handleCheckout = async () => {
        if (currentBalance < total) {
            setErrorModal({
                title: 'Insufficient Balance',
                message: `You need $${(total - currentBalance).toFixed(2)} more to complete this purchase.`,
                subMessage: `Your balance: $${currentBalance.toFixed(2)}`
            });
            return;
        }

        setLoading(true);
        try {
            const res = await orderService.checkout({
                items: items.map(item => ({
                    productId: item.productId,
                    quantity: item.quantity,
                    price: item.productPrice
                })),
                totalSum: total,
                comment: comment,
            });

            if (res.status === 200 || res.status === 201 || res.data?.success) {

                const newBalance = currentBalance - total;
                const updatedUser = {
                    ...user,
                    balance: newBalance,
                    Balance: newBalance
                };

                setUser(updatedUser);
                localStorage.setItem('user', JSON.stringify(updatedUser));

                setSuccessModal({
                    title: 'Order Successful!',
                    message: 'Your order has been successfully placed.',
                    buttonText: 'Go to Orders',
                });
            } else {
                throw new Error(res.data?.message || 'Server declined the order');
            }
        } catch (err) {
            console.error('Checkout error:', err);
            setErrorModal({
                title: 'Order Error',
                message: err.response?.data?.message || 'Failed to place order.',
                subMessage: 'Your balance remains unchanged.'
            });
        } finally {
            setLoading(false);
        }
    };

    const handleCloseError = () => setErrorModal(null);
    const handleGoToOrders = () => {
        onClose();
        navigate('/orders');
    };

    if (errorModal) {
        return (
            <div className="modal-overlay">
                <div className="modal error-modal">
                    <div className="error-icon" style={{fontSize: '40px'}}>⚠️</div>
                    <h2>{errorModal.title}</h2>
                    <p>{errorModal.message}</p>
                    {errorModal.subMessage && <p className="sub-message" style={{color: '#ef4444'}}>{errorModal.subMessage}</p>}
                    <Button variant="primary" onClick={handleCloseError}>Close</Button>
                </div>
            </div>
        );
    }

    if (successModal) {
        return (
            <div className="modal-overlay">
                <div className="modal success-modal">
                    <div className="success-icon" style={{fontSize: '40px'}}>🎉</div>
                    <h2>{successModal.title}</h2>
                    <p>{successModal.message}</p>
                    <Button variant="primary" onClick={handleGoToOrders}>{successModal.buttonText}</Button>
                </div>
            </div>
        );
    }

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal" onClick={e => e.stopPropagation()}>
                <button className="modal-close" onClick={onClose}>×</button>
                <h2 className="subtitle">Checkout</h2>

                {step === 1 && (
                    <>
                        <div className="checkout-summary">
                            <h3>Order Summary</h3>
                            <div className="checkout-items-list">
                                {items.map(item => (
                                    <div key={item.id} className="checkout-item">
                                        <span>{item.productName} × {item.quantity}</span>
                                        <span>${(item.productPrice * item.quantity).toFixed(2)}</span>
                                    </div>
                                ))}
                            </div>
                            <div className="checkout-total">
                                <strong>Total: ${total.toFixed(2)}</strong>
                            </div>
                        </div>
                        <Input
                            placeholder="Comment (phone number, address, etc.)"
                            value={comment}
                            onChange={e => setComment(e.target.value)}
                        />
                        <Button variant="primary" onClick={() => setStep(2)}>Next: Payment</Button>
                    </>
                )}

                {step === 2 && (
                    <>
                        <div className="payment-methods">
                            <h3>Payment Method</h3>
                            <label className={`payment-option ${paymentMethod === 'balance' ? 'active' : ''}`}>
                                <input
                                    type="radio"
                                    checked={paymentMethod === 'balance'}
                                    onChange={() => setPaymentMethod('balance')}
                                />
                                <div className="payment-info">
                                    <strong>My Balance</strong>
                                    <span>${currentBalance.toLocaleString()} Available</span>
                                </div>
                            </label>
                        </div>
                        <div className="checkout-total-final">
                            <strong>To Pay: ${total.toFixed(2)}</strong>
                        </div>
                        <div className="modal-actions">
                            <Button variant="secondary" onClick={() => setStep(1)} disabled={loading}>Back</Button>
                            <Button variant="success" onClick={handleCheckout} disabled={loading}>
                                {loading ? 'Processing...' : 'Confirm Order'}
                            </Button>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
};
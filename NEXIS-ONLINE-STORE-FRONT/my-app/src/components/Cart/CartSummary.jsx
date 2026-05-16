import { useState } from 'react';
import { Button } from '../UI/Button';
import { CheckoutModal } from '../Checkout/CheckoutModal';

export const CartSummary = ({ items, onCheckoutComplete }) => {
    const [showCheckout, setShowCheckout] = useState(false);

    const total = items.reduce((sum, item) => sum + item.productPrice * item.quantity, 0);
    const count = items.reduce((sum, item) => sum + item.quantity, 0);

    const handleSuccess = () => {
        setShowCheckout(false);
        onCheckoutComplete?.();
    };

    return (
        <>
            <div className="card">
                <h3 className="subtitle">Order Summary</h3>
                <p>Items: {count}</p>
                <p className="product-price">Total: ${total.toFixed(2)}</p>
                <Button
                    variant="success"
                    className="mt"
                    onClick={() => setShowCheckout(true)}
                    disabled={items.length === 0}
                >
                    Checkout
                </Button>
            </div>

            {showCheckout && (
                <CheckoutModal
                    items={items}
                    total={total}
                    onClose={() => setShowCheckout(false)}
                    onSuccess={handleSuccess}
                />
            )}
        </>
    );
};
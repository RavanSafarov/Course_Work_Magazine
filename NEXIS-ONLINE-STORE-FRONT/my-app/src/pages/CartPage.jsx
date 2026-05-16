import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { cartService } from '../services/cart';
import { productService } from '../services/product';
import { CartItem } from '../components/Cart/CartItem';
import { CartSummary } from '../components/Cart/CartSummary';
import { Message } from '../components/UI/Message';
import { Button } from '../components/UI/Button';

export const CartPage = () => {
    const [items, setItems] = useState([]);
    const [recommendations, setRecommendations] = useState([]);
    const [message, setMessage] = useState(null);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        loadCart();
    }, []);

    const loadCart = async () => {
        try {
            const res = await cartService.getAll();
            setItems(res.data.data);

            if (!res.data.data || res.data.data.length === 0) {
                loadRecommendations();
            }
        } catch {
            setMessage({ type: 'error', text: 'Failed to load cart' });
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

    const updateQuantity = async (id, productId, quantity) => {
        if (quantity < 1) return;
        try {
            await cartService.update(id, productId, quantity);
            loadCart();
        } catch (err) {
            setMessage({ type: 'error', text: err.response?.data?.message || 'Failed to update' });
        }
    };

    const removeItem = async (id) => {
        try {
            await cartService.delete(id);
            loadCart();
        } catch {
            setMessage({ type: 'error', text: 'Failed to remove item' });
        }
    };

    const viewProduct = (productId) => {
        navigate(`/product/${productId}`);
    };

    if (loading)
        return <div className="loading">Loading...</div>;

    return (
        <div>
            <h1 className="title">Shopping Cart</h1>
            {message && <Message type={message.type}>{message.text}</Message>}

            {items.length === 0 ? (
                <div className="empty-state">
                    <div className="empty-cart">
                        <div className="empty-icon">🛒</div>
                        <h3>Your cart is empty</h3>
                        <p>Browse our catalog and find something you like</p>
                        <Button variant="primary" onClick={() => navigate('/')}>
                            Go to Shop
                        </Button>
                    </div>

                    {recommendations.length > 0 && (
                        <div className="recommendations-section">
                            <h3 className="recommendations-title">
                                <span>✨</span> Recommended for You
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
                                                onClick={() => viewProduct(product.id)}
                                                className="btn-view-product-sm"
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
                <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '20px' }}>
                    <div>
                        {items.map((item) => (
                            <CartItem
                                key={item.id}
                                item={item}
                                onUpdate={updateQuantity}
                                onRemove={removeItem}
                            />
                        ))}
                    </div>
                    <CartSummary items={items} />
                </div>
            )}
        </div>
    );
};
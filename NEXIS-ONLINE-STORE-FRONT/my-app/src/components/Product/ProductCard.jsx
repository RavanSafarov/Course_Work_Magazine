import { useAuth } from '../../context/AuthContext';
import { cartService } from '../../services/cart';
import { Button } from '../UI/Button';
import { useState, useEffect } from 'react';
import { Message } from '../UI/Message';
import { useNavigate } from 'react-router-dom';
import { toggleFavorite, isFavorite } from '../../services/favorites';

export const ProductCard = ({ product }) => {
    const { user } = useAuth();
    const navigate = useNavigate();
    const [message, setMessage] = useState(null);
    const [favorite, setFavorite] = useState(false);

    const productId = product?.id || product?.productId;

    useEffect(() => {
        if (productId && user) {
            setFavorite(isFavorite(productId, user));
        }
    }, [productId, user]);

    const addToCart = async (e) => {
        e.stopPropagation();
        if (!user) {
            setMessage({ type: 'error', text: 'Please login first' });
            return;
        }
        try {
            await cartService.add(productId, 1);
            setMessage({ type: 'success', text: 'Added to cart!' });
            setTimeout(() => setMessage(null), 3000);
        } catch {
            setMessage({ type: 'error', text: 'Error adding to cart' });
        }
    };

    const handleFavorite = (e) => {
        e.stopPropagation();
        toggleFavorite(product, user);
        setFavorite(!favorite);
    };

    const goToProduct = () => {
        if (!productId) {
            console.error('No product ID!', product);
            return;
        }
        navigate(`/product/${productId}`);
    };

    return (
        <div className="product" onClick={goToProduct}>
            <img
                src={product.imageUrl}
                alt={product.nameOfProduct}
                className="product-img"
            />
            <div className="product-body">
                <h3 className="product-title">{product.nameOfProduct}</h3>
                <p className="product-desc">{product.description}</p>
                <p className="product-price">${product.price}</p>
                <p className="product-category">{product.category?.nameOfCategory}</p>

                {message && <Message type={message.type}>{message.text}</Message>}

                {user && (
                    <>
                        <Button onClick={addToCart} className="mt">
                            Add to Cart
                        </Button>

                        <Button onClick={handleFavorite} className="mt">
                            {favorite ? '❤️ Remove Favorite' : '🤍 Add Favorite'}
                        </Button>
                    </>
                )}
            </div>
        </div>
    );
};
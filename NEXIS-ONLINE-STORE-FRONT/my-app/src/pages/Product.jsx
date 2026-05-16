import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { productService } from '../services/product';
import { cartService } from '../services/cart';
import { Button } from '../components/UI/Button';
import { Message } from '../components/UI/Message';

export const Product = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [product, setProduct] = useState(null);
    const [similarProducts, setSimilarProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [message, setMessage] = useState(null);
    const [quantity, setQuantity] = useState(1);

    useEffect(() => {
        loadProduct();
        loadSimilarProducts();
    }, [id]);

    const loadProduct = async () => {
        try {
            const res = await productService.getById(id);
            let productData = res.data?.data || res.data;

            if (!productData.category) {
                const allRes = await productService.getAll();
                const allProducts = allRes.data?.data || allRes.data || [];
                const productFromList = allProducts.find(p => p.id === Number(id));
                if (productFromList?.category) {
                    productData = { ...productData, category: productFromList.category };
                }
            }

            setProduct(productData);
        } catch (err) {
            setMessage({ type: 'error', text: 'Failed to load product' });
        } finally {
            setLoading(false);
        }
    };

    const loadSimilarProducts = async () => {
        try {
            const res = await productService.getAll();
            const allProducts = res.data?.data || res.data || [];
            const filtered = allProducts.filter(p => p.id !== Number(id));
            const shuffled = filtered.sort(() => 0.5 - Math.random());
            setSimilarProducts(shuffled.slice(0, 4));
        } catch (err) {
            console.error('Failed to load similar products:', err);
        }
    };

    const addToCart = async () => {
        if (!product?.id) return;
        try {
            await cartService.add(product.id, quantity);
            setMessage({ type: 'success', text: 'Added to cart!' });
            setTimeout(() => setMessage(null), 2000);
        } catch (err) {
            setMessage({ type: 'error', text: 'Failed to add to cart' });
        }
    };

    const goToCart = () => {
        navigate('/cart');
    };

    const goToProduct = (productId) => {
        navigate(`/product/${productId}`);
        window.scrollTo(0, 0);
    };

    if (loading) return <div className="loading">Loading...</div>;
    if (!product) return <div className="error">Product not found</div>;

    return (
        <div className="product-page">
            {message && <Message type={message.type}>{message.text}</Message>}

            <div className="product-container">
                <div className="product-gallery">
                    <div className="main-image">
                        <img
                            src={product.imageUrl || '/placeholder.png'}
                            alt={product.nameOfProduct}
                        />
                    </div>
                </div>
                <div className="product-info">
                    <h1 className="product-title">{product.nameOfProduct}</h1>
                    <div className="product-price">
                        <span className="price">${product.price}</span>
                    </div>
                    <div className="product-category">
                        CATEGORY: {product.category?.nameOfCategory || 'UNCATEGORIZED'}
                    </div>
                    <div className="product-description">
                        <h3>Description</h3>
                        <p>{product.description || 'No description available'}</p>
                    </div>
                    <div className="quantity-selector">
                        <label>Quantity:</label>
                        <div className="quantity-controls">
                            <button
                                onClick={() => setQuantity(Math.max(1, quantity - 1))}
                                className="qty-btn"
                            >-</button>
                            <span className="qty-value">{quantity}</span>
                            <button
                                onClick={() => setQuantity(quantity + 1)}
                                className="qty-btn"
                            >+</button>
                        </div>
                    </div>
                    <div className="product-actions">
                        <Button
                            variant="primary"
                            onClick={addToCart}
                            className="btn-add-cart"
                        >
                            Add to Cart
                        </Button>
                        <Button
                            variant="secondary"
                            onClick={goToCart}
                            className="btn-view-cart"
                        >
                            View Cart
                        </Button>
                    </div>
                </div>
            </div>

            {similarProducts.length > 0 && (
                <div className="similar-products-section">
                    <h2 className="similar-title">Similar Products</h2>
                    <div className="similar-products-grid">
                        {similarProducts.map(similarProduct => (
                            <div
                                key={similarProduct.id}
                                className="similar-product-card"
                                onClick={() => goToProduct(similarProduct.id)}
                            >
                                <div className="similar-img-wrapper">
                                    <img
                                        src={similarProduct.imageUrl || '/placeholder.png'}
                                        alt={similarProduct.nameOfProduct}
                                        className="similar-img"
                                    />
                                </div>
                                <div className="similar-info">
                                    <h4 className="similar-name">{similarProduct.nameOfProduct}</h4>
                                    <p className="similar-price">${similarProduct.price}</p>
                                    <Button
                                        variant="primary"
                                        onClick={(e) => {
                                            e.stopPropagation();
                                            goToProduct(similarProduct.id);
                                        }}
                                        className="btn-view-similar"
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
    );
};
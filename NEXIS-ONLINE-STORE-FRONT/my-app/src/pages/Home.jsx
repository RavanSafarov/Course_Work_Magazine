import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { productService } from '../services/product';
import { categoryService } from '../services/category';
import { ProductList } from '../components/Product/ProductList';
import { Message } from '../components/UI/Message';

export const Home = () => {
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [message, setMessage] = useState(null);
    const [loading, setLoading] = useState(true);
    const { user } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (!user) {
            navigate('/auth');
            return;
        }
        loadData();
    }, [user, navigate]);

    const loadData = async () => {
        try {
            const [prodRes, catRes] = await Promise.all([
                productService.getAll(),
                categoryService.getAll(),
            ]);
            setProducts(prodRes.data.data);
            setCategories(catRes.data.data);
        } catch (err) {
            setMessage({ type: 'error', text: 'Failed to load data' });
        } finally {
            setLoading(false);
        }
    };

    if (!user)
        return null;
    if (loading)
        return <p>Loading...</p>;

    return (
        <div>
            <h1 className="title">Products</h1>
            {message && <Message type={message.type}>{message.text}</Message>}
            <ProductList products={products} categories={categories}/>

            <footer className="footer">
                <p className="footer-desc">
                    Nexis Store — online marketplace for quality products.
                    Student project built with React & ASP.NET.
                </p>
                <p className="footer-contact">Contact: +994 70 700 70 70</p>
                <p className="footer-copy">&copy; {new Date().getFullYear()} Nexis Store</p>
            </footer>
        </div>
    );
};
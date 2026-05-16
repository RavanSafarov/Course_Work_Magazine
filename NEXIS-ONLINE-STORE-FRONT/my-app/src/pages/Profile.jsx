import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/auth';
import { orderService } from '../services/order';
import { Input } from '../components/UI/Input';
import { Button } from '../components/UI/Button';
import { Message } from '../components/UI/Message';
import { getFavorites } from '../services/favorites';
import { useNavigate } from 'react-router-dom';

export const Profile = () => {
    const { user, setUser } = useAuth();
    const navigate = useNavigate();
    const [form, setForm] = useState({
        userName: '',
        email: '',
        avatarUrl: '',
        currentPassword: '',
        newPassword: '',
    })
    const [topUpAmount, setTopUpAmount] = useState('');
    const [balanceMessage, setBalanceMessage] = useState(null);
    const [message, setMessage] = useState(null);
    const [avatarPreview, setAvatarPreview] = useState('');
    const [favorites, setFavorites] = useState([]);
    const [orders, setOrders] = useState([]);
    const [loadingOrders, setLoadingOrders] = useState(false);

    useEffect(() => {
        if (user) {
            const savedNames = JSON.parse(localStorage.getItem('userNames') || '{}');
            const savedName = savedNames[user.email];

            setForm({
                userName: savedName || user.name || user.userName || user.email?.split('@')[0] || '',
                email: user.email || '',
                avatarUrl: user.avatarUrl || '',
                currentPassword: '',
                newPassword: '',
            });
            setAvatarPreview(user.avatarUrl || '');

            setFavorites(getFavorites(user));
            loadUserOrders();
        }
    }, [user]);

    const loadUserOrders = async () => {
        setLoadingOrders(true);
        try {
            const res = await orderService.getAll();
            const data = res.data?.data || res.data || [];
            setOrders(data.sort((a, b) => b.id - a.id).slice(0, 5));
        } catch (err) {
            console.error("Orders load error", err);
        } finally {
            setLoadingOrders(false);
        }
    };

    const handleBalanceSubmit = async (e) => {
        e.preventDefault();
        setBalanceMessage(null);

        const numAmount = Number(topUpAmount);
        if (!topUpAmount || numAmount <= 0) {
            setBalanceMessage({ type: 'error', text: 'Enter valid amount' });
            return;
        }

        try {
            const res = await authService.updateBalance(numAmount);
            const serverUserData = res.data?.data;
            const newBalance = serverUserData?.balance ?? serverUserData?.Balance;

            if (newBalance !== undefined) {
                const updatedUser = { ...user, balance: newBalance };
                setUser(updatedUser);
                localStorage.setItem('user', JSON.stringify(updatedUser));
                setBalanceMessage({ type: 'success', text: `Added! New: $${newBalance.toFixed(2)}` });
                setTopUpAmount('');
            }
        } catch (err) {
            setBalanceMessage({ type: 'error', text: 'Failed to update' });
        }
    };

    const handleAvatarChange = async (e) => {
        const file = e.target.files[0];
        if (!file) return;
        const reader = new FileReader();
        reader.onloadend = () => setAvatarPreview(reader.result);
        reader.readAsDataURL(file);

        const formData = new FormData();
        formData.append('avatar', file);

        try {
            const token = localStorage.getItem('accessToken');
            const res = await fetch('/api/user/upload-avatar', {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
                body: formData,
            });
            const data = await res.json();
            if (data.avatarUrl) {
                setForm(prev => ({ ...prev, avatarUrl: data.avatarUrl }));
                setMessage({ type: 'success', text: 'Photo uploaded' });
            }
        } catch (err) {
            setMessage({ type: 'error', text: 'Failed to upload photo' });
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage(null);
        try {
            const res = await authService.updateProfile({ ...form });
            const mergedUser = { ...user, ...res.data.data };
            localStorage.setItem('user', JSON.stringify(mergedUser));
            if (setUser) setUser(mergedUser);
            setMessage({ type: 'success', text: 'Profile updated' });
        } catch (err) {
            setMessage({ type: 'error', text: 'Failed to update' });
        }
    };

    return (
        <div className="profile-layout">
            <div className="profile-main">
                <div className="card shadow">
                    <h2 className="subtitle center">Profile Info</h2>
                    {message && <Message type={message.type}>{message.text}</Message>}

                    <div className="avatar-section">
                        <img
                            src={avatarPreview || "https://www.nicepng.com/png/detail/73-730285_it-benefits-per-users-icon-user-png-green.png"}
                            alt="Avatar" className="avatar-preview-img"/>
                        <label className="btn btn-secondary avatar-upload-btn">
                            {avatarPreview ? 'Change Photo' : 'Add Photo'}
                            <input type="file" accept="image/*" onChange={handleAvatarChange} hidden/>
                        </label>
                    </div>

                    <form onSubmit={handleSubmit} className="form">
                        <Input placeholder="Username" value={form.userName}
                               onChange={(e) => setForm({...form, userName: e.target.value})}/>
                        <Input type="email" value={form.email} readOnly style={{background: '#f5f5f5'}}/>
                        <hr className="my"/>
                        <h4 className="subtitle">Security</h4>
                        <Input type="password" placeholder="Current Password" value={form.currentPassword}
                               onChange={(e) => setForm({...form, currentPassword: e.target.value})}/>
                        <Input type="password" placeholder="New Password" value={form.newPassword}
                               onChange={(e) => setForm({...form, newPassword: e.target.value})}/>
                        <Button type="submit">Update Profile</Button>
                    </form>
                </div>
            </div>
            <div className="profile-sidebar">
                <div className="card shadow mb">
                    <h3 className="subtitle">Favorites</h3>
                    {favorites.length === 0 ? <p className="muted">No favorites</p> : (
                        <div className="mini-products">
                            {favorites.map(product => {
                                const productId = product.id || product.productId;
                                return (
                                    <div key={productId}
                                         className="mini-product flex justify-between align-center mb-s">
                                        <div className="flex align-center gap-s">
                                            <img src={product.imageUrl} alt=""
                                                 style={{
                                                     width: '35px',
                                                     height: '35px',
                                                     borderRadius: '4px',
                                                     objectFit: 'cover'
                                                 }}/>
                                            <div>
                                                <p className="mini-title"
                                                   style={{margin: 0, fontSize: '13px'}}>{product.nameOfProduct}</p>
                                                <span className="mini-price"
                                                      style={{fontSize: '11px', color: '#666'}}>${product.price}</span>
                                            </div>
                                        </div>
                                        <button className="mini-remove"
                                                style={{background: 'none', border: 'none', cursor: 'pointer'}}
                                                onClick={() => {
                                                    const updated = favorites.filter(item => (item.id || item.productId) !== productId);
                                                    localStorage.setItem(`favorites_${user.email}`, JSON.stringify(updated));
                                                    setFavorites(updated);
                                                }}>❤️
                                        </button>
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </div>
                <div className="card shadow">
                    <h3 className="subtitle">Recent Orders</h3>
                    {loadingOrders ? <p>Loading...</p> : orders.length === 0 ? <p className="muted">No orders</p> : (
                        <div className="mini-orders-list">
                            {orders.map(order => (
                                <div key={order.id} className="mini-order-item"
                                     style={{borderBottom: '1px solid #eee', padding: '8px 0'}}>
                                    <div className="flex justify-between align-center">
                                        <div>
                                            <div style={{fontWeight: '600', fontSize: '13px'}}>Order #{order.id}</div>
                                            <small
                                                style={{color: '#888'}}>{new Date(order.createdAt).toLocaleDateString()}</small>
                                        </div>
                                        <div style={{
                                            fontWeight: 'bold',
                                            color: '#27ae60'
                                        }}>${order.totalSum?.toFixed(2)}</div>
                                    </div>
                                </div>
                            ))}
                            <Button variant="secondary" className="full-width mt-s" onClick={() => navigate('/orders')}>Full
                                History</Button>
                        </div>
                    )}
                </div>
            </div>
            <div className="profile-sidebar">
                <div className="card shadow">
                    <h3 className="subtitle">Wallet</h3>
                    <div className="balance-display mb-s" style={{
                        background: 'linear-gradient(135deg, #27ae60 0%, #2ecc71 100%)',
                        padding: '20px',
                        borderRadius: '12px',
                        color: 'white',
                        textAlign: 'center'
                    }}>
                        <div style={{fontSize: '12px', opacity: 0.8}}>Current Balance</div>
                        <div style={{fontSize: '28px', fontWeight: 'bold'}}>${(user?.balance || 0).toFixed(2)}</div>
                    </div>

                    <form onSubmit={handleBalanceSubmit} className="form">
                        <Input
                            placeholder="Amount"
                            type="number"
                            value={topUpAmount}
                            onChange={(e) => setTopUpAmount(e.target.value)}
                        />
                        <Button type="submit" variant="success" className="full-width">Add Funds</Button>
                    </form>
                </div>
            </div>
        </div>
    );
};
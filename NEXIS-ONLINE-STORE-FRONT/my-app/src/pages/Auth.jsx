import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Input } from '../components/UI/Input';
import { Button } from '../components/UI/Button';
import { Message } from '../components/UI/Message';

export const Auth = () => {
    const [isLogin, setIsLogin] = useState(true);
    const [form, setForm] = useState({
        name: '',
        email: '',
        password: '',
        confirmedPassword: ''
    });
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [message, setMessage] = useState(null);
    const { login, register } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage(null);

        try {
            if (isLogin) {
                const result = await login(form.email, form.password);
                if (result.success) {
                    navigate('/');
                }
            } else {
                const result = await register(form);
                if (result.success) {
                    navigate('/');
                }
            }
        } catch (err) {
            setMessage({
                type: 'error',
                text: err.response?.data?.message || 'Authentication failed'
            });
        }
    };

    const togglePassword = () => setShowPassword(!showPassword);
    const toggleConfirmPassword = () => setShowConfirmPassword(!showConfirmPassword);

    return (
        <div className="card" style={{maxWidth: '400px', margin: '40px auto'}}>
            <h2 className="subtitle center">{isLogin ? 'Login' : 'Register'}</h2>

            {message && <Message type={message.type}>{message.text}</Message>}

            <form onSubmit={handleSubmit} className="form">
                {!isLogin && (
                    <Input
                        placeholder="Name"
                        value={form.name}
                        onChange={(e) => setForm({...form, name: e.target.value})}
                        required
                    />
                )}

                <Input
                    type="email"
                    placeholder="Email"
                    value={form.email}
                    onChange={(e) => setForm({...form, email: e.target.value})}
                    required
                />

                <div className="password-field">
                    <Input
                        type={showPassword ? 'text' : 'password'}
                        placeholder="Password"
                        value={form.password}
                        onChange={(e) => setForm({...form, password: e.target.value})}
                        required
                    />
                    <button
                        type="button"
                        className="toggle-password"
                        onClick={togglePassword}
                    >
                        {showPassword ? '⌣' : '👁'}
                    </button>
                </div>

                {!isLogin && (
                    <div className="password-field">
                        <Input
                            type={showConfirmPassword ? 'text' : 'password'}
                            placeholder="Confirm Password"
                            value={form.confirmedPassword}
                            onChange={(e) => setForm({...form, confirmedPassword: e.target.value})}
                            required
                        />
                        <button
                            type="button"
                            className="toggle-password"
                            onClick={toggleConfirmPassword}
                        >
                            {showConfirmPassword ? '⌣' : '👁'}
                        </button>
                    </div>
                )}

                <Button type="submit" variant="primary">
                    {isLogin ? 'Login' : 'Register'}
                </Button>
            </form>

            <p className="center mt">
                {isLogin ? "Don't have an account? " : "Already have an account? "}
                <button
                    onClick={() => setIsLogin(!isLogin)}
                    className="link"
                >
                    {isLogin ? 'Register' : 'Login'}
                </button>
            </p>

            <footer className="auth-footer">
                <div className="footer-content">
                    <p>&copy; 2026 Nexis Store. All rights reserved.</p>
                    <div className="footer-links">
                        <a href="https://t.me/TuralAlibey" target="_blank" rel="noopener noreferrer">
                            Contact Us
                        </a>
                    </div>
                </div>
            </footer>
        </div>
    );
};
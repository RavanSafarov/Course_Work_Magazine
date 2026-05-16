import { Button } from '../UI/Button';

export const CartItem = ({ item, onUpdate, onRemove }) => {
    return (
        <div className="cart-item">
            <img
                src={item.productImageUrl}
                alt={item.productName}
                className="cart-item-img"
            />
            <div className="cart-item-info">
                <div className="cart-item-title">{item.productName}</div>
                <div className="cart-item-price">${item.productPrice}</div>
            </div>
            <div className="qty">
                <button onClick={() => onUpdate(item.id, item.productId, item.quantity - 1)}>-</button>
                <span>{item.quantity}</span>
                <button onClick={() => onUpdate(item.id, item.productId, item.quantity + 1)}>+</button>
            </div>
            <Button variant="danger" onClick={() => onRemove(item.id)}>
                Remove
            </Button>
        </div>
    );
};
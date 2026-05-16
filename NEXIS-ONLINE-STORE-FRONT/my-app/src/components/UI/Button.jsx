export const Button = ({children, onClick, type = 'button', variant = 'primary', className = ''}) => {
    const variants = {
        primary: 'btn btn-primary',
        danger: 'btn btn-danger',
        success: 'btn btn-success',
        secondary: 'btn btn-secondary',
    };

    return (
        <button
            type={type}
            onClick={onClick}
            className={`${variants[variant]} ${className}`}
        >
            {children}
        </button>
    );
};
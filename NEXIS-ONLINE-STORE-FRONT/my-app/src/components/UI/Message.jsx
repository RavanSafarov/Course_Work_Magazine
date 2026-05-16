export const Message = ({ type, children }) => {
    if (!children)
        return null;

    const types = {
        error: 'msg msg-error',
        success: 'msg msg-success',
        info: 'msg msg-info',
    };

    return (
        <div className={types[type] || 'msg msg-info'}>
            {children}
        </div>
    );
};
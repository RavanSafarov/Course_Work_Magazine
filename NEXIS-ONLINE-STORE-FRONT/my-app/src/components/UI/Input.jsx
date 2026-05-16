export const Input = ({ type = 'text', placeholder, value, onChange, required = false, label }) => (
    <div className="field">
        {label && <label>{label}</label>}
        <input
            type={type}
            placeholder={placeholder}
            value={value}
            onChange={onChange || (() => {})}
            required={required}
            className="input"
        />
    </div>
);
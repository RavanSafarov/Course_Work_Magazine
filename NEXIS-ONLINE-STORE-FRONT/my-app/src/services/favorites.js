const getKey = (user) => `favorites_${user?.email}`;

export const getFavorites = (user) => {
    const key = getKey(user);
    return JSON.parse(localStorage.getItem(key)) || [];
};

export const addToFavorites = (product, user) => {
    const key = getKey(user);
    const favorites = getFavorites(user);

    const productId = product.id || product.productId;

    if (!favorites.find(item => (item.id || item.productId) === productId)) {
        const updated = [...favorites, product];
        localStorage.setItem(key, JSON.stringify(updated));
    }
};

export const removeFromFavorites = (id, user) => {
    const key = getKey(user);

    const updated = getFavorites(user).filter(
        item => (item.id || item.productId) !== id
    );

    localStorage.setItem(key, JSON.stringify(updated));
};

export const toggleFavorite = (product, user) => {
    const productId = product.id || product.productId;

    if (isFavorite(productId, user)) {
        removeFromFavorites(productId, user);
    } else {
        addToFavorites(product, user);
    }
};

export const isFavorite = (id, user) => {
    return getFavorites(user).some(
        item => (item.id || item.productId) === id
    );
};
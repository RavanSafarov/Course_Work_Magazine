import { useState, useMemo } from 'react';
import { ProductCard } from './ProductCard';
import { Input } from '../UI/Input';
import { Pagination } from '../UI/Pagination';

export const ProductList = ({ products = [], categories = [], addToast }) => {
    const [search, setSearch] = useState('');
    const [selectedCategory, setSelectedCategory] = useState('');
    const [minPrice, setMinPrice] = useState('');
    const [maxPrice, setMaxPrice] = useState('');

    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 8;

    const filteredProducts = useMemo(() => {
        let result = (products || []).filter((product) => {
            const matchesSearch = product.nameOfProduct
                ?.toLowerCase()
                .includes(search.toLowerCase());

            const matchesCategory = selectedCategory
                ? product.categoryId === Number(selectedCategory)
                : true;

            const matchesMinPrice = minPrice
                ? product.price >= Number(minPrice)
                : true;

            const matchesMaxPrice = maxPrice
                ? product.price <= Number(maxPrice)
                : true;

            return matchesSearch && matchesCategory && matchesMinPrice && matchesMaxPrice;
        });

        return result;
    }, [products, search, selectedCategory, minPrice, maxPrice]);

    const totalPages = Math.ceil(filteredProducts.length / itemsPerPage);

    const paginatedProducts = filteredProducts.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    const clearFilters = () => {
        setSearch('');
        setSelectedCategory('');
        setMinPrice('');
        setMaxPrice('');
        setCurrentPage(1);
    };

    const handlePageChange = (page) => {
        setCurrentPage(page);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const hasFilters = search || selectedCategory || minPrice || maxPrice;

    return (
        <div>
            <div className="card mb">
                <div className="filters">
                    <Input
                        placeholder="Search products..."
                        value={search}
                        onChange={(e) => { setSearch(e.target.value); setCurrentPage(1); }}
                    />

                    <select
                        className="input"
                        value={selectedCategory}
                        onChange={(e) => { setSelectedCategory(e.target.value); setCurrentPage(1); }}
                    >
                        <option value="">All Categories</option>
                        {(categories || []).map((cat) => (
                            <option key={cat.id} value={cat.id}>
                                {cat.nameOfCategory}
                            </option>
                        ))}
                    </select>

                    <div className="price-range">
                        <Input
                            type="number"
                            placeholder="Min price"
                            value={minPrice}
                            onChange={(e) => { setMinPrice(e.target.value); setCurrentPage(1); }}
                        />
                        <span>-</span>
                        <Input
                            type="number"
                            placeholder="Max price"
                            value={maxPrice}
                            onChange={(e) => { setMaxPrice(e.target.value); setCurrentPage(1); }}
                        />
                    </div>

                    {hasFilters && (
                        <button className="btn btn-secondary" onClick={clearFilters}>
                            Clear
                        </button>
                    )}
                </div>

                <p className="results-count">
                    Showing {paginatedProducts.length} of {filteredProducts.length} products
                    (Page {currentPage} of {totalPages || 1})
                </p>
            </div>

            {paginatedProducts.length === 0 ? (
                <p>No products found</p>
            ) : (
                <>
                    <div className="products">
                        {paginatedProducts.map((product) => (
                            <ProductCard
                                key={product.id || product.productId}
                                product={product}
                                addToast={addToast}
                            />
                        ))}
                    </div>

                    {totalPages > 1 && (
                        <Pagination
                            currentPage={currentPage}
                            totalPages={totalPages}
                            onPageChange={handlePageChange}
                        />
                    )}
                </>
            )}
        </div>
    );
};
import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { productService, categoryService } from '../api/services';
import ProductCard from '../components/ProductCard';
import './Catalog.css';

export default function Catalog() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [products, setProducts] = useState([]);
  const [allProducts, setAllProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filters, setFilters] = useState({
    category: searchParams.get('category') || '',
    minPrice: '',
    maxPrice: '',
    brand: ''
  });

  useEffect(() => {
    Promise.all([
      categoryService.getAll(),
      productService.getAll()
    ]).then(([categoriesRes, productsRes]) => {
      setCategories(categoriesRes.data);
      setAllProducts(productsRes.data);
      setProducts(productsRes.data);
    }).catch(() => {
      setError('Ошибка загрузки данных');
    }).finally(() => {
      setLoading(false);
    });
  }, []);

  useEffect(() => {
    // Клиентская фильтрация
    let filtered = [...allProducts];

    if (filters.category) {
      filtered = filtered.filter(p => p.categoryId === parseInt(filters.category));
    }

    if (filters.minPrice) {
      filtered = filtered.filter(p => p.price >= parseFloat(filters.minPrice));
    }

    if (filters.maxPrice) {
      filtered = filtered.filter(p => p.price <= parseFloat(filters.maxPrice));
    }

    if (filters.brand) {
      const brandLower = filters.brand.toLowerCase();
      filtered = filtered.filter(p => p.brand && p.brand.toLowerCase().includes(brandLower));
    }

    setProducts(filtered);
  }, [filters, allProducts]);

  const handleFilterChange = (key, value) => {
    const newFilters = { ...filters, [key]: value };
    setFilters(newFilters);
    const newParams = new URLSearchParams();
    Object.entries(newFilters).forEach(([k, v]) => {
      if (v) newParams.set(k, v);
    });
    setSearchParams(newParams);
  };

  return (
    <div className="catalog">
      <aside className="filters">
        <h3>Фильтры</h3>
        
        <div className="filter-group">
          <label>Категория</label>
          <select 
            value={filters.category} 
            onChange={(e) => handleFilterChange('category', e.target.value)}
          >
            <option value="">Все категории</option>
            {categories.map(cat => (
              <option key={cat.id} value={cat.id}>{cat.name}</option>
            ))}
          </select>
        </div>

        <div className="filter-group">
          <label>Цена от</label>
          <input 
            type="number" 
            value={filters.minPrice}
            onChange={(e) => handleFilterChange('minPrice', e.target.value)}
            placeholder="0"
          />
        </div>

        <div className="filter-group">
          <label>Цена до</label>
          <input 
            type="number" 
            value={filters.maxPrice}
            onChange={(e) => handleFilterChange('maxPrice', e.target.value)}
            placeholder="100000"
          />
        </div>

        <div className="filter-group">
          <label>Бренд</label>
          <input 
            type="text" 
            value={filters.brand}
            onChange={(e) => handleFilterChange('brand', e.target.value)}
            placeholder="Razer, Logitech..."
          />
        </div>

        <button 
          className="clear-filters"
          onClick={() => {
            setFilters({ category: '', minPrice: '', maxPrice: '', brand: '' });
            setSearchParams({});
          }}
        >
          Сбросить фильтры
        </button>
      </aside>

      <main className="catalog-content">
        <h1>Каталог товаров</h1>
        <p className="products-count">{products.length} товаров</p>
        
        {error ? (
          <div className="error-message">{error}</div>
        ) : loading ? (
          <div className="loading">Загрузка...</div>
        ) : products.length === 0 ? (
          <p className="no-products">Товары не найдены</p>
        ) : (
          <div className="products-grid">
            {products.map(product => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )}
      </main>
    </div>
  );
}

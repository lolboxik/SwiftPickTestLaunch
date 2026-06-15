import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { productService, categoryService } from '../api/services';
import ProductCard from '../components/ProductCard';
import './Home.css';

export default function Home() {
  const [featuredProducts, setFeaturedProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    async function loadData() {
      try {
        const [productsRes, categoriesRes] = await Promise.all([
          productService.getAll(),
          categoryService.getAll()
        ]);
        setFeaturedProducts(productsRes.data.slice(0, 8));
        setCategories(categoriesRes.data);
      } catch (error) {
        console.error('Ошибка загрузки:', error);
      } finally {
        setLoading(false);
      }
    }
    loadData();
  }, []);

  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Загрузка...</p>
      </div>
    );
  }

  return (
    <div className="home">
      {/* Hero Section */}
      <section className="hero">
        <div className="hero-content">
          <h1 className="hero-title">
            Игровые аксессуары <span className="highlight">премиум класса</span>
          </h1>
          <p className="hero-subtitle">
            Лучшие бренды, гарантия качества, быстрая доставка
          </p>
          <button className="hero-btn" onClick={() => navigate('/catalog')}>
            Перейти в каталог →
          </button>
        </div>
        <div className="hero-image">
          <div className="hero-visual">🎮</div>
        </div>
      </section>

      {/* Categories */}
      <section className="categories-section">
        <h2 className="section-title">Категории</h2>
        <div className="categories-grid">
          {categories.map(category => (
            <div
              key={category.id}
              className="category-card"
              onClick={() => navigate(`/catalog?category=${category.id}`)}
              style={{ cursor: 'pointer' }}
            >
              <div className="category-icon">📦</div>
              <h3>{category.name}</h3>
              <span className="category-count">{category.productCount} товаров</span>
            </div>
          ))}
        </div>
      </section>

      {/* Featured Products */}
      <section className="products-section">
        <div className="section-header">
          <h2 className="section-title">Популярные товары</h2>
          <button className="view-all" onClick={() => navigate('/catalog')}>
            Смотреть все →
          </button>
        </div>
        <div className="products-grid">
          {featuredProducts.map(product => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      </section>

      {/* Benefits */}
      <section className="benefits-section">
        <h2 className="section-title">Почему SwiftPick?</h2>
        <div className="benefits-grid">
          <div className="benefit-card">
            <div className="benefit-icon">🚀</div>
            <h3>Быстрая доставка</h3>
            <p>Отправляем заказы в день оформления</p>
          </div>
          <div className="benefit-card">
            <div className="benefit-icon">🛡️</div>
            <h3>Гарантия качества</h3>
            <p>Только оригинальная продукция</p>
          </div>
          <div className="benefit-card">
            <div className="benefit-icon">💬</div>
            <h3>Поддержка 24/7</h3>
            <p>Поможем с выбором и ответим на вопросы</p>
          </div>
          <div className="benefit-card">
            <div className="benefit-icon">💳</div>
            <h3>Удобная оплата</h3>
            <p>Различные способы оплаты онлайн</p>
          </div>
        </div>
      </section>
    </div>
  );
}

import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import './ProductCard.css';

export default function ProductCard({ product }) {
  const { addToCart } = useCart();
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [adding, setAdding] = useState(false);
  const [imageError, setImageError] = useState(false);

  const handleAddToCart = async (e) => {
    e.stopPropagation();
    
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    setAdding(true);
    try {
      await addToCart(product.id, 1);
    } catch (error) {
      console.error('Ошибка добавления в корзину:', error);
    } finally {
      setAdding(false);
    }
  };

  const handleClick = () => {
    navigate(`/product/${product.id}`);
  };

  const mainImage = !imageError && product.imagePath ? product.imagePath : '/placeholder.jpg';

  return (
    <div className="product-card" onClick={handleClick} style={{ cursor: 'pointer' }}>
      <div className="product-image">
        <img 
          src={mainImage} 
          alt={product.name} 
          onError={() => setImageError(true)}
        />
        {product.stock === 0 && <span className="out-of-stock">Нет в наличии</span>}
      </div>

      <div className="product-info">
        <div className="product-brand">{product.brand}</div>
        <h3 className="product-name">{product.name}</h3>
        
        {product.specifications && product.specifications.length > 0 && (
          <div className="product-specs">
            {product.specifications.slice(0, 3).map((spec, index) => (
              <div key={index} className="spec-item">
                <span className="spec-key">{spec.key}:</span>
                <span className="spec-value">{spec.value}</span>
              </div>
            ))}
          </div>
        )}

        <div className="product-footer">
          <div className="product-price">{product.price.toLocaleString()} ₽</div>
          <button 
            className={`add-to-cart-btn ${adding ? 'adding' : ''}`}
            onClick={handleAddToCart}
            disabled={product.stock === 0 || adding}
          >
            {adding ? 'Добавление...' : product.stock > 0 ? 'В корзину' : 'Нет в наличии'}
          </button>
        </div>
      </div>
    </div>
  );
}

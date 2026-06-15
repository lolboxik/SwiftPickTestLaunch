import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { productService } from '../api/services';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import './ProductDetail.css';

export default function ProductDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart, cart, removeFromCart, updateCartItem } = useCart();
  const { isAuthenticated } = useAuth();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [adding, setAdding] = useState(false);

  useEffect(() => {
    productService.getById(id).then(res => {
      setProduct(res.data);
    }).catch(() => {
      setError('Ошибка загрузки товара');
    }).finally(() => {
      setLoading(false);
    });
  }, [id]);

  // Находим товар в корзине
  const cartItem = cart?.items?.find(item => item.productId === product?.id);
  const cartQuantity = cartItem?.quantity || 0;
  const isInCart = cartQuantity > 0;

  const handleAddToCart = async () => {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    setAdding(true);
    try {
      await addToCart(product.id, quantity);
    } catch (error) {
      console.error('Ошибка добавления в корзину:', error);
    } finally {
      setAdding(false);
    }
  };

  const handleRemoveFromCart = async () => {
    try {
      await removeFromCart(product.id);
    } catch (error) {
      console.error('Ошибка удаления из корзины:', error);
    }
  };

  const handleIncreaseCart = async () => {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }
    setAdding(true);
    try {
      await updateCartItem(product.id, cartQuantity + 1);
    } catch (error) {
      console.error('Ошибка обновления корзины:', error);
    } finally {
      setAdding(false);
    }
  };

  const handleDecreaseCart = async () => {
    if (cartQuantity <= 1) {
      await handleRemoveFromCart();
    } else {
      setAdding(true);
      try {
        await updateCartItem(product.id, cartQuantity - 1);
      } catch (error) {
        console.error('Ошибка обновления корзины:', error);
      } finally {
        setAdding(false);
      }
    }
  };

  if (loading) return <div className="loading">Загрузка...</div>;
  if (error) return <div className="not-found">{error}</div>;
  if (!product) return <div className="not-found">Товар не найден</div>;

  return (
    <div className="product-detail-page">
      <button className="back-btn" onClick={() => navigate('/catalog')}>← Назад в каталог</button>
      
      <div className="product-detail">
        <div className="product-detail-image">
          <img src={product.imagePath || '/placeholder.jpg'} alt={product.name} onError={(e) => e.target.src = '/placeholder.jpg'} />
        </div>

        <div className="product-detail-info">
          <div className="product-detail-brand">{product.brand}</div>
          <h1 className="product-detail-name">{product.name}</h1>
          <p className="product-detail-description">{product.description}</p>
          
          <div className="product-detail-price">{product.price.toLocaleString()} ₽</div>
          
          <div className="product-detail-stock">
            {product.stock > 0 ? (
              <span className="in-stock">✓ В наличии ({product.stock} шт.)</span>
            ) : (
              <span className="out-of-stock">✕ Нет в наличии</span>
            )}
          </div>

          <div className="product-detail-actions">
            {isInCart ? (
              <div className="cart-quantity-selector">
                <button onClick={handleDecreaseCart} disabled={adding}>−</button>
                <span className="quantity-value">{cartQuantity} шт. в корзине</span>
                <button onClick={handleIncreaseCart} disabled={adding || product.stock === 0}>+</button>
              </div>
            ) : (
              <>
                <div className="quantity-selector">
                  <button onClick={() => setQuantity(Math.max(1, quantity - 1))}>−</button>
                  <span>{quantity}</span>
                  <button onClick={() => setQuantity(quantity + 1)}>+</button>
                </div>
                <button 
                  className="add-to-cart-btn" 
                  onClick={handleAddToCart}
                  disabled={product.stock === 0 || adding}
                >
                  {adding ? 'Добавление...' : 'В корзину'}
                </button>
              </>
            )}
          </div>
        </div>
      </div>

      {product.specifications && product.specifications.length > 0 && (
        <div className="product-specifications">
          <h2>Характеристики</h2>
          <table>
            <tbody>
              {product.specifications.map((spec, index) => (
                <tr key={index}>
                  <td className="spec-key">{spec.key}</td>
                  <td className="spec-value">{spec.value}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { orderService } from '../api/services';
import './Cart.css';

export default function Cart() {
  const { cart, loading, removeFromCart, updateCartItem, clearCart, refreshCart, totalAmount } = useCart();
  const navigate = useNavigate();
  const [processing, setProcessing] = useState(false);

  const handleCheckout = async () => {
    if (!cart?.items?.length || processing) return;
    setProcessing(true);

    try {
      const order = await orderService.create({
        shippingAddress: 'г. Москва, ул. Примерная, д. 1',
        phoneNumber: '+7 (999) 000-00-00',
        email: cart.userId,
        paymentMethod: 'card'
      });
      alert(`Заказ ${order.data.orderNumber} оформлен!`);
      await refreshCart();
      navigate('/orders');
    } catch (error) {
      alert(error.response?.data?.message || 'Ошибка оформления заказа');
    } finally {
      setProcessing(false);
    }
  };

  const handleDecrease = async (item) => {
    if (processing) return;
    setProcessing(true);
    try {
      if (item.quantity <= 1) {
        await removeFromCart(item.productId);
      } else {
        await updateCartItem(item.productId, item.quantity - 1);
      }
    } catch {
      alert('Ошибка обновления корзины');
    } finally {
      setProcessing(false);
    }
  };

  const handleIncrease = async (item) => {
    if (processing) return;
    setProcessing(true);
    try {
      await updateCartItem(item.productId, item.quantity + 1);
    } catch {
      alert('Ошибка обновления корзины');
    } finally {
      setProcessing(false);
    }
  };

  const handleClear = async () => {
    if (processing) return;
    setProcessing(true);
    try {
      await clearCart();
    } catch {
      alert('Ошибка очистки корзины');
    } finally {
      setProcessing(false);
    }
  };

  const handleRemove = async (productId) => {
    if (processing) return;
    setProcessing(true);
    try {
      await removeFromCart(productId);
    } catch {
      alert('Ошибка удаления товара');
    } finally {
      setProcessing(false);
    }
  };

  if (loading) return <div className="loading-cart">Загрузка...</div>;

  if (!cart?.items?.length) {
    return (
      <div className="cart-empty">
        <h1>Корзина пуста</h1>
        <p>Добавьте товары из каталога</p>
        <Link to="/catalog" className="btn-primary">Перейти в каталог</Link>
      </div>
    );
  }

  return (
    <div className="cart-page">
      <h1>Корзина</h1>

      <div className="cart-content">
        <div className="cart-items">
          {cart.items.map(item => (
            <div key={item.id} className="cart-item">
              <img src={item.productImagePath || '/placeholder.jpg'} alt={item.productName} />
              <div className="cart-item-info">
                <h3>{item.productName}</h3>
                <p className="price">{item.price.toLocaleString()} ₽</p>
              </div>
              <div className="cart-item-quantity">
                <button onClick={() => handleDecrease(item)} disabled={processing}>-</button>
                <span>{item.quantity}</span>
                <button onClick={() => handleIncrease(item)} disabled={processing}>+</button>
              </div>
              <p className="cart-item-total">{(item.price * item.quantity).toLocaleString()} ₽</p>
              <button className="remove-btn" onClick={() => handleRemove(item.productId)} disabled={processing}>✕</button>
            </div>
          ))}
        </div>

        <div className="cart-summary">
          <h3>Итого</h3>
          <div className="summary-row">
            <span>Товары ({cart.items.reduce((s, i) => s + i.quantity, 0)} шт.)</span>
            <span>{totalAmount.toLocaleString()} ₽</span>
          </div>
          <div className="summary-row">
            <span>Доставка</span>
            <span>Бесплатно</span>
          </div>
          <div className="summary-total">
            <span>К оплате</span>
            <span>{totalAmount.toLocaleString()} ₽</span>
          </div>
          <button className="checkout-btn" onClick={handleCheckout} disabled={processing}>
            {processing ? 'Оформление...' : 'Оформить заказ'}
          </button>
          <button className="clear-cart-btn" onClick={handleClear} disabled={processing}>Очистить корзину</button>
        </div>
      </div>
    </div>
  );
}

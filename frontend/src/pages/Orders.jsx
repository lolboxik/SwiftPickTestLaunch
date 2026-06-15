import { useState, useEffect } from 'react';
import { orderService } from '../api/services';
import './Orders.css';

export default function Orders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    orderService.getAll().then(res => {
      setOrders(res.data);
    }).catch(() => {
      setError('Ошибка загрузки заказов');
    }).finally(() => {
      setLoading(false);
    });
  }, []);

  const getStatusLabel = (status) => {
    const labels = {
      new: 'Новый',
      paid: 'Оплачен',
      shipped: 'Отправлен',
      delivered: 'Доставлен',
      cancelled: 'Отменён'
    };
    return labels[status?.toLowerCase()] || status;
  };

  const getStatusClass = (status) => {
    const classes = {
      new: 'status-new',
      paid: 'status-paid',
      shipped: 'status-shipped',
      delivered: 'status-delivered',
      cancelled: 'status-cancelled'
    };
    return classes[status?.toLowerCase()] || '';
  };

  if (loading) return <div className="loading">Загрузка...</div>;
  if (error) return <div className="orders-page"><h1>Мои заказы</h1><div className="error-message">{error}</div></div>;

  return (
    <div className="orders-page">
      <h1>Мои заказы</h1>

      {orders.length === 0 ? (
        <p className="no-orders">У вас пока нет заказов</p>
      ) : (
        <div className="orders-list">
          {orders.map(order => (
            <div key={order.id} className="order-card">
              <div className="order-header">
                <span className="order-number">Заказ {order.orderNumber}</span>
                <span className={`order-status ${getStatusClass(order.status)}`}>
                  {getStatusLabel(order.status)}
                </span>
              </div>

              <div className="order-date">
                {new Date(order.createdAt).toLocaleDateString('ru-RU', {
                  year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'
                })}
              </div>

              <div className="order-items">
                {order.items.map(item => (
                  <div key={item.id} className="order-item">
                    <img src={item.productImagePath || '/placeholder.jpg'} alt={item.productName} />
                    <div className="order-item-info">
                      <span>{item.productName}</span>
                      <span className="quantity">× {item.quantity}</span>
                    </div>
                    <span className="order-item-price">{(item.price * item.quantity).toLocaleString()} ₽</span>
                  </div>
                ))}
              </div>

              <div className="order-footer">
                <span className="order-total">Итого: <strong>{order.totalAmount.toLocaleString()} ₽</strong></span>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

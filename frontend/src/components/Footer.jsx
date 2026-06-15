import './Footer.css';

export default function Footer() {
  return (
    <footer className="footer">
      <div className="footer-container">
        <div className="footer-section">
          <h3>⚡ SwiftPick</h3>
          <p>Ваш надёжный магазин игровых аксессуаров</p>
        </div>

        <div className="footer-section">
          <h4>Категории</h4>
          <ul>
            <li>Клавиатуры</li>
            <li>Мыши</li>
            <li>Наушники</li>
            <li>Геймпады</li>
          </ul>
        </div>

        <div className="footer-section">
          <h4>Помощь</h4>
          <ul>
            <li>Доставка и оплата</li>
            <li>Возврат</li>
            <li>Гарантия</li>
            <li>Контакты</li>
          </ul>
        </div>

        <div className="footer-section">
          <h4>Контакты</h4>
          <p>📧 support@swiftpick.ru</p>
          <p>📞 8 (800) 000-00-00</p>
          <p>📍 Москва, Россия</p>
        </div>
      </div>

      <div className="footer-bottom">
        <p>&copy; 2026 SwiftPick. Все права защищены.</p>
      </div>
    </footer>
  );
}

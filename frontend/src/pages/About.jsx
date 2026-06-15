import './About.css';

export default function About() {
  return (
    <div className="about-page">
      <h1>О компании SwiftPick</h1>
      
      <div className="about-content">
        <section>
          <h2>🎮 Наша миссия</h2>
          <p>
            SwiftPick — это ваш надёжный партнёр в мире игровых аксессуаров. 
            Мы стремимся предоставить геймерам лучший выбор периферии от ведущих мировых брендов 
            по доступным ценам с быстрым и удобным сервисом.
          </p>
        </section>

        <section>
          <h2>🏆 Почему мы</h2>
          <ul>
            <li>Только оригинальная продукция от официальных поставщиков</li>
            <li>Быстрая доставка по всей России</li>
            <li>Гарантия на все товары</li>
            <li>Профессиональная консультация при выборе</li>
          </ul>
        </section>

        <section>
          <h2>📞 Контакты</h2>
          <p><strong>Email:</strong> support@swiftpick.ru</p>
          <p><strong>Телефон:</strong> 8 (800) 000-00-00</p>
          <p><strong>Адрес:</strong> г. Москва, ул. Примерная, д. 1</p>
          <p><strong>Режим работы:</strong> Пн-Вс: 9:00 - 21:00</p>
        </section>
      </div>
    </div>
  );
}

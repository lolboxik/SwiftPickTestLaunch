import { useState, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';
import './Header.css';

export default function Header({ theme, onToggleTheme }) {
  const { user, logout, isAdmin } = useAuth();
  const { itemCount } = useCart();
  const navigate = useNavigate();

  const [dropdownOpen, setDropdownOpen] = useState(false);
  const hideTimer = useRef(null);

  useEffect(() => {
    return () => clearTimeout(hideTimer.current);
  }, []);

  const handleLogout = () => {
    setDropdownOpen(false);
    logout();
    navigate('/');
  };

  const toggleDropdown = () => {
    clearTimeout(hideTimer.current);
    setDropdownOpen((isOpen) => !isOpen);
  };

  const navigateFromDropdown = (path) => {
    setDropdownOpen(false);
    navigate(path);
  };

  return (
    <header className="header">
      <div className="header-container">
        <div className="logo" onClick={() => navigate('/')} style={{ cursor: 'pointer' }}>
          <span className="logo-icon">⚡</span>
          <span className="logo-text">SwiftPick</span>
        </div>

        <nav className="nav">
          <button className="nav-link" onClick={() => navigate('/catalog')}>Каталог</button>
          <button className="nav-link" onClick={() => navigate('/about')}>О нас</button>
          {isAdmin && (
            <button className="nav-link admin-link" onClick={() => navigate('/admin')}>Админ-панель</button>
          )}
        </nav>

        <div className="header-actions">
          <button
            className={`theme-toggle ${theme === 'light' ? 'light-active' : 'dark-active'}`}
            onClick={onToggleTheme}
            aria-label={theme === 'light' ? 'Включить тёмную тему' : 'Включить светлую тему'}
            title={theme === 'light' ? 'Включить тёмную тему' : 'Включить светлую тему'}
            type="button"
          >
            {theme === 'light' ? '●' : '☀'}
          </button>

          <button className="cart-btn" onClick={() => navigate('/cart')}>
            Корзина
            {itemCount > 0 && <span className="cart-count">{itemCount}</span>}
          </button>

          {user ? (
            <div
              className={`user-menu${dropdownOpen ? ' open' : ''}`}
              onMouseEnter={() => { clearTimeout(hideTimer.current); setDropdownOpen(true); }}
              onMouseLeave={() => { hideTimer.current = setTimeout(() => setDropdownOpen(false), 500); }}
            >
              <button
                className="user-name"
                onClick={toggleDropdown}
                aria-expanded={dropdownOpen}
                aria-haspopup="menu"
                type="button"
              >
                {user.firstName || user.email}
              </button>
              <div className="user-dropdown" role="menu">
                <button className="dropdown-item" onClick={() => navigateFromDropdown('/profile')} role="menuitem">Профиль</button>
                <button className="dropdown-item" onClick={() => navigateFromDropdown('/orders')} role="menuitem">Заказы</button>
                <button onClick={handleLogout} className="dropdown-item logout" role="menuitem">
                  Выйти
                </button>
              </div>
            </div>
          ) : (
            <div className="auth-buttons">
              <button className="btn btn-outline" onClick={() => navigate('/login')}>Вход</button>
              <button className="btn btn-primary" onClick={() => navigate('/register')}>Регистрация</button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}

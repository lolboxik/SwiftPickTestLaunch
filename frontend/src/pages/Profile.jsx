import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { authService } from '../api/services';
import './Profile.css';

export default function Profile() {
  const { user } = useAuth();
  const [passwordForm, setPasswordForm] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const handlePasswordChange = (e) => {
    setPasswordForm((currentForm) => ({
      ...currentForm,
      [e.target.name]: e.target.value
    }));
  };

  const handlePasswordSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setMessage('');

    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      setError('Новый пароль и подтверждение не совпадают');
      return;
    }

    if (passwordForm.newPassword.length < 6) {
      setError('Новый пароль должен быть не менее 6 символов');
      return;
    }

    setSaving(true);

    try {
      const result = await authService.changePassword(
        passwordForm.currentPassword,
        passwordForm.newPassword
      );

      if (result.success) {
        setMessage('Пароль успешно изменён');
        setPasswordForm({
          currentPassword: '',
          newPassword: '',
          confirmPassword: ''
        });
      } else {
        setError(result.error || 'Не удалось изменить пароль');
      }
    } catch (err) {
      setError(err.response?.data?.error || 'Не удалось изменить пароль');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="profile-page">
      <section className="profile-card">
        <h1>Профиль</h1>
        <p className="profile-subtitle">Данные вашей учетной записи</p>

        <div className="profile-fields">
          <div className="profile-field">
            <span className="profile-label">Почта / логин</span>
            <span className="profile-value">{user?.email || 'Не указано'}</span>
          </div>

          <form className="change-password-form" onSubmit={handlePasswordSubmit}>
            <h2>Сменить пароль</h2>

            {message && <div className="profile-message success">{message}</div>}
            {error && <div className="profile-message error">{error}</div>}

            <div className="profile-field">
              <label className="profile-label" htmlFor="currentPassword">Текущий пароль</label>
              <input
                id="currentPassword"
                name="currentPassword"
                type="password"
                value={passwordForm.currentPassword}
                onChange={handlePasswordChange}
                required
                autoComplete="current-password"
                placeholder="Введите текущий пароль"
              />
            </div>

            <div className="profile-field">
              <label className="profile-label" htmlFor="newPassword">Новый пароль</label>
              <input
                id="newPassword"
                name="newPassword"
                type="password"
                value={passwordForm.newPassword}
                onChange={handlePasswordChange}
                required
                autoComplete="new-password"
                placeholder="Минимум 6 символов"
              />
            </div>

            <div className="profile-field">
              <label className="profile-label" htmlFor="confirmPassword">Повторите новый пароль</label>
              <input
                id="confirmPassword"
                name="confirmPassword"
                type="password"
                value={passwordForm.confirmPassword}
                onChange={handlePasswordChange}
                required
                autoComplete="new-password"
                placeholder="Повторите новый пароль"
              />
            </div>

            <button className="change-password-btn" type="submit" disabled={saving}>
              {saving ? 'Сохранение...' : 'Сменить пароль'}
            </button>
          </form>
        </div>
      </section>
    </div>
  );
}

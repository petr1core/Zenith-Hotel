import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_CONFIG } from '../config';
import './Auth.css';

const Auth = ({ setToken }) => {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({
    Login: '',
    Password: '',
    Role: 'User',
    UserFirstname: '',
    UserLastname: '',
    UserPhoneNumber: '',
    UserEmail: '',
    confirmPassword: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    if (!isLogin && formData.Password !== formData.confirmPassword) {
      setError('Пароли не совпадают');
      setLoading(false);
      return;
    }

    try {
      const endpoint = isLogin
        ? `${API_CONFIG.API_BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.LOGIN}`
        : `${API_CONFIG.API_BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.REGISTER}`;
      const payload = isLogin
        ? { Login: formData.Login, Password: formData.Password }
        : {
          UserEmail: formData.UserEmail,
          UserPassword: formData.Password,
          Role: formData.Role,
          UserFirstname: formData.UserFirstname,
          UserLastname: formData.UserLastname,
          UserPhoneNumber: formData.UserPhoneNumber
        };

      const response = await axios.post(endpoint, payload);
      console.log(response.data);

      if (response.data.token) {
        localStorage.setItem('token', response.data.token);
        setToken(response.data.token);

        // Декодируем JWT токен для получения роли
        const tokenParts = response.data.token.split('.');
        const payload = JSON.parse(atob(tokenParts[1]));
        const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        console.log('Logged with role:', role);
        localStorage.setItem('role', role);

        navigate('/');
      }
    } catch (error) {
      console.error('Auth error:', error);
      setError(error.response?.data?.message || 'Произошла ошибка при авторизации');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-form">
        <h2>{isLogin ? 'Вход' : 'Регистрация'}</h2>

        {error && <div className="auth-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          {!isLogin && (
            <div className="form-group">
              <label htmlFor="UserFirstname">Имя:</label>
              <input
                type="text"
                id="UserFirstname"
                name="UserFirstname"
                value={formData.UserFirstname}
                onChange={handleChange}
                required
              />

              <label htmlFor="UserLastname">Фамилия:</label>
              <input
                type="text"
                id="UserLastname"
                name="UserLastname"
                value={formData.UserLastname}
                onChange={handleChange}
                required
              />
            </div>
          )}

          {!isLogin && (
            <div className="form-group">
              <label htmlFor="UserEmail">Email:</label>
              <input
                type="email"
                id="UserEmail"
                name="UserEmail"
                value={formData.UserEmail}
                onChange={handleChange}
                required
              />
            </div>
          )}

          {!isLogin && (
            <div className="form-group">
              <label htmlFor="UserPhoneNumber">Номер телефона:</label>
              <input
                type="tel"
                id="UserPhoneNumber"
                name="UserPhoneNumber"
                value={formData.UserPhoneNumber}
                onChange={handleChange}
                required
              />
            </div>
          )}

          {isLogin && (
            <div className="form-group">
              <label htmlFor="Login">Email или номер телефона:</label>
              <input
                type="text"
                id="Login"
                name="Login"
                value={formData.Login}
                onChange={handleChange}
                required
              />
            </div>
          )}

          <div className="form-group">
            <label htmlFor="Password">Пароль:</label>
            <input
              type="password"
              id="Password"
              name="Password"
              value={formData.Password}
              onChange={handleChange}
              required
              minLength="6"
            />
          </div>

          {!isLogin && (
            <div className="form-group">
              <label htmlFor="confirmPassword">Подтвердите пароль:</label>
              <input
                type="password"
                id="confirmPassword"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
                minLength="6"
              />
            </div>
          )}

          {!isLogin && (
            <div className="form-group">
              <label htmlFor="Role">Роль:</label>
              <select
                id="Role"
                name="Role"
                value={formData.Role}
                onChange={handleChange}
              >
                <option value="User">Пользователь</option>
                <option value="Admin">Администратор</option>
                <option value="Manager">Менеджер</option>
              </select>
            </div>
          )}

          <button type="submit" disabled={loading}>
            {loading ? 'Загрузка...' : isLogin ? 'Войти' : 'Зарегистрироваться'}
          </button>
        </form>

        <div className="auth-switch">
          {isLogin ? (
            <p>Нет аккаунта? <button onClick={() => setIsLogin(false)}>Зарегистрироваться</button></p>
          ) : (
            <p>Уже есть аккаунт? <button onClick={() => setIsLogin(true)}>Войти</button></p>
          )}
        </div>
      </div>
    </div>
  );
};

export default Auth;
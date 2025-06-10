import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import axios from 'axios';
import { API_CONFIG } from './config';
import HomePage from './pages/HomePage';
import Auth from './pages/Auth';
import Navbar from './pages/Navbar';
import BookingPage from './pages/BookingPage';
import RoomPage from './pages/RoomPage';
import AdminPage from './pages/AdminPage';

// Настраиваем базовый URL для axios
axios.defaults.baseURL = API_CONFIG.BASE_URL;
axios.defaults.headers.common['Content-Type'] = 'application/json';

function App() {
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [role, setRole] = useState(localStorage.getItem('role'));

  const clearAuth = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    setToken(null);
    setRole(null);
  };

  const checkTokenValidity = async () => {
    if (!token) return;

    try {
      const response = await axios.get(`${API_CONFIG.API_BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.VALIDATE}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (!response.data.isValid) {
        console.log('Token is invalid, clearing auth...');
        clearAuth();
      }
    } catch (error) {
      console.error('Token validation error:', error);
      clearAuth();
    }
  };

  // Обновляем заголовок Authorization при изменении токена
  useEffect(() => {
    if (token) {
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      // Проверяем токен при его установке
      checkTokenValidity();
    } else {
      delete axios.defaults.headers.common['Authorization'];
    }
  }, [token]);

  useEffect(() => {
    // Настраиваем перехватчик для axios
    const interceptor = axios.interceptors.response.use(
      response => response,
      error => {
        if (error.response?.status === 401) {
          console.log('Received 401, clearing auth...');
          clearAuth();
        }
        return Promise.reject(error);
      }
    );

    // Проверяем токен каждые 30 секунд
    const intervalId = setInterval(checkTokenValidity, 30000);

    return () => {
      axios.interceptors.response.eject(interceptor);
      clearInterval(intervalId);
    };
  }, []);

  const handleLogout = () => {
    clearAuth();
  };

  return (
    <Router>
      <Navbar token={token} role={role} onLogout={handleLogout} />
      <Routes>
        <Route path="/" element={<HomePage token={token} />} />
        <Route 
          path="/auth" 
          element={token ? <Navigate to="/" /> : <Auth setToken={setToken} />} 
        />
        <Route 
          path="/booking/:roomId" 
          element={token ? <BookingPage /> : <Navigate to="/auth" />} 
        />
        <Route path="/room/:id" element={<RoomPage token={token} />} />
        <Route 
          path="/admin" 
          element={role === 'Admin' ? <AdminPage /> : <Navigate to="/" />} 
        />
      </Routes>
    </Router>
  );
}

export default App;
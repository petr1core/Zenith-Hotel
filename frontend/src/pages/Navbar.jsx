import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';

const Navbar = ({ token, role, onLogout }) => {
  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/" className="navbar-brand">Zenith Hotel</Link>
        
        <div className="navbar-links">
          {token ? (
            <>
              {role === 'Admin' && (
                <Link to="/admin" className="navbar-link">Админ панель</Link>
              )}
              <button onClick={onLogout} className="navbar-link">Выйти</button>
            </>
          ) : (
            <Link to="/auth" className="navbar-link">Войти</Link>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
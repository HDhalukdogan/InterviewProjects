import React from 'react';
import { NavLink, Link, useNavigate } from 'react-router';
import { useAuth } from '../context/AuthContext';
import './Navbar.css';

function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login', {
      state: { message: 'Başarıyla çıkış yaptınız.' },
    });
  };

  const getInitial = (email) => {
    return email ? email.charAt(0).toUpperCase() : 'U';
  };

  return (
    <header className="navbar">
      <Link to="/" className="navbar-brand">
        <span>Identity</span>Hub
      </Link>

      <nav>
        <ul className="navbar-links">
          <li>
            <NavLink to="/" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
              Ana Sayfa
            </NavLink>
          </li>
          <li>
            <NavLink to="/appointment" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
              Randevular
            </NavLink>
          </li>
          <li>
            <NavLink to="/cart" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
              Sepet
            </NavLink>
          </li>
          <li>
            <NavLink to="/weather" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
              Hava Durumu
            </NavLink>
          </li>
        </ul>
      </nav>

      <div className="navbar-auth">
        {isAuthenticated ? (
          <>
            <div className="user-badge" title={user?.email || ''}>
              <span className="user-avatar-circle">{getInitial(user?.email)}</span>
              <span>{user?.email ? user.email.split('@')[0] : 'Kullanıcı'}</span>
            </div>
            <button type="button" className="nav-btn nav-btn-danger" onClick={handleLogout}>
              Çıkış Yap
            </button>
          </>
        ) : (
          <>
            <Link to="/login" className="nav-btn nav-btn-outline">
              Giriş Yap
            </Link>
            <Link to="/register" className="nav-btn nav-btn-primary">
              Kayıt Ol
            </Link>
          </>
        )}
      </div>
    </header>
  );
}

export default Navbar;

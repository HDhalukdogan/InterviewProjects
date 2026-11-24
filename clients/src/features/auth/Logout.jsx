import React, { useEffect } from 'react';
import { useNavigate } from 'react-router';
import { useAuth } from '../../context/AuthContext';
import './Auth.css';

function Logout() {
  const { logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const performLogout = async () => {
      await logout();
      navigate('/login', {
        state: { message: 'Başarıyla çıkış yaptınız.' },
        replace: true,
      });
    };
    performLogout();
  }, [logout, navigate]);

  return (
    <div className="auth-container">
      <div className="auth-card" style={{ textAlign: 'center' }}>
        <div className="auth-icon" style={{ margin: '0 auto 16px' }}>
          <span className="spinner" style={{ borderColor: 'var(--accent)', borderTopColor: 'transparent' }}></span>
        </div>
        <h2>Çıkış Yapılıyor</h2>
        <p>Hesabınız güvenli bir şekilde kapatılıyor...</p>
      </div>
    </div>
  );
}

export default Logout;

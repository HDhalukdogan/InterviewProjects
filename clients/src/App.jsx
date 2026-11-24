import React from 'react';
import { Link } from 'react-router';
import { useAuth } from './context/AuthContext';
import './App.css';

function App() {
  const { user, isAuthenticated, logout } = useAuth();

  return (
    <div style={{ padding: '40px 20px', maxWidth: '960px', margin: '0 auto', textAlign: 'left' }}>
      <header style={{ marginBottom: '40px', textAlign: 'center' }}>
        <h1 style={{ fontSize: '38px', marginBottom: '12px' }}>
          InterviewProjects <span style={{ color: 'var(--accent)' }}>Portal</span>
        </h1>
        <p style={{ fontSize: '18px', color: 'var(--text)' }}>
          .NET 10 & IdentityApp mimarisi ile entegre mikroservis ve modül yönetim paneli.
        </p>
      </header>

      {/* Auth Status Card */}
      <div
        style={{
          background: 'var(--code-bg)',
          border: '1px solid var(--border)',
          borderRadius: '12px',
          padding: '24px',
          marginBottom: '36px',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          flexWrap: 'wrap',
          gap: '16px',
        }}
      >
        <div>
          <h3 style={{ margin: '0 0 6px 0', fontSize: '18px', color: 'var(--text-h)' }}>
            Oturum Durumu
          </h3>
          {isAuthenticated ? (
            <p style={{ color: '#16a34a', fontWeight: '500', margin: 0 }}>
              Giriş yapıldı: <strong>{user?.email}</strong>
            </p>
          ) : (
            <p style={{ color: 'var(--text)', margin: 0 }}>
              Şu anda misafir olarak geziniyorsunuz. Giriş yaparak işlemlerinize devam edebilirsiniz.
            </p>
          )}
        </div>

        <div style={{ display: 'flex', gap: '10px' }}>
          {isAuthenticated ? (
            <button
              onClick={logout}
              style={{
                padding: '8px 16px',
                borderRadius: '6px',
                background: 'rgba(239, 68, 68, 0.1)',
                border: '1px solid #ef4444',
                color: '#ef4444',
                fontWeight: '600',
                cursor: 'pointer',
              }}
            >
              Çıkış Yap
            </button>
          ) : (
            <>
              <Link
                to="/login"
                style={{
                  padding: '8px 18px',
                  borderRadius: '6px',
                  background: 'transparent',
                  border: '1px solid var(--border)',
                  color: 'var(--text-h)',
                  fontWeight: '600',
                  textDecoration: 'none',
                }}
              >
                Giriş Yap
              </Link>
              <Link
                to="/register"
                style={{
                  padding: '8px 18px',
                  borderRadius: '6px',
                  background: 'var(--accent)',
                  border: '1px solid transparent',
                  color: '#fff',
                  fontWeight: '600',
                  textDecoration: 'none',
                }}
              >
                Kayıt Ol
              </Link>
            </>
          )}
        </div>
      </div>

      {/* Quick Navigation Cards */}
      <h2 style={{ fontSize: '22px', marginBottom: '20px', color: 'var(--text-h)' }}>Modüller</h2>
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(260px, 1fr))',
          gap: '20px',
        }}
      >
        {/* IdentityApp Card */}
        <div
          style={{
            border: '1px solid var(--border)',
            borderRadius: '12px',
            padding: '24px',
            background: 'var(--bg)',
            boxShadow: 'var(--shadow)',
          }}
        >
          <div style={{ fontSize: '28px', marginBottom: '10px' }}>🔐</div>
          <h3 style={{ margin: '0 0 8px 0', color: 'var(--text-h)' }}>IdentityApp (Auth)</h3>
          <p style={{ fontSize: '14px', color: 'var(--text)', marginBottom: '16px' }}>
            ASP.NET Core Identity Cookie kimlik doğrulaması (register, login?useCookies=true, logout, manage/info) ve <code>agentIdentity</code> axios istemcisi.
          </p>
          <div style={{ display: 'flex', gap: '8px' }}>
            <Link
              to="/login"
              style={{
                fontSize: '13px',
                color: 'var(--accent)',
                fontWeight: '600',
                textDecoration: 'none',
              }}
            >
              Giriş →
            </Link>
            <span style={{ color: 'var(--border)' }}>|</span>
            <Link
              to="/register"
              style={{
                fontSize: '13px',
                color: 'var(--accent)',
                fontWeight: '600',
                textDecoration: 'none',
              }}
            >
              Kayıt →
            </Link>
          </div>
        </div>

        {/* Appointments Card */}
        <div
          style={{
            border: '1px solid var(--border)',
            borderRadius: '12px',
            padding: '24px',
            background: 'var(--bg)',
            boxShadow: 'var(--shadow)',
          }}
        >
          <div style={{ fontSize: '28px', marginBottom: '10px' }}>📅</div>
          <h3 style={{ margin: '0 0 8px 0', color: 'var(--text-h)' }}>Randevu Modülü</h3>
          <p style={{ fontSize: '14px', color: 'var(--text)', marginBottom: '16px' }}>
            Doktor randevu listeleme ve yeni randevu oluşturma servisi (<code>agentAppointment</code>).
          </p>
          <Link
            to="/appointment"
            style={{
              fontSize: '13px',
              color: 'var(--accent)',
              fontWeight: '600',
              textDecoration: 'none',
            }}
          >
            Randevuları Gör →
          </Link>
        </div>

        {/* Cart Card */}
        <div
          style={{
            border: '1px solid var(--border)',
            borderRadius: '12px',
            padding: '24px',
            background: 'var(--bg)',
            boxShadow: 'var(--shadow)',
          }}
        >
          <div style={{ fontSize: '28px', marginBottom: '10px' }}>🛒</div>
          <h3 style={{ margin: '0 0 8px 0', color: 'var(--text-h)' }}>Sepet & Ürünler</h3>
          <p style={{ fontSize: '14px', color: 'var(--text)', marginBottom: '16px' }}>
            Ürün listeleme ve sepete ürün ekleme servisi (<code>agentCart</code>).
          </p>
          <Link
            to="/cart"
            style={{
              fontSize: '13px',
              color: 'var(--accent)',
              fontWeight: '600',
              textDecoration: 'none',
            }}
          >
            Sepete Git →
          </Link>
        </div>

        {/* WeatherForecast Card */}
        <div
          style={{
            border: '1px solid var(--border)',
            borderRadius: '12px',
            padding: '24px',
            background: 'var(--bg)',
            boxShadow: 'var(--shadow)',
          }}
        >
          <div style={{ fontSize: '28px', marginBottom: '10px' }}>🌤️</div>
          <h3 style={{ margin: '0 0 8px 0', color: 'var(--text-h)' }}>Hava Durumu</h3>
          <p style={{ fontSize: '14px', color: 'var(--text)', marginBottom: '16px' }}>
            IdentityApp üzerinden hava durumu verilerini çekme (<code>agentIdentity</code>).
          </p>
          <Link
            to="/weather"
            style={{
              fontSize: '13px',
              color: 'var(--accent)',
              fontWeight: '600',
              textDecoration: 'none',
            }}
          >
            Hava Durumunu Gör →
          </Link>
        </div>
      </div>
    </div>
  );
}

export default App;

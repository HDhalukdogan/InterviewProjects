import React, { useEffect, useState } from 'react';
import { identityApi } from '../../lib/agentIdentity';
import { useAuth } from '../../context/AuthContext';
import './Weather.css';

function WeatherForecast() {
  const { user, isAuthenticated } = useAuth();
  const [forecasts, setForecasts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showJson, setShowJson] = useState(false);

  const fetchWeather = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await identityApi.getWeatherForecast();
      setForecasts(data);
    } catch (err) {
      console.error('Error fetching WeatherForecast:', err);
      setError(
        err.response?.data?.message ||
        err.message ||
        'Hava durumu verisi alınamadı. IdentityApp (http://localhost:5118) çalışıyor mu kontrol ediniz.'
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchWeather();
  }, []);

  // Weather icon & badge category helper
  const getWeatherInfo = (summary, tempC) => {
    const s = summary?.toLowerCase() || '';
    if (s.includes('freeze') || s.includes('bracing') || tempC <= 0) {
      return { icon: '❄️', category: 'badge-cold', label: summary || 'Freezing' };
    }
    if (s.includes('chill') || s.includes('cool') || tempC <= 15) {
      return { icon: '🌤️', category: 'badge-mild', label: summary || 'Cool' };
    }
    if (s.includes('mild') || s.includes('balmy') || tempC <= 25) {
      return { icon: '⛅', category: 'badge-warm', label: summary || 'Mild' };
    }
    if (s.includes('hot') || s.includes('swelter') || s.includes('scorch') || tempC > 25) {
      return { icon: '🔥', category: 'badge-hot', label: summary || 'Hot' };
    }
    return { icon: '☀️', category: 'badge-mild', label: summary || 'Clear' };
  };

  // Format DateOnly (YYYY-MM-DD) nicely
  const formatDate = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', {
      weekday: 'short',
      month: 'short',
      day: 'numeric',
    });
  };

  return (
    <div className="weather-container">
      <div className="weather-header">
        <div>
          <h1>Hava Durumu Tahmini</h1>
          <p>
            <strong>IdentityApp</strong> (<code>WeatherForecastController</code>) üzerinden <code>agentIdentity</code> ile çekilen 5 günlük tahmin verileri.
          </p>
        </div>

        <button
          type="button"
          className="refresh-button"
          onClick={fetchWeather}
          disabled={loading}
        >
          {loading ? (
            <>
              <span className="spinner" style={{ width: 14, height: 14 }}></span>
              Yenileniyor...
            </>
          ) : (
            <>
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.67" />
              </svg>
              Yenile
            </>
          )}
        </button>
      </div>

      {/* Auth & Identity Cookie Context Banner */}
      <div className={`weather-auth-banner ${isAuthenticated ? 'authenticated' : 'unauthenticated'}`}>
        <span style={{ fontSize: 22 }}>{isAuthenticated ? '🍪' : '👤'}</span>
        <div>
          {isAuthenticated ? (
            <>
              <strong>Oturum Açık ({user?.email}):</strong> İstek Identity Cookie (<code>withCredentials: true</code>) ile gönderildi. Kullanıcı bilgileri backend'de Redis cache ve RabbitMQ kuyruğuna yazıldı.
            </>
          ) : (
            <>
              <strong>Misafir İstek:</strong> Şu an aktif bir Identity çerezi yok veya anonim olarak istek yapıldı.
            </>
          )}
        </div>
      </div>

      {/* Error Message */}
      {error && (
        <div className="weather-error">
          <h3 style={{ margin: '0 0 8px 0' }}>Bağlantı Hatası</h3>
          <p style={{ margin: '0 0 16px 0' }}>{error}</p>
          <button type="button" className="refresh-button" onClick={fetchWeather}>
            Tekrar Dene
          </button>
        </div>
      )}

      {/* Loading state */}
      {loading && forecasts.length === 0 && (
        <div className="weather-loading">
          <span className="spinner" style={{ width: 32, height: 32, borderColor: 'var(--accent)', borderTopColor: 'transparent', margin: '0 auto 12px' }}></span>
          <p>Hava durumu verileri yükleniyor...</p>
        </div>
      )}

      {/* Weather Forecasts Grid */}
      {!loading && !error && forecasts.length > 0 && (
        <div className="weather-grid">
          {forecasts.map((item, index) => {
            const info = getWeatherInfo(item.summary, item.temperatureC);
            return (
              <div key={index} className="weather-card">
                <div className="weather-card-date">{formatDate(item.date)}</div>
                <div className="weather-card-icon">{info.icon}</div>
                <div className="weather-card-temp">
                  <div className="temp-celsius">{item.temperatureC}°C</div>
                  <div className="temp-fahrenheit">{item.temperatureF}°F</div>
                </div>
                <span className={`weather-badge ${info.category}`}>
                  {info.label}
                </span>
              </div>
            );
          })}
        </div>
      )}

      {/* Raw JSON Debug Viewer */}
      {forecasts.length > 0 && (
        <div className="debug-section">
          <button
            type="button"
            className="debug-toggle"
            onClick={() => setShowJson(!showJson)}
          >
            {showJson ? '▼ Ham JSON Yanıtını Gizle' : '▶ Ham JSON Yanıtını Göster (Debug)'}
          </button>
          {showJson && (
            <pre className="debug-json">
              {JSON.stringify(forecasts, null, 2)}
            </pre>
          )}
        </div>
      )}
    </div>
  );
}

export default WeatherForecast;

import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { useAuth } from '../../context/AuthContext';
import './Auth.css';

function Register() {
  const { register } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState([]);
  const [success, setSuccess] = useState(false);

  // Password validation checks according to Identity default rules
  const hasMinLength = password.length >= 6;
  const hasUpper = /[A-Z]/.test(password);
  const hasLower = /[a-z]/.test(password);
  const hasDigit = /[0-9]/.test(password);
  const hasSpecial = /[^a-zA-Z0-9]/.test(password);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrors([]);

    const clientErrors = [];

    if (!email) {
      clientErrors.push('E-posta adresi gereklidir.');
    }

    if (!hasMinLength) {
      clientErrors.push('Şifre en az 6 karakter olmalıdır.');
    }

    if (password !== confirmPassword) {
      clientErrors.push('Şifreler birbiriyle eşleşmiyor.');
    }

    if (clientErrors.length > 0) {
      setErrors(clientErrors);
      return;
    }

    setLoading(true);
    try {
      await register(email, password);
      setSuccess(true);
      setTimeout(() => {
        navigate('/login', {
          state: { message: 'Kayıt başarılı! Şimdi hesabınıza giriş yapabilirsiniz.' },
        });
      }, 1500);
    } catch (err) {
      console.error('Register error:', err);
      const serverErrors = [];

      if (err.response?.data?.errors) {
        // Identity returns: { errors: { PasswordRequiresNonAlphanumeric: [...], ... } }
        const rawErrors = err.response.data.errors;
        Object.keys(rawErrors).forEach((key) => {
          const msgs = Array.isArray(rawErrors[key]) ? rawErrors[key] : [rawErrors[key]];
          msgs.forEach((m) => {
            // Translate common ASP.NET Identity error strings to Turkish
            if (m.includes('is already taken')) {
              serverErrors.push(`Bu e-posta adresi (${email}) zaten kullanımda.`);
            } else if (m.includes('must have at least one non alphanumeric')) {
              serverErrors.push('Şifre en az bir özel karakter (örn: !?*$) içermelidir.');
            } else if (m.includes('must have at least one digit')) {
              serverErrors.push('Şifre en az bir rakam (0-9) içermelidir.');
            } else if (m.includes('must have at least one uppercase')) {
              serverErrors.push('Şifre en az bir büyük harf (A-Z) içermelidir.');
            } else if (m.includes('must have at least one lowercase')) {
              serverErrors.push('Şifre en az bir küçük harf (a-z) içermelidir.');
            } else {
              serverErrors.push(m);
            }
          });
        });
      } else if (err.response?.data?.detail) {
        serverErrors.push(err.response.data.detail);
      } else {
        serverErrors.push('Kayıt oluşturulurken bir hata oluştu. Lütfen tekrar deneyiniz.');
      }

      setErrors(serverErrors.length > 0 ? serverErrors : ['Kayıt işlemi başarısız.']);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div className="auth-header">
          <div className="auth-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
              <circle cx="8.5" cy="7" r="4" />
              <line x1="20" y1="8" x2="20" y2="14" />
              <line x1="23" y1="11" x2="17" y2="11" />
            </svg>
          </div>
          <h2>Kayıt Ol</h2>
          <p>Yeni bir IdentityApp hesabı oluşturun</p>
        </div>

        {success && (
          <div className="auth-alert auth-alert-success">
            Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...
          </div>
        )}

        {errors.length > 0 && (
          <div className="auth-alert auth-alert-error">
            <strong>Lütfen aşağıdaki hataları düzeltin:</strong>
            <ul>
              {errors.map((err, idx) => (
                <li key={idx}>{err}</li>
              ))}
            </ul>
          </div>
        )}

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="reg-email">E-posta Adresi</label>
            <div className="input-wrapper">
              <input
                id="reg-email"
                type="email"
                className="auth-input"
                placeholder="ornek@domain.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                autoComplete="email"
                required
                disabled={loading || success}
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="reg-password">Şifre</label>
            <div className="input-wrapper">
              <input
                id="reg-password"
                type={showPassword ? 'text' : 'password'}
                className="auth-input"
                placeholder="En az 6 karakter"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                autoComplete="new-password"
                required
                disabled={loading || success}
              />
              <button
                type="button"
                className="toggle-password"
                onClick={() => setShowPassword(!showPassword)}
                tabIndex={-1}
              >
                {showPassword ? 'Gizle' : 'Göster'}
              </button>
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="reg-confirm-password">Şifre Tekrarı</label>
            <div className="input-wrapper">
              <input
                id="reg-confirm-password"
                type={showPassword ? 'text' : 'password'}
                className="auth-input"
                placeholder="Şifrenizi tekrar giriniz"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                autoComplete="new-password"
                required
                disabled={loading || success}
              />
            </div>
          </div>

          {/* Password complexity helper */}
          <div className="password-criteria">
            <span style={{ fontWeight: 600, color: 'var(--text-h)' }}>Şifre Gereksinimleri:</span>
            <div className={`criteria-item ${hasMinLength ? 'valid' : ''}`}>
              <span className="check">{hasMinLength ? '✓' : '•'}</span> En az 6 karakter
            </div>
            <div className={`criteria-item ${hasUpper ? 'valid' : ''}`}>
              <span className="check">{hasUpper ? '✓' : '•'}</span> En az bir büyük harf (A-Z)
            </div>
            <div className={`criteria-item ${hasLower ? 'valid' : ''}`}>
              <span className="check">{hasLower ? '✓' : '•'}</span> En az bir küçük harf (a-z)
            </div>
            <div className={`criteria-item ${hasDigit ? 'valid' : ''}`}>
              <span className="check">{hasDigit ? '✓' : '•'}</span> En az bir rakam (0-9)
            </div>
            <div className={`criteria-item ${hasSpecial ? 'valid' : ''}`}>
              <span className="check">{hasSpecial ? '✓' : '•'}</span> En az bir özel karakter (örn: !@#$%^&*)
            </div>
          </div>

          <button type="submit" className="auth-button" disabled={loading || success}>
            {loading ? (
              <>
                <span className="spinner"></span> Kayıt yapılıyor...
              </>
            ) : (
              'Kayıt Ol'
            )}
          </button>
        </form>

        <div className="auth-footer">
          Zaten hesabınız var mı?
          <Link to="/login">Giriş Yap</Link>
        </div>
      </div>
    </div>
  );
}

export default Register;

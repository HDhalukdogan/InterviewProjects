import React, { useEffect, useState, useCallback } from 'react';
import { Link } from 'react-router';
import agentCart from '../../lib/agentCart';
import { useAuth } from '../../context/AuthContext';

function Cart() {
  const { user, isAuthenticated } = useAuth();
  const [products, setProducts] = useState([]);
  const [myCart, setMyCart] = useState(null);
  const [jwtAuthInfo, setJwtAuthInfo] = useState(null);
  const [loading, setLoading] = useState(false);
  const [notification, setNotification] = useState(null);

  const checkJwtAuth = useCallback(async () => {
    try {
      const response = await agentCart.get('userCarts/user-info');
      setJwtAuthInfo(response.data);
    } catch (error) {
      console.warn('Cart.API JWT check error:', error?.message || error);
    }
  }, []);

  const getMyCart = useCallback(async () => {
    try {
      const response = await agentCart.get('userCarts/my-cart');
      setMyCart(response.data);
    } catch {
      setMyCart(null);
    }
  }, []);

  const getProducts = async () => {
    try {
      const response = await agentCart.get('products');
      setProducts(response.data);
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  useEffect(() => {
    getProducts();
    checkJwtAuth();
    if (isAuthenticated) {
      getMyCart();
    }
  }, [checkJwtAuth, getMyCart, isAuthenticated]);

  const addToCart = async (product) => {
    setLoading(true);
    setNotification(null);
    try {
      // Cart.API: Kullanıcı kimliği artık SADECE gelen JWT token'dan (çerez) çözülür.
      const response = await agentCart.put('userCarts/addItemToUserCart', {
        productId: product.id,
        quantity: 1,
        unitPrice: product.price,
      });

      if (response.status === 200) {
        setNotification({
          type: 'success',
          message: `"${product.name}" sepete başarıyla eklendi!`,
        });
        getMyCart();
      }
    } catch (error) {
      console.error('Error adding product to cart:', error);
      if (error.response?.status === 401) {
        setNotification({
          type: 'error',
          message: 'Sepete ürün eklemek için lütfen giriş yapınız.',
        });
      } else {
        setNotification({
          type: 'error',
          message: error.response?.data?.message || 'Ürün sepete eklenirken bir hata oluştu.',
        });
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '960px', margin: '0 auto', padding: '32px 20px', textAlign: 'left' }}>
      <header style={{ marginBottom: '24px' }}>
        <h1 style={{ fontSize: '32px', margin: '0 0 8px 0' }}>Sepet ve Ürünler</h1>
        <p style={{ color: 'var(--text)', margin: 0 }}>
          <strong>Cart.API</strong> (Port 5193) servisinde kullanıcı parametresi tamamen kaldırılmıştır; kimlik <strong>yalnızca gelen JWT token</strong> üzerinden okunur.
        </p>
      </header>

      {/* JWT Connection Status Banner */}
      <div
        style={{
          background: 'var(--code-bg)',
          border: '1px solid var(--border)',
          borderLeft: jwtAuthInfo?.isAuthenticated ? '4px solid #16a34a' : '4px solid #ef4444',
          borderRadius: '8px',
          padding: '14px 18px',
          marginBottom: '24px',
          fontSize: '14px',
          display: 'flex',
          alignItems: 'center',
          gap: '12px',
        }}
      >
        <span style={{ fontSize: '20px' }}>{jwtAuthInfo?.isAuthenticated ? '🛡️' : '🔒'}</span>
        <div>
          {jwtAuthInfo?.isAuthenticated ? (
            <div>
              <strong style={{ color: '#16a34a' }}>JWT Kimliği Doğrulandı:</strong> İstek gelen kullanıcının kimliğiyle eşleştirildi.
              <div style={{ fontSize: '13px', color: 'var(--text)', marginTop: '4px' }}>
                Kullanıcı: <strong>{jwtAuthInfo.email}</strong> | UserId (JWT Sub): <code>{jwtAuthInfo.userId}</code>
              </div>
            </div>
          ) : (
            <div>
              <strong style={{ color: '#ef4444' }}>Oturum Açılmamış:</strong> Cart.API'ye sepete ekleme yapabilmek için lütfen{' '}
              <Link to="/login" style={{ color: 'var(--accent)', fontWeight: 'bold' }}>
                Giriş Yapın
              </Link>
              .
            </div>
          )}
        </div>
      </div>

      {notification && (
        <div
          style={{
            padding: '12px 16px',
            borderRadius: '8px',
            marginBottom: '20px',
            background: notification.type === 'success' ? '#f0fdf4' : '#fef2f2',
            border: notification.type === 'success' ? '1px solid #bbf7d0' : '1px solid #fecaca',
            color: notification.type === 'success' ? '#15803d' : '#b91c1c',
            fontSize: '14px',
          }}
        >
          {notification.message}
        </div>
      )}

      {/* User's Cart Items (If logged in and cart exists) */}
      {jwtAuthInfo?.isAuthenticated && myCart && myCart.items && myCart.items.length > 0 && (
        <div
          style={{
            background: 'var(--bg)',
            border: '1px solid var(--border)',
            borderRadius: '12px',
            padding: '20px',
            marginBottom: '32px',
            boxShadow: 'var(--shadow)',
          }}
        >
          <h2 style={{ fontSize: '20px', margin: '0 0 14px 0', color: 'var(--text-h)' }}>
            🛒 Sepetim ({myCart.items.length} Kalem Ürün)
          </h2>
          <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: '8px' }}>
            {myCart.items.map((item, idx) => (
              <li
                key={idx}
                style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  padding: '10px 12px',
                  background: 'var(--code-bg)',
                  borderRadius: '6px',
                  fontSize: '14px',
                }}
              >
                <span>Ürün ID: <code>{item.productId}</code> (Adet: <strong>{item.quantity}</strong>)</span>
                <span style={{ fontWeight: 600 }}>Birim Fiyat: ₺{item.unitPrice?.toFixed(2)}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      <h2 style={{ fontSize: '22px', marginBottom: '16px' }}>Ürün Listesi</h2>
      {products.length === 0 ? (
        <p style={{ color: 'var(--text)' }}>Yükleniyor veya kayıtlı ürün bulunmamaktadır...</p>
      ) : (
        <div
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))',
            gap: '20px',
          }}
        >
          {products.map((item) => (
            <div
              key={item.id}
              style={{
                background: 'var(--bg)',
                border: '1px solid var(--border)',
                borderRadius: '12px',
                padding: '20px',
                boxShadow: 'var(--shadow)',
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between',
              }}
            >
              <div>
                <h3 style={{ margin: '0 0 8px 0', fontSize: '18px', color: 'var(--text-h)' }}>
                  {item.name}
                </h3>
                <p style={{ fontSize: '20px', fontWeight: 700, color: 'var(--accent)', margin: '0 0 8px 0' }}>
                  ₺{item.price?.toFixed(2)}
                </p>
                <p style={{ fontSize: '13px', color: 'var(--text)', margin: 0 }}>
                  Stok: {item.quantity}
                </p>
              </div>

              <button
                type="button"
                disabled={loading}
                onClick={() => addToCart(item)}
                style={{
                  marginTop: '16px',
                  padding: '10px',
                  borderRadius: '6px',
                  border: 'none',
                  background: 'var(--accent)',
                  color: '#fff',
                  fontWeight: 600,
                  cursor: 'pointer',
                  transition: 'opacity 0.2s',
                }}
              >
                Sepete Ekle
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Cart;

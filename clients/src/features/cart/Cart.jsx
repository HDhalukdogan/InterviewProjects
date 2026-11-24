import React, { useEffect, useState } from 'react';
import agentCart from '../../lib/agentCart';
import { useAuth } from '../../context/AuthContext';

function Cart() {
  const { user, isAuthenticated } = useAuth();
  const [products, setProducts] = useState([]);
  const [jwtAuthInfo, setJwtAuthInfo] = useState(null);
  const [loading, setLoading] = useState(false);
  const [notification, setNotification] = useState(null);

  useEffect(() => {
    getProducts();
    checkJwtAuth();
  }, []);

  const checkJwtAuth = async () => {
    try {
      const response = await agentCart.get('userCarts/user-info');
      setJwtAuthInfo(response.data);
    } catch (error) {
      console.warn('Cart.API JWT check error:', error?.message || error);
    }
  };

  const getProducts = async () => {
    try {
      const response = await agentCart.get('products');
      setProducts(response.data);
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  const addToCart = async (product) => {
    setLoading(true);
    setNotification(null);
    try {
      // Cart.API artık kullanıcı kimliğini çerezdeki JWT token'dan otomatik çözer
      const response = await agentCart.put('userCarts/addItemToUserCart', {
        productId: product.id,
        quantity: 1,
        unitPrice: product.price,
      });

      if (response.status === 200) {
        setNotification({
          type: 'success',
          message: `"${product.name}" sepete başarıyla eklendi! (Kullanıcı: ${jwtAuthInfo?.email || user?.email || 'Varsayılan'})`,
        });
      }
    } catch (error) {
      console.error('Error adding product to cart:', error);
      setNotification({
        type: 'error',
        message: 'Ürün sepete eklenirken bir hata oluştu.',
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '900px', margin: '0 auto', padding: '32px 20px', textAlign: 'left' }}>
      <header style={{ marginBottom: '24px' }}>
        <h1 style={{ fontSize: '32px', margin: '0 0 8px 0' }}>Sepet ve Ürünler</h1>
        <p style={{ color: 'var(--text)', margin: 0 }}>
          <strong>Cart.API</strong> (Port 5193) servisi, IdentityApp çerezindeki <strong>JWT Token</strong> ile doğrulanmaktadır.
        </p>
      </header>

      {/* JWT Connection Status Banner */}
      <div
        style={{
          background: 'var(--code-bg)',
          border: '1px solid var(--border)',
          borderLeft: jwtAuthInfo?.isAuthenticated ? '4px solid #16a34a' : '4px solid #eab308',
          borderRadius: '8px',
          padding: '14px 18px',
          marginBottom: '24px',
          fontSize: '14px',
          display: 'flex',
          alignItems: 'center',
          gap: '12px',
        }}
      >
        <span style={{ fontSize: '20px' }}>{jwtAuthInfo?.isAuthenticated ? '🛡️' : '👤'}</span>
        <div>
          {jwtAuthInfo?.isAuthenticated ? (
            <div>
              <strong style={{ color: '#16a34a' }}>JWT Kimliği Doğrulandı:</strong> Cart.API, çerezdeki JWT Bearer token'ı başarıyla çözdü.
              <div style={{ fontSize: '13px', color: 'var(--text)', marginTop: '4px' }}>
                Kullanıcı: <strong>{jwtAuthInfo.email}</strong> | UserId (Claim): <code>{jwtAuthInfo.userId}</code>
              </div>
            </div>
          ) : isAuthenticated ? (
            <div>
              <strong>Oturum Açık:</strong> IdentityApp çerezi gönderiliyor. Cart.API JWT doğrulamasını tamamlıyor...
            </div>
          ) : (
            <div>
              <strong>Misafir / Anonim İstek:</strong> JWT çerezi bulunamadı. Sepet işlemleri misafir kullanıcısı üzerinden yürütülür.
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
                  Stok Adedi: {item.quantity}
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

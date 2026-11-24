import React, { useEffect, useRef, useState } from 'react';
import agentAppointment from '../../lib/agentAppointment';
import { useAuth } from '../../context/AuthContext';

function Appointment() {
  const { user, isAuthenticated } = useAuth();
  const [appointments, setAppointments] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [backendAuthInfo, setBackendAuthInfo] = useState(null);
  const [loading, setLoading] = useState(false);
  const formRef = useRef();

  useEffect(() => {
    getAppointments();
    getDoctors();
    checkBackendAuth();
  }, []);

  const checkBackendAuth = async () => {
    try {
      const response = await agentAppointment.get('appointments/user-info');
      setBackendAuthInfo(response.data);
    } catch (err) {
      console.warn('Could not verify cookie on DoctorAppointmentBookingAPI:', err?.message || err);
    }
  };

  const getAppointments = async () => {
    try {
      const response = await agentAppointment.get('appointments');
      setAppointments(response.data);
    } catch (error) {
      console.error('Error fetching appointments:', error);
    }
  };

  const getDoctors = async () => {
    try {
      const response = await agentAppointment.get('doctors');
      setDoctors(response.data);
    } catch (error) {
      console.error('Error fetching doctors:', error);
    }
  };

  const createAppointment = async (e) => {
    e.preventDefault();
    setLoading(true);
    const formData = new FormData(e.target);
    const appointmentData = {
      doctorId: formData.get('doctorId'),
      fullName: formData.get('fullName'),
      phone: formData.get('phone'),
      email: formData.get('email'),
      startTime: formData.get('startTime'),
      endTime: formData.get('endTime'),
    };
    try {
      const response = await agentAppointment.post('appointments', appointmentData);
      if (response.status === 204 || response.status === 200 || response.status === 201) {
        getAppointments();
        formRef.current.reset();
        alert('Randevu başarıyla oluşturuldu!');
      }
    } catch (error) {
      console.error('Error creating appointment:', error?.response?.data);
      alert('Randevu oluşturulamadı. Sunucu bağlantısını veya yetkiyi kontrol ediniz.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '800px', margin: '0 auto', padding: '32px 20px', textAlign: 'left' }}>
      <header style={{ marginBottom: '24px' }}>
        <h1 style={{ fontSize: '32px', margin: '0 0 8px 0' }}>Doktor Randevu Sistemi</h1>
        <p style={{ color: 'var(--text)', margin: 0 }}>
          <strong>DoctorAppointmentBookingAPI</strong> (Port 5085) servisi, IdentityApp çerezi ile bağlanmıştır.
        </p>
      </header>

      {/* Cookie Connection Status Banner */}
      <div
        style={{
          background: 'var(--code-bg)',
          border: '1px solid var(--border)',
          borderLeft: backendAuthInfo?.isAuthenticated || isAuthenticated ? '4px solid #16a34a' : '4px solid #eab308',
          borderRadius: '8px',
          padding: '14px 18px',
          marginBottom: '24px',
          fontSize: '14px',
          display: 'flex',
          alignItems: 'center',
          gap: '12px',
        }}
      >
        <span style={{ fontSize: '20px' }}>
          {backendAuthInfo?.isAuthenticated || isAuthenticated ? '🍪' : '👤'}
        </span>
        <div>
          {backendAuthInfo?.isAuthenticated ? (
            <div>
              <strong style={{ color: '#16a34a' }}>IdentityApp Çerezi Doğrulandı:</strong> DoctorAppointmentBookingAPI, oturum açmış kullanıcıyı tanıdı (<code>{backendAuthInfo.email}</code>). İstekler <code>withCredentials: true</code> ile gönderiliyor.
            </div>
          ) : isAuthenticated ? (
            <div>
              <strong>Oturum Aktif ({user?.email}):</strong> İstekler IdentityApp çerezi ile DoctorAppointmentBookingAPI'ye iletiliyor.
            </div>
          ) : (
            <div>
              <strong>Anonim / Misafir İstek:</strong> Aktif bir IdentityApp çerezi bulunamadı. Randevu alırken bilgilerinizi elle doldurabilirsiniz.
            </div>
          )}
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '32px', alignItems: 'start' }}>
        {/* Create Appointment Form */}
        <div
          style={{
            background: 'var(--bg)',
            border: '1px solid var(--border)',
            borderRadius: '12px',
            padding: '24px',
            boxShadow: 'var(--shadow)',
          }}
        >
          <h2 style={{ fontSize: '20px', marginTop: 0, marginBottom: '16px' }}>Yeni Randevu Al</h2>
          <form ref={formRef} onSubmit={createAppointment} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              <label style={{ fontSize: '13px', fontWeight: 600 }}>Doktor:</label>
              <select
                name="doctorId"
                required
                style={{
                  padding: '10px',
                  borderRadius: '6px',
                  border: '1px solid var(--border)',
                  background: 'var(--bg)',
                  color: 'var(--text-h)',
                }}
              >
                <option value="">Doktor Seçiniz</option>
                {doctors.map((doctor) => (
                  <option key={doctor.id} value={doctor.id}>
                    {doctor.fullName}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              <label style={{ fontSize: '13px', fontWeight: 600 }}>Hasta Ad Soyad:</label>
              <input
                type="text"
                name="fullName"
                required
                placeholder="Ad Soyad"
                style={{
                  padding: '10px',
                  borderRadius: '6px',
                  border: '1px solid var(--border)',
                  background: 'var(--bg)',
                  color: 'var(--text-h)',
                }}
              />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              <label style={{ fontSize: '13px', fontWeight: 600 }}>Telefon:</label>
              <input
                type="text"
                name="phone"
                required
                placeholder="0555..."
                style={{
                  padding: '10px',
                  borderRadius: '6px',
                  border: '1px solid var(--border)',
                  background: 'var(--bg)',
                  color: 'var(--text-h)',
                }}
              />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              <label style={{ fontSize: '13px', fontWeight: 600 }}>E-posta:</label>
              <input
                type="email"
                name="email"
                required
                defaultValue={user?.email || ''}
                placeholder="ornek@domain.com"
                style={{
                  padding: '10px',
                  borderRadius: '6px',
                  border: '1px solid var(--border)',
                  background: 'var(--bg)',
                  color: 'var(--text-h)',
                }}
              />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              <label style={{ fontSize: '13px', fontWeight: 600 }}>Başlangıç Zamanı:</label>
              <input
                type="datetime-local"
                name="startTime"
                required
                style={{
                  padding: '10px',
                  borderRadius: '6px',
                  border: '1px solid var(--border)',
                  background: 'var(--bg)',
                  color: 'var(--text-h)',
                }}
              />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              <label style={{ fontSize: '13px', fontWeight: 600 }}>Bitiş Zamanı:</label>
              <input
                type="datetime-local"
                name="endTime"
                required
                style={{
                  padding: '10px',
                  borderRadius: '6px',
                  border: '1px solid var(--border)',
                  background: 'var(--bg)',
                  color: 'var(--text-h)',
                }}
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              style={{
                marginTop: '10px',
                padding: '12px',
                borderRadius: '8px',
                border: 'none',
                background: 'var(--accent)',
                color: '#fff',
                fontWeight: 600,
                cursor: 'pointer',
              }}
            >
              {loading ? 'Randevu Oluşturuluyor...' : 'Randevu Oluştur'}
            </button>
          </form>
        </div>

        {/* Existing Appointments List */}
        <div>
          <h2 style={{ fontSize: '20px', marginTop: 0, marginBottom: '16px' }}>Mevcut Randevular</h2>
          {appointments.length === 0 ? (
            <p style={{ color: 'var(--text)', fontSize: '14px' }}>Kayıtlı randevu bulunmamaktadır.</p>
          ) : (
            <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: '12px' }}>
              {appointments.map((appointment) => (
                <li
                  key={appointment.id}
                  style={{
                    background: 'var(--bg)',
                    border: '1px solid var(--border)',
                    borderRadius: '8px',
                    padding: '14px',
                    boxShadow: 'var(--shadow)',
                  }}
                >
                  <div style={{ fontWeight: 600, color: 'var(--text-h)', marginBottom: '4px' }}>
                    🩺 {appointment.doctorName}
                  </div>
                  <div style={{ fontSize: '14px', color: 'var(--text)', marginBottom: '4px' }}>
                    👤 Hasta: {appointment.patientName}
                  </div>
                  <div style={{ fontSize: '12px', color: 'var(--text)' }}>
                    ⏰ {new Date(appointment.startTime).toLocaleString('tr-TR')} - {new Date(appointment.endTime).toLocaleTimeString('tr-TR')}
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}

export default Appointment;

using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Domain.Enums
{
    public enum PrescriptionStatus
    {
        Pending = 1,    // Oluşturuldu, eczaneden teslim bekleniyor
        Dispensed = 2,  // Eczaneden teslim alındı (stok düşüldü)
        Cancelled = 3,  // İptal edildi
        Expired = 4     // Geçerlilik süresi doldu
    }
}

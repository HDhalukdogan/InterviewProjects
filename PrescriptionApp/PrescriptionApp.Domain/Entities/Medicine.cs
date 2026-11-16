using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Domain.Entities
{
    public class Medicine : BaseEntity
    {
        public string Barcode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public bool IsSpecialControl { get; set; } // Kırmızı/Yeşil reçete kontrolü

        // PostgreSQL xmin sistem kolonu ile Optimistic Concurrency koruması
        public uint RowVersion { get; set; }

        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = [];
    }
}

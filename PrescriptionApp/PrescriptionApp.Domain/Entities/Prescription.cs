using PrescriptionApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Domain.Entities
{
    public class Prescription : BaseEntity
    {
        public string PrescriptionCode { get; set; } = null!; // Örn: RX-2026-A8F91C
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Pending;
        public DateTime ExpiresAt { get; set; }
        public DateTime? DispensedAt { get; set; }

        public Doctor Doctor { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public ICollection<PrescriptionItem> Items { get; set; } = [];
    }
}

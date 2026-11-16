using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Domain.Entities
{
    public class PrescriptionItem : BaseEntity
    {
        public int PrescriptionId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public string DosageInstructions { get; set; } = null!; // Örn: "Günde 2x1 tok karnına"

        public Prescription Prescription { get; set; } = null!;
        public Medicine Medicine { get; set; } = null!;
    }
}

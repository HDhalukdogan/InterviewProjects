using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string IdentityNumber { get; set; } = null!; // TC Kimlik No
        public string Email { get; set; } = null!;

        public ICollection<Prescription> Prescriptions { get; set; } = [];
    }
}

using System.ComponentModel.DataAnnotations;

namespace DoctorRedis.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        [MinLength(11, ErrorMessage = "Identity number must be 11 characters long.")]
        [MaxLength(11, ErrorMessage = "Identity number must be 11 characters long.")]
        public required string IdentityNumber { get; set; }
        public required string FullName { get; set; }
        public string? Email { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

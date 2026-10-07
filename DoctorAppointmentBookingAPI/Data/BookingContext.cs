using DoctorAppointmentBookingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentBookingAPI.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}

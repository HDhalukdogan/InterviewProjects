using DoctorRedis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoctorRedis.Persistence.Contexts
{
    public class DoctorRedisContext : DbContext
    {
        public DoctorRedisContext(DbContextOptions<DoctorRedisContext> options) : base(options)
        {

        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentSlot> AppointmentSlots { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.IdentityNumber)
                .IsUnique();

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.SlotId)
                .IsUnique();
        }
    }
}

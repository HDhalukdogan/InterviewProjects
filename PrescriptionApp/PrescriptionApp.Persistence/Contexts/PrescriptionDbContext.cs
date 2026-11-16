using Microsoft.EntityFrameworkCore;
using PrescriptionApp.Domain.Entities;
using PrescriptionApp.Persistence.Configurations;
using System.Reflection;

namespace PrescriptionApp.Persistence.Contexts
{
    public class PrescriptionDbContext(DbContextOptions<PrescriptionDbContext> options) : DbContext(options)
    {
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrescriptionConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}

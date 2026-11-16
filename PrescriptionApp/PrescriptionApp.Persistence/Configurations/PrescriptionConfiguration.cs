using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrescriptionApp.Domain.Entities;

namespace PrescriptionApp.Persistence.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasIndex(p => p.PrescriptionCode).IsUnique();
            builder.Property(p => p.PrescriptionCode).HasMaxLength(20).IsRequired();

            builder.HasOne(p => p.Doctor)
                   .WithMany(d => d.Prescriptions)
                   .HasForeignKey(p => p.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Patient)
                   .WithMany(pt => pt.Prescriptions)
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrescriptionApp.Domain.Entities;

namespace PrescriptionApp.Persistence.Configurations
{
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
    {
        public void Configure(EntityTypeBuilder<Medicine> builder)
        {
            builder.HasIndex(m => m.Barcode).IsUnique();
            builder.Property(m => m.Barcode).HasMaxLength(30).IsRequired();
            builder.Property(m => m.Name).HasMaxLength(150).IsRequired();
            builder.Property(m => m.Price).HasPrecision(18, 2);

            // PostgreSQL'de satır bazlı eşzamanlılık (Optimistic Concurrency) kilidi
            builder.Property(m => m.RowVersion).IsRowVersion();
        }
    }
}

using AmbulanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceApp.Persistence.Contexts;

public class AmbulanceAppDbContext(DbContextOptions<AmbulanceAppDbContext> options) : DbContext(options)
{
    public DbSet<Ambulance> Ambulances => Set<Ambulance>();
    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<EmergencyCall> EmergencyCalls => Set<EmergencyCall>();
    public DbSet<Dispatch> Dispatches => Set<Dispatch>();
    public DbSet<DispatchAuditLog> DispatchAuditLogs => Set<DispatchAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Ambulance
        modelBuilder.Entity<Ambulance>(entity =>
        {
            entity.HasIndex(a => a.PlateNumber).IsUnique();
            entity.HasIndex(a => a.CallSign).IsUnique();

            entity.Property(a => a.PlateNumber).HasMaxLength(15).IsRequired();
            entity.Property(a => a.CallSign).HasMaxLength(20).IsRequired();
            entity.Property(a => a.CurrentLatitude).HasPrecision(9, 6);
            entity.Property(a => a.CurrentLongitude).HasPrecision(9, 6);

            // PostgreSQL xmin ile eşzamanlı atama (Optimistic Concurrency) kilidi
            entity.Property(a => a.RowVersion).IsRowVersion();
        });

        // 2. StaffMember
        modelBuilder.Entity<StaffMember>(entity =>
        {
            entity.HasIndex(s => s.RegistrationNo).IsUnique();

            entity.Property(s => s.FullName).HasMaxLength(150).IsRequired();
            entity.Property(s => s.RegistrationNo).HasMaxLength(30).IsRequired();

            entity.HasOne(s => s.Ambulance)
                  .WithMany(a => a.StaffMembers)
                  .HasForeignKey(s => s.AmbulanceId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 3. Hospital
        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.Property(h => h.Name).HasMaxLength(150).IsRequired();
            entity.Property(h => h.District).HasMaxLength(100).IsRequired();
            entity.Property(h => h.Latitude).HasPrecision(9, 6);
            entity.Property(h => h.Longitude).HasPrecision(9, 6);
        });

        // 4. EmergencyCall
        modelBuilder.Entity<EmergencyCall>(entity =>
        {
            entity.HasIndex(e => e.CallCode).IsUnique();
            entity.HasIndex(e => new { e.Status, e.Priority });

            entity.Property(e => e.CallCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CallerName).HasMaxLength(150).IsRequired();
            entity.Property(e => e.CallerPhone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.IncidentAddress).HasMaxLength(300).IsRequired();
            entity.Property(e => e.IncidentType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Latitude).HasPrecision(9, 6);
            entity.Property(e => e.Longitude).HasPrecision(9, 6);
        });

        // 5. Dispatch
        modelBuilder.Entity<Dispatch>(entity =>
        {
            entity.Property(d => d.CaseNotes).HasMaxLength(500);

            entity.HasOne(d => d.EmergencyCall)
                  .WithMany(e => e.Dispatches)
                  .HasForeignKey(d => d.EmergencyCallId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Ambulance)
                  .WithMany(a => a.Dispatches)
                  .HasForeignKey(d => d.AmbulanceId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Hospital)
                  .WithMany(h => h.Dispatches)
                  .HasForeignKey(d => d.HospitalId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 6. DispatchAuditLog
        modelBuilder.Entity<DispatchAuditLog>(entity =>
        {
            entity.Property(l => l.EventType).HasMaxLength(50).IsRequired();
            entity.Property(l => l.Payload).HasColumnType("jsonb").IsRequired();

            entity.HasOne(l => l.Dispatch)
                  .WithMany(d => d.AuditLogs)
                  .HasForeignKey(l => l.DispatchId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Compliance.Entities;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
  public void Configure(EntityTypeBuilder<Shift> builder)
  {
    builder.ToTable("Shifts");

    builder.HasKey(s => s.Id);

    // Indexes
    builder.HasIndex(s => s.OrganizationId);
    builder.HasIndex(s => s.RosterId);
    builder.HasIndex(s => s.EmployeeId);
    builder.HasIndex(s => new { s.EmployeeId, s.Date });
    builder.HasIndex(s => s.Date);

    // Date column
    builder.Property(s => s.Date)
        .IsRequired()
        .HasColumnType("date");

    // Time columns (stored as TimeSpan)
    builder.Property(s => s.StartTime)
        .IsRequired();

    builder.Property(s => s.EndTime)
        .IsRequired();

    // String columns
    builder.Property(s => s.PublicHolidayName)
        .HasMaxLength(100);

    builder.Property(s => s.Location)
        .HasMaxLength(100);

    builder.Property(s => s.Notes)
        .HasMaxLength(500);

    // Relationships
    builder.HasOne(s => s.Organization)
        .WithMany()
        .HasForeignKey(s => s.OrganizationId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(s => s.Roster)
        .WithMany(r => r.Shifts)
        .HasForeignKey(s => s.RosterId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(s => s.Employee)
        .WithMany(e => e.Shifts)
        .HasForeignKey(s => s.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasMany(s => s.Issues)
        .WithOne(i => i.Shift)
        .HasForeignKey(i => i.ShiftId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
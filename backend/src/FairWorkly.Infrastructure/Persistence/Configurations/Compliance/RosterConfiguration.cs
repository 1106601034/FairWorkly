using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Compliance.Entities;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

public class RosterConfiguration : IEntityTypeConfiguration<Roster>
{
  public void Configure(EntityTypeBuilder<Roster> builder)
  {
    builder.ToTable("Rosters");

    builder.HasKey(r => r.Id);

    // Indexes
    builder.HasIndex(r => r.OrganizationId);
    builder.HasIndex(r => new { r.OrganizationId, r.WeekStartDate, r.WeekEndDate });
    builder.HasIndex(r => new { r.Year, r.WeekNumber });
    builder.HasIndex(r => r.IsFinalized);

    // Date columns
    builder.Property(r => r.WeekStartDate)
        .IsRequired()
        .HasColumnType("date");

    builder.Property(r => r.WeekEndDate)
        .IsRequired()
        .HasColumnType("date");

    // Decimal columns
    builder.Property(r => r.TotalHours)
        .HasPrecision(10, 2);

    // String columns
    builder.Property(r => r.Notes)
        .HasMaxLength(1000);

    // Relationships
    builder.HasOne(r => r.Organization)
        .WithMany()
        .HasForeignKey(r => r.OrganizationId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(r => r.RosterValidation)
        .WithOne(rv => rv.Roster)
        .HasForeignKey<RosterValidation>(rv => rv.RosterId)
        .OnDelete(DeleteBehavior.SetNull);

    builder.HasMany(r => r.Shifts)
        .WithOne(s => s.Roster)
        .HasForeignKey(s => s.RosterId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(r => r.Issues)
        .WithOne(i => i.Roster)
        .HasForeignKey(i => i.RosterId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
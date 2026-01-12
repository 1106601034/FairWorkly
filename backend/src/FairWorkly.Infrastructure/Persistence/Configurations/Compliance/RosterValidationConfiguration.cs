using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Compliance.Entities;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

public class RosterValidationConfiguration : IEntityTypeConfiguration<RosterValidation>
{
  public void Configure(EntityTypeBuilder<RosterValidation> builder)
  {
    builder.ToTable("RosterValidations");

    builder.HasKey(rv => rv.Id);

    // Indexes
    builder.HasIndex(rv => rv.OrganizationId);
    builder.HasIndex(rv => rv.RosterId);
    builder.HasIndex(rv => new { rv.OrganizationId, rv.WeekStartDate, rv.WeekEndDate });
    builder.HasIndex(rv => rv.Status);

    // Properties
    builder.Property(rv => rv.Status)
        .IsRequired()
        .HasConversion<int>();

    // Date columns
    builder.Property(rv => rv.WeekStartDate)
        .IsRequired()
        .HasColumnType("date");

    builder.Property(rv => rv.WeekEndDate)
        .IsRequired()
        .HasColumnType("date");

    // Notes
    builder.Property(rv => rv.Notes)
        .HasMaxLength(1000);

    // Relationships
    builder.HasOne(rv => rv.Organization)
        .WithMany()
        .HasForeignKey(rv => rv.OrganizationId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(rv => rv.Roster)
        .WithOne(r => r.RosterValidation)
        .HasForeignKey<RosterValidation>(rv => rv.RosterId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(rv => rv.Issues)
        .WithOne(i => i.RosterValidation)
        .HasForeignKey(i => i.RosterValidationId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
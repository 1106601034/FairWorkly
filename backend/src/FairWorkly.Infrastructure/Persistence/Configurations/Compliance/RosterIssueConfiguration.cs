using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Compliance.Entities;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Compliance;

public class RosterIssueConfiguration : IEntityTypeConfiguration<RosterIssue>
{
  public void Configure(EntityTypeBuilder<RosterIssue> builder)
  {
    builder.ToTable("RosterIssues");

    builder.HasKey(ri => ri.Id);

    // Indexes
    builder.HasIndex(ri => ri.OrganizationId);
    builder.HasIndex(ri => ri.RosterValidationId);
    builder.HasIndex(ri => ri.RosterId);
    builder.HasIndex(ri => ri.ShiftId);
    builder.HasIndex(ri => ri.EmployeeId);
    builder.HasIndex(ri => ri.Severity);
    builder.HasIndex(ri => ri.IsResolved);
    builder.HasIndex(ri => ri.IsWaived);

    // Properties
    builder.Property(ri => ri.CheckType)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(ri => ri.Severity)
        .IsRequired()
        .HasConversion<int>();

    builder.Property(ri => ri.Description)
        .IsRequired()
        .HasMaxLength(500);

    builder.Property(ri => ri.DetailedExplanation)
        .HasColumnType("text");

    builder.Property(ri => ri.Recommendation)
        .HasColumnType("text");

    // Decimal values
    builder.Property(ri => ri.ExpectedValue)
        .HasPrecision(10, 2);

    builder.Property(ri => ri.ActualValue)
        .HasPrecision(10, 2);

    // Affected data
    builder.Property(ri => ri.AffectedDates)
        .HasMaxLength(500);

    // Resolution
    builder.Property(ri => ri.ResolutionNotes)
        .HasMaxLength(1000);

    // Waiver
    builder.Property(ri => ri.WaiverReason)
        .HasMaxLength(1000);

    // Relationships
    builder.HasOne(ri => ri.Organization)
        .WithMany()
        .HasForeignKey(ri => ri.OrganizationId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(ri => ri.RosterValidation)
        .WithMany(rv => rv.Issues)
        .HasForeignKey(ri => ri.RosterValidationId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(ri => ri.Roster)
        .WithMany(r => r.Issues)
        .HasForeignKey(ri => ri.RosterId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(ri => ri.Shift)
        .WithMany(s => s.Issues)
        .HasForeignKey(ri => ri.ShiftId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(ri => ri.Employee)
        .WithMany()
        .HasForeignKey(ri => ri.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(ri => ri.ResolvedByUser)
        .WithMany()
        .HasForeignKey(ri => ri.ResolvedByUserId)
        .OnDelete(DeleteBehavior.SetNull);

    builder.HasOne(ri => ri.WaivedByUser)
        .WithMany()
        .HasForeignKey(ri => ri.WaivedByUserId)
        .OnDelete(DeleteBehavior.SetNull);
  }
}
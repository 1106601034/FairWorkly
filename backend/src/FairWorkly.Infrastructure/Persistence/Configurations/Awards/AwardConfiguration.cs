using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Awards.Entities;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Awards;

public class AwardConfiguration : IEntityTypeConfiguration<Award>
{
  public void Configure(EntityTypeBuilder<Award> builder)
  {
    builder.ToTable("Awards");

    builder.HasKey(a => a.Id);

    // Indexes
    builder.HasIndex(a => a.AwardType).IsUnique();
    builder.HasIndex(a => a.AwardCode);

    // Properties
    builder.Property(a => a.AwardType)
        .IsRequired()
        .HasConversion<int>();

    builder.Property(a => a.Name)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(a => a.AwardCode)
        .IsRequired()
        .HasMaxLength(20);

    builder.Property(a => a.Description)
        .HasMaxLength(1000);

    // Decimal precision
    builder.Property(a => a.SaturdayPenaltyRate)
        .HasPrecision(5, 2);

    builder.Property(a => a.SundayPenaltyRate)
        .HasPrecision(5, 2);

    builder.Property(a => a.PublicHolidayPenaltyRate)
        .HasPrecision(5, 2);

    builder.Property(a => a.CasualLoadingRate)
        .HasPrecision(5, 2);

    builder.Property(a => a.MinimumShiftHours)
        .HasPrecision(5, 2);

    builder.Property(a => a.MealBreakThresholdHours)
        .HasPrecision(5, 2);

    builder.Property(a => a.MinimumRestPeriodHours)
        .HasPrecision(5, 2);

    // Relationships
    builder.HasMany(a => a.Levels)
        .WithOne(al => al.Award)
        .HasForeignKey(al => al.AwardId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
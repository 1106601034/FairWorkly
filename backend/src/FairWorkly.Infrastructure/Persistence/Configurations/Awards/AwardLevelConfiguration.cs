using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Awards.Entities;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Awards;

public class AwardLevelConfiguration : IEntityTypeConfiguration<AwardLevel>
{
  public void Configure(EntityTypeBuilder<AwardLevel> builder)
  {
    builder.ToTable("AwardLevels");

    builder.HasKey(al => al.Id);

    // Indexes
    builder.HasIndex(al => new { al.AwardId, al.LevelNumber, al.IsActive });
    builder.HasIndex(al => al.EffectiveFrom);

    // Properties
    builder.Property(al => al.LevelName)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(al => al.Description)
        .HasMaxLength(1000);

    // Decimal precision for pay rates
    builder.Property(al => al.FullTimeHourlyRate)
        .IsRequired()
        .HasPrecision(10, 2);

    builder.Property(al => al.PartTimeHourlyRate)
        .IsRequired()
        .HasPrecision(10, 2);

    builder.Property(al => al.CasualHourlyRate)
        .IsRequired()
        .HasPrecision(10, 2);

    // Date columns
    builder.Property(al => al.EffectiveFrom)
        .IsRequired()
        .HasColumnType("date");

    builder.Property(al => al.EffectiveTo)
        .HasColumnType("date");
  }
}
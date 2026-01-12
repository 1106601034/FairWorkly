using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Auth;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
  public void Configure(EntityTypeBuilder<Organization> builder)
  {
    builder.ToTable("Organizations");

    builder.HasKey(o => o.Id);

    // Indexes
    builder.HasIndex(o => o.ABN).IsUnique();
    builder.HasIndex(o => o.Email);

    // Properties
    builder.Property(o => o.Name)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(o => o.ABN)
        .IsRequired()
        .HasMaxLength(11);

    builder.Property(o => o.Email)
        .HasMaxLength(255);

    builder.Property(o => o.Phone)
        .HasMaxLength(20);

    builder.Property(o => o.Address)
        .HasMaxLength(500);

    builder.Property(o => o.State)
        .HasConversion<int>();

    builder.Property(o => o.SubscriptionTier)
        .IsRequired()
        .HasConversion<int>();
  }
}
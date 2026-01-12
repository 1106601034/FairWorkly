using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Infrastructure.Persistence.Configurations.Auth;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");

    builder.HasKey(u => u.Id);

    // Indexes
    builder.HasIndex(u => u.Email).IsUnique();
    builder.HasIndex(u => u.OrganizationId);

    // Properties
    builder.Property(u => u.Email)
        .IsRequired()
        .HasMaxLength(255);

    builder.Property(u => u.PasswordHash)
        .IsRequired()
        .HasMaxLength(500);

    builder.Property(u => u.FirstName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(u => u.LastName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(u => u.Role)
        .IsRequired()
        .HasConversion<int>();

    // Relationships
    builder.HasOne(u => u.Organization)
        .WithMany()
        .HasForeignKey(u => u.OrganizationId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
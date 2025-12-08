using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FairWorkly.Infrastructure.Persistence;

/// <summary>
/// Database seeder: used to initialize seed data
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FairWorklyDbContext>();

        // Check if any users exist; if none, it's a new database — start seeding
        if (!await context.Users.AnyAsync())
        {
            // Create default organization (Demo Corp)
            var demoOrg = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Demo Corp Pty Ltd",
                ABN = "12 345 678 901",
                SubscriptionTier = SubscriptionTier.Tier1,
                SubscriptionStartDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
            };

            // Create super administrator
            // Account: admin@fairworkly.com.au
            // Password: Password123!
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@fairworkly.com.au",
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                IsActive = true,
                OrganizationId = demoOrg.Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
            };

            context.Organizations.Add(demoOrg);
            context.Users.Add(adminUser);

            await context.SaveChangesAsync();

            Console.WriteLine("√ Database seeded successfully with Admin user.");
        }
    }
}

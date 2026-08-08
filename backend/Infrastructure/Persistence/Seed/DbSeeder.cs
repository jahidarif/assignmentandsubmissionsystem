using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AssignmentSubmissionSystem.Infrastructure.Persistence.Seed;

/// <summary>
/// Ensures exactly one Admin account exists on startup. Safe to run every
/// time the app boots — it's idempotent (checks before inserting), so it
/// won't duplicate the admin on every restart.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAdminAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger logger)
    {
        await context.Database.MigrateAsync();

        var adminAlreadyExists = await context.Users
            .AnyAsync(u => u.Role == UserRole.Admin);

        if (adminAlreadyExists)
        {
            logger.LogInformation("Admin user already exists — skipping seed.");
            return;
        }

        var fullName = configuration["AdminSeed:FullName"];
        var email = configuration["AdminSeed:Email"];
        var password = configuration["AdminSeed:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "AdminSeed:Email / AdminSeed:Password not configured — skipping admin seed. " +
                "Set them in appsettings.Development.json or environment variables.");
            return;
        }

        var admin = new User
        {
            FullName = string.IsNullOrWhiteSpace(fullName) ? "System Administrator" : fullName,
            Email = email,
            PasswordHash = passwordHasher.Hash(password),
            Role = UserRole.Admin,
            IsActive = true
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded initial Admin account with email {Email}.", email);
    }
}
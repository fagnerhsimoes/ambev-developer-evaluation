using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Ensure database is created/migrated
        await context.Database.MigrateAsync();

        // Look for any users.
        if (await context.Users.AnyAsync())
        {
            return;   // DB has been seeded
        }

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "Admin",
            Email = "admin@ambev.com.br",
            Phone = "(11) 99999-9999",
            Password = passwordHasher.HashPassword("Admin123!"), // Password requirement: 8 chars, 1 upper, 1 lower, 1 number, 1 special
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
    }
}

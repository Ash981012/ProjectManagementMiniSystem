using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ProjectManagement.Domain.Users;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var adminEmail = configuration["DefaultAdmin:Email"];
        var adminPassword = configuration["DefaultAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail))
            throw new InvalidOperationException("Default admin email is not configured. Set 'DefaultAdmin:Email' in configuration or secrets.");

        if (string.IsNullOrWhiteSpace(adminPassword))
            throw new InvalidOperationException("Default admin password is not configured. Set 'DefaultAdmin:Password' in configuration or secrets.");

        await EnsureRoleAsync(roleManager, RoleNames.Admin);
        await EnsureRoleAsync(roleManager, RoleNames.Employee);
        await EnsureAdminUserAsync(userManager, adminEmail, adminPassword);
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            ThrowIfFailed(result, $"Could not create role '{role}'.");
        }
    }

    private static async Task EnsureAdminUserAsync(UserManager<ApplicationUser> userManager, string adminEmail, string adminPassword)
    {
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator"
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            ThrowIfFailed(createResult, "Could not create default admin user.");
        }

        if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(admin, RoleNames.Admin);
            ThrowIfFailed(roleResult, "Could not assign admin role to default admin user.");
        }
    }

    private static void ThrowIfFailed(IdentityResult result, string message)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"{message} {errors}");
    }
}

using Microsoft.AspNetCore.Identity;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Users.Dtos;
using ProjectManagement.Domain.Users;

namespace ProjectManagement.Infrastructure.Identity;

public sealed class IdentityUserRegistrationService(UserManager<ApplicationUser> userManager) : IUserRegistrationService
{
    public async Task<UserRegistrationResult> RegisterEmployeeAsync(
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        var trimmedEmail = email?.Trim() ?? string.Empty;

        var user = new ApplicationUser
        {
            UserName = trimmedEmail,
            Email = trimmedEmail,
            EmailConfirmed = true,
            FullName = trimmedFullName
        };

        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
            return ToFailedResult(createResult);

        var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Employee);

        if (roleResult.Succeeded)
            return UserRegistrationResult.Success();

        await userManager.DeleteAsync(user);
        return ToFailedResult(roleResult);
    }

    private static UserRegistrationResult ToFailedResult(IdentityResult result)
    {
        return UserRegistrationResult.Failed(result.Errors.Select(error => error.Description));
    }
}

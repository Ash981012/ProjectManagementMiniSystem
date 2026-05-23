using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Projects.Dtos;
using ProjectManagement.Domain.Users;

namespace ProjectManagement.Infrastructure.Identity;

public sealed class IdentityEmployeeDirectory : IEmployeeDirectory
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityEmployeeDirectory(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<EmployeeDto?> FindEmployeeAsync(string employeeId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == employeeId, cancellationToken);

        if (user is null || !await _userManager.IsInRoleAsync(user, RoleNames.Employee))
            return null;

        return ToDto(user);
    }

    public async Task<IReadOnlyList<EmployeeDto>> ListEmployeesAsync(CancellationToken cancellationToken)
    {
        var employees = await _userManager.GetUsersInRoleAsync(RoleNames.Employee);

        return employees
            .OrderBy(user => user.FullName)
            .ThenBy(user => user.Email)
            .Select(ToDto)
            .ToList();
    }

    private static EmployeeDto ToDto(ApplicationUser user)
    {
        var displayName = string.IsNullOrWhiteSpace(user.FullName)
            ? user.Email ?? user.UserName ?? user.Id
            : user.FullName;

        return new EmployeeDto(user.Id, displayName, user.Email ?? string.Empty);
    }
}

using ProjectManagement.Application.Users.Dtos;

namespace ProjectManagement.Application.Abstractions;

public interface IUserRegistrationService
{
    Task<UserRegistrationResult> RegisterEmployeeAsync(
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken);
}

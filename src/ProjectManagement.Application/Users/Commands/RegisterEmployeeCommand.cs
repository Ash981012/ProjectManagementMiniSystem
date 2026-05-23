using FluentValidation;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;
using ProjectManagement.Application.Users.Dtos;

namespace ProjectManagement.Application.Users.Commands;

public sealed record RegisterEmployeeCommand(
    string FullName,
    string Email,
    string Password);

public sealed class RegisterEmployeeCommandHandler(
    IUserRegistrationService users,
    IValidator<RegisterEmployeeCommand> validator)
{
    public async Task<UserRegistrationResult> Handle(RegisterEmployeeCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowUseCaseExceptionAsync(command, cancellationToken);

        return await users.RegisterEmployeeAsync(
            command.FullName.Trim(),
            command.Email.Trim(),
            command.Password,
            cancellationToken);
    }

}

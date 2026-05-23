namespace ProjectManagement.Application.Users.Dtos;

public sealed record UserRegistrationResult(
    bool Succeeded,
    IReadOnlyList<string> Errors)
{
    public static UserRegistrationResult Success()
    {
        return new UserRegistrationResult(true, []);
    }

    public static UserRegistrationResult Failed(IEnumerable<string> errors)
    {
        return new UserRegistrationResult(false, errors.Where(error => !string.IsNullOrWhiteSpace(error)).ToList());
    }
}

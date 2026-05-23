namespace ProjectManagement.Application.Common;

public sealed class UseCaseException : Exception
{
    public UseCaseException(string message)
        : base(message)
    {
    }
}

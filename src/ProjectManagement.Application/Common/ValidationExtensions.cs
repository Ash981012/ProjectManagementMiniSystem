using FluentValidation;
using FluentValidation.Results;

namespace ProjectManagement.Application.Common;

public static class ValidationExtensions
{
    public static async Task ValidateAndThrowUseCaseExceptionAsync<T>(
        this IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(instance);

        ValidationResult result = await validator.ValidateAsync(instance, cancellationToken);

        if (result.IsValid)
            return;

        var message = string.Join(" ", result.Errors.Select(error => error.ErrorMessage));
        throw new UseCaseException(message);
    }
}

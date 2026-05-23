using FluentValidation;

namespace ProjectManagement.Application.Projects.Commands.Validators;

public sealed class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty().WithMessage("Task id is required.");

        RuleFor(command => command.RowVersion)
            .NotEmpty().WithMessage("Row version is required.")
            .Must(BeValidBase64RowVersion).WithMessage("Row version is invalid.");

        RuleFor(command => command.Status)
            .IsInEnum().WithMessage("Task status is invalid.");

        RuleFor(command => command.CurrentUserId)
            .NotEmpty().WithMessage("Current user id is required.");
    }

    private static bool BeValidBase64RowVersion(string rowVersion)
    {
        if (string.IsNullOrWhiteSpace(rowVersion))
            return false;

        try
        {
            return Convert.FromBase64String(rowVersion).Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

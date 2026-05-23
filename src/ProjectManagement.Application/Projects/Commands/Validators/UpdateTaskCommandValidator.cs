using FluentValidation;

namespace ProjectManagement.Application.Projects.Commands.Validators;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty().WithMessage("Task id is required.");

        RuleFor(command => command.RowVersion)
            .NotEmpty().WithMessage("Row version is required.")
            .Must(BeValidBase64RowVersion).WithMessage("Row version is invalid.");

        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(160).WithMessage("Task title must not exceed 160 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(1000).WithMessage("Task description must not exceed 1000 characters.");

        RuleFor(command => command.AssignedEmployeeId)
            .NotEmpty().WithMessage("Assigned employee is required.");
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

using FluentValidation;

namespace ProjectManagement.Application.Projects.Commands.Validators;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty().WithMessage("Project id is required.");

        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(160).WithMessage("Task title must not exceed 160 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(1000).WithMessage("Task description must not exceed 1000 characters.");

        RuleFor(command => command.AssignedEmployeeId)
            .NotEmpty().WithMessage("Assigned employee is required.");
    }
}

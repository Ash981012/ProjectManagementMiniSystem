using FluentValidation;

namespace ProjectManagement.Application.Projects.Commands.Validators;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(120).WithMessage("Project name must not exceed 120 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(1000).WithMessage("Project description must not exceed 1000 characters.");
    }
}

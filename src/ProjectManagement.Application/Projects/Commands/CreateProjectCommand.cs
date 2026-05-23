using FluentValidation;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;
using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Application.Projects.Commands;

public sealed record CreateProjectCommand(string Name, string? Description);

public sealed class CreateProjectCommandHandler(
    IProjectRepository projects,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    IAppCache cache,
    IValidator<CreateProjectCommand> validator)
{
    public async Task<Guid> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowUseCaseExceptionAsync(command, cancellationToken);

        var project = Project.Create(command.Name, command.Description, clock.UtcNow);

        await projects.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cache.Remove(CacheKeys.Dashboard);

        return project.Id;
    }

}

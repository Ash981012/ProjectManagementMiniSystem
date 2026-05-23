using FluentValidation;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;

namespace ProjectManagement.Application.Projects.Commands;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    string AssignedEmployeeId);

public sealed class CreateTaskCommandHandler(
    IProjectRepository projects,
    IEmployeeDirectory employees,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    IAppCache cache,
    IValidator<CreateTaskCommand> validator)
{
    public async Task<Guid> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowUseCaseExceptionAsync(command, cancellationToken);

        var project = await projects.GetByIdWithTasksAsync(command.ProjectId, trackChanges: true, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        var employee = await employees.FindEmployeeAsync(command.AssignedEmployeeId, cancellationToken)
            ?? throw new NotFoundException("Assigned employee was not found.");

        var task = project.AddTask(
            command.Title,
            command.Description,
            employee.Id,
            employee.Name,
            clock.UtcNow);

        await projects.AddTaskAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cache.Remove(CacheKeys.Dashboard);

        return task.Id;
    }

}

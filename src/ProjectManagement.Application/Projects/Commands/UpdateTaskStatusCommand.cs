using FluentValidation;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;
using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Application.Projects.Commands;

public sealed record UpdateTaskStatusCommand(
    Guid TaskId,
    TaskWorkflowStatus Status,
    string CurrentUserId,
    string RowVersion);

public sealed class UpdateTaskStatusCommandHandler(
    IProjectRepository projects,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    IAppCache cache,
    IValidator<UpdateTaskStatusCommand> validator)
{
    public async Task Handle(UpdateTaskStatusCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowUseCaseExceptionAsync(command, cancellationToken);

        var project = await projects.GetByTaskIdWithTasksAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var task = project.GetTask(command.TaskId);

        if (!string.Equals(task.AssignedEmployeeId, command.CurrentUserId, StringComparison.Ordinal))
            throw new ForbiddenAccessException("Employees can update only their assigned tasks.");

        projects.SetTaskOriginalRowVersion(task, Convert.FromBase64String(command.RowVersion));

        task.ChangeStatus(command.Status, clock.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        cache.Remove(CacheKeys.Dashboard);
    }

}

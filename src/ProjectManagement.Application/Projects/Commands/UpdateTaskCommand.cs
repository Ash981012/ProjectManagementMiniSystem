using FluentValidation;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;

namespace ProjectManagement.Application.Projects.Commands;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description,
    string AssignedEmployeeId,
    string RowVersion);

public sealed class UpdateTaskCommandHandler(
    IProjectRepository projects,
    IEmployeeDirectory employees,
    IUnitOfWork unitOfWork,
    IAppCache cache,
    IValidator<UpdateTaskCommand> validator)
{
    public async Task Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowUseCaseExceptionAsync(command, cancellationToken);

        var project = await projects.GetByTaskIdWithTasksAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var task = project.GetTask(command.TaskId);
        var employee = await employees.FindEmployeeAsync(command.AssignedEmployeeId, cancellationToken)
            ?? throw new NotFoundException("Assigned employee was not found.");

        projects.SetTaskOriginalRowVersion(task, Convert.FromBase64String(command.RowVersion));

        task.UpdateDetails(command.Title, command.Description);
        task.Reassign(employee.Id, employee.Name);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Avoid leaking EF Core into Application layer. Detect EF concurrency exception by type name
            if (ex.GetType().FullName == "Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException")
                throw new UseCaseException("The task was modified by another user. Please refresh and try again.");

            throw;
        }

        cache.Remove(CacheKeys.Dashboard);
    }

}

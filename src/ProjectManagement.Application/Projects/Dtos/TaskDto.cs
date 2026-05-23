using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Application.Projects.Dtos;

public sealed record TaskDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string AssignedEmployeeId,
    string AssignedEmployeeName,
    TaskWorkflowStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    string RowVersion)
{
    public static TaskDto FromDomain(ProjectTask task)
    {
        return new TaskDto(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.AssignedEmployeeId,
            task.AssignedEmployeeName,
            task.Status,
            task.CreatedAt,
            task.StartedAt,
            task.CompletedAt,
            Convert.ToBase64String(task.RowVersion));
    }
}

using ProjectManagement.Domain.Common;

namespace ProjectManagement.Domain.Projects;

public sealed class ProjectTask : Entity<Guid>
{
    private ProjectTask()
    {
    }

    private ProjectTask(
        Guid id,
        Guid projectId,
        string title,
        string? description,
        string assignedEmployeeId,
        string assignedEmployeeName,
        DateTimeOffset createdAt)
    {
        Id = id;
        ProjectId = projectId;
        Title = GuardTitle(title);
        Description = NormalizeOptionalText(description);
        AssignedEmployeeId = GuardEmployeeId(assignedEmployeeId);
        AssignedEmployeeName = GuardEmployeeName(assignedEmployeeName);
        Status = TaskWorkflowStatus.Todo;
        CreatedAt = createdAt;
    }

    public Guid ProjectId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string AssignedEmployeeId { get; private set; } = string.Empty;

    public string AssignedEmployeeName { get; private set; } = string.Empty;

    public TaskWorkflowStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public static ProjectTask Create(
        Guid projectId,
        string title,
        string? description,
        string assignedEmployeeId,
        string assignedEmployeeName,
        DateTimeOffset utcNow)
    {
        if (projectId == Guid.Empty)
            throw new DomainException("Project id is required.");

        return new ProjectTask(
            Guid.NewGuid(),
            projectId,
            title,
            description,
            assignedEmployeeId,
            assignedEmployeeName,
            utcNow);
    }

    public void UpdateDetails(string title, string? description)
    {
        Title = GuardTitle(title);
        Description = NormalizeOptionalText(description);
    }

    public void Reassign(string assignedEmployeeId, string assignedEmployeeName)
    {
        AssignedEmployeeId = GuardEmployeeId(assignedEmployeeId);
        AssignedEmployeeName = GuardEmployeeName(assignedEmployeeName);
    }

    public void ChangeStatus(TaskWorkflowStatus newStatus, DateTimeOffset utcNow)
    {
        if (!Enum.IsDefined(newStatus))
            throw new DomainException("Task status is invalid.");

        if (Status == newStatus)
            return;

        if (newStatus == TaskWorkflowStatus.Done && StartedAt is null)
            throw new DomainException("A task must be started before it can be completed.");

        Status = newStatus;

        if (newStatus == TaskWorkflowStatus.InProgress && StartedAt is null)
            StartedAt = utcNow;

        CompletedAt = newStatus == TaskWorkflowStatus.Done ? utcNow : null;
    }

    private static string GuardTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        return title.Trim();
    }

    private static string GuardEmployeeId(string assignedEmployeeId)
    {
        if (string.IsNullOrWhiteSpace(assignedEmployeeId))
            throw new DomainException("A task must be assigned to an employee.");

        return assignedEmployeeId.Trim();
    }

    private static string GuardEmployeeName(string assignedEmployeeName)
    {
        if (string.IsNullOrWhiteSpace(assignedEmployeeName))
            throw new DomainException("Assigned employee name is required.");

        return assignedEmployeeName.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

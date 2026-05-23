using ProjectManagement.Domain.Common;

namespace ProjectManagement.Domain.Projects;

public sealed class Project : Entity<Guid>
{
    private readonly List<ProjectTask> _tasks = [];

    private Project()
    {
    }

    private Project(Guid id, string name, string? description, DateTimeOffset createdAt)
    {
        Id = id;
        Name = GuardName(name);
        Description = NormalizeOptionalText(description);
        CreatedAt = createdAt;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<ProjectTask> Tasks => _tasks.AsReadOnly();

    public int TaskCount => _tasks.Count;

    public int CompletedTaskCount => _tasks.Count(task => task.Status == TaskWorkflowStatus.Done);

    public decimal ProgressPercentage => ProjectProgressCalculator.CalculateProjectProgress(_tasks);

    public static Project Create(string name, string? description, DateTimeOffset utcNow)
    {
        return new Project(Guid.NewGuid(), name, description, utcNow);
    }

    public void Rename(string name, string? description)
    {
        Name = GuardName(name);
        Description = NormalizeOptionalText(description);
    }

    public ProjectTask AddTask(
        string title,
        string? description,
        string assignedEmployeeId,
        string assignedEmployeeName,
        DateTimeOffset utcNow)
    {
        var task = ProjectTask.Create(Id, title, description, assignedEmployeeId, assignedEmployeeName, utcNow);
        _tasks.Add(task);
        return task;
    }

    public ProjectTask GetTask(Guid taskId)
    {
        return _tasks.FirstOrDefault(task => task.Id == taskId)
            ?? throw new DomainException("Task was not found in this project.");
    }

    private static string GuardName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name is required.");

        return name.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

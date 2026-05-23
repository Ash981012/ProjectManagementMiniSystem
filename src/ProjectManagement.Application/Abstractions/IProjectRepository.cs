using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Application.Abstractions;

public interface IProjectRepository
{
    Task AddAsync(Project project, CancellationToken cancellationToken);

    Task AddTaskAsync(ProjectTask task, CancellationToken cancellationToken);

    Task<Project?> GetByIdWithTasksAsync(Guid projectId, bool trackChanges, CancellationToken cancellationToken);

    Task<Project?> GetByTaskIdWithTasksAsync(Guid taskId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> ListWithTasksAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectTask>> ListTasksByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken);

    void SetTaskOriginalRowVersion(ProjectTask task, byte[] rowVersion);
}

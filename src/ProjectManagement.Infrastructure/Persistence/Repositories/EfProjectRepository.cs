using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Infrastructure.Persistence.Repositories;

public sealed class EfProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EfProjectRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await _dbContext.Projects.AddAsync(project, cancellationToken);
    }

    public async Task AddTaskAsync(ProjectTask task, CancellationToken cancellationToken)
    {
        await _dbContext.ProjectTasks.AddAsync(task, cancellationToken);
    }

    public async Task<Project?> GetByIdWithTasksAsync(
        Guid projectId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        IQueryable<Project> query = _dbContext.Projects.Include(project => project.Tasks);

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(project => project.Id == projectId, cancellationToken);
    }

    public async Task<Project?> GetByTaskIdWithTasksAsync(Guid taskId, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .Include(project => project.Tasks)
            .FirstOrDefaultAsync(project => project.Tasks.Any(task => task.Id == taskId), cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> ListWithTasksAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Tasks)
            .OrderBy(project => project.Name)
            .ToListAsync(cancellationToken);
    }

    public void SetTaskOriginalRowVersion(ProjectTask task, byte[] rowVersion)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(rowVersion);

        _dbContext.Entry(task).Property(currentTask => currentTask.RowVersion).OriginalValue = rowVersion;
    }

    public async Task<IReadOnlyList<ProjectTask>> ListTasksByEmployeeIdAsync(
        string employeeId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ProjectTasks
            .AsNoTracking()
            .Where(task => task.AssignedEmployeeId == employeeId)
            .OrderBy(task => task.Status)
            .ThenBy(task => task.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

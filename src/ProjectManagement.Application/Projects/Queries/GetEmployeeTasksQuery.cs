using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;
using ProjectManagement.Application.Projects.Dtos;

namespace ProjectManagement.Application.Projects.Queries;

public sealed record GetEmployeeTasksQuery(string EmployeeId);

public sealed class GetEmployeeTasksQueryHandler
{
    private readonly IProjectRepository _projects;

    public GetEmployeeTasksQueryHandler(IProjectRepository projects)
    {
        ArgumentNullException.ThrowIfNull(projects);

        _projects = projects;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetEmployeeTasksQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.IsNullOrWhiteSpace(query.EmployeeId))
            throw new UseCaseException("Employee id is required.");

        var tasks = await _projects.ListTasksByEmployeeIdAsync(query.EmployeeId, cancellationToken);

        return tasks
            .Select(TaskDto.FromDomain)
            .ToList();
    }
}

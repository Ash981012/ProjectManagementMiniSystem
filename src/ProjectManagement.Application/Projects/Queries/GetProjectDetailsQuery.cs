using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;
using ProjectManagement.Application.Projects.Dtos;

namespace ProjectManagement.Application.Projects.Queries;

public sealed record GetProjectDetailsQuery(Guid ProjectId);

public sealed class GetProjectDetailsQueryHandler
{
    private readonly IProjectRepository _projects;

    public GetProjectDetailsQueryHandler(IProjectRepository projects)
    {
        ArgumentNullException.ThrowIfNull(projects);

        _projects = projects;
    }

    public async Task<ProjectDetailsDto> Handle(GetProjectDetailsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.ProjectId == Guid.Empty)
            throw new UseCaseException("Project id is required.");

        var project = await _projects.GetByIdWithTasksAsync(query.ProjectId, trackChanges: false, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        return new ProjectDetailsDto(
            project.Id,
            project.Name,
            project.Description,
            project.TaskCount,
            project.CompletedTaskCount,
            project.ProgressPercentage,
            project.Tasks
                .OrderBy(task => task.CreatedAt)
                .Select(TaskDto.FromDomain)
                .ToList());
    }
}

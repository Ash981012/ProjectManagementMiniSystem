using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Common;
using ProjectManagement.Application.Projects.Dtos;
using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Application.Projects.Queries;

public sealed record GetDashboardQuery;

public sealed class GetDashboardQueryHandler
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);
    private readonly IAppCache _cache;
    private readonly IProjectRepository _projects;

    public GetDashboardQueryHandler(IProjectRepository projects, IAppCache cache)
    {
        ArgumentNullException.ThrowIfNull(projects);
        ArgumentNullException.ThrowIfNull(cache);

        _projects = projects;
        _cache = cache;
    }

    public Task<DashboardDto> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return _cache.GetOrCreateAsync(CacheKeys.Dashboard, CacheDuration, BuildDashboardAsync, cancellationToken);
    }

    private async Task<DashboardDto> BuildDashboardAsync(CancellationToken cancellationToken)
    {
        var projects = await _projects.ListWithTasksAsync(cancellationToken);
        var projectSummaries = projects
            .OrderBy(project => project.Name)
            .Select(ProjectSummaryDto.FromDomain)
            .ToList();

        return new DashboardDto(
            projects.Count,
            ProjectProgressCalculator.CalculateOverallAverageProgress(projects),
            projectSummaries);
    }
}

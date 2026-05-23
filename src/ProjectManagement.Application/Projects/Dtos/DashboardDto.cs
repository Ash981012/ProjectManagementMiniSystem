namespace ProjectManagement.Application.Projects.Dtos;

public sealed record DashboardDto(
    int TotalProjects,
    decimal AverageProgressPercentage,
    IReadOnlyList<ProjectSummaryDto> Projects);

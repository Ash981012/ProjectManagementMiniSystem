using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Application.Projects.Dtos;

public sealed record ProjectSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    int TaskCount,
    int CompletedTaskCount,
    decimal ProgressPercentage)
{
    public static ProjectSummaryDto FromDomain(Project project)
    {
        return new ProjectSummaryDto(
            project.Id,
            project.Name,
            project.Description,
            project.TaskCount,
            project.CompletedTaskCount,
            project.ProgressPercentage);
    }
}

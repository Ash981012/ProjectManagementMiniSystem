namespace ProjectManagement.Application.Projects.Dtos;

public sealed record ProjectDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    int TaskCount,
    int CompletedTaskCount,
    decimal ProgressPercentage,
    IReadOnlyList<TaskDto> Tasks);

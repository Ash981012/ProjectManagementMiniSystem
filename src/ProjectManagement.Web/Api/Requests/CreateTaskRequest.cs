using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Web.Api.Requests;

public sealed class CreateTaskRequest
{
    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(160, MinimumLength = 2, ErrorMessage = "Task title must be between 2 and 160 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Assigned employee is required.")]
    public string AssignedEmployeeId { get; set; } = string.Empty;
}

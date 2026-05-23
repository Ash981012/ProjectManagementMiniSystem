using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Web.Api.Requests;

public sealed class CreateProjectRequest
{
    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 120 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }
}

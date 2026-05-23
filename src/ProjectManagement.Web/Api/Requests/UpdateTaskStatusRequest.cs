using System.ComponentModel.DataAnnotations;
using ProjectManagement.Domain.Projects;

namespace ProjectManagement.Web.Api.Requests;

public sealed class UpdateTaskStatusRequest
{
    [EnumDataType(typeof(TaskWorkflowStatus), ErrorMessage = "Task status is invalid.")]
    public TaskWorkflowStatus Status { get; set; }

    [Required(ErrorMessage = "Row version is required.")]
    public string RowVersion { get; set; } = string.Empty;
}

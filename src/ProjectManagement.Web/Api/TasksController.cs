using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Projects.Commands;
using ProjectManagement.Domain.Users;
using ProjectManagement.Web.Api.Mappers;
using ProjectManagement.Web.Api.Requests;
using ProjectManagement.Web.Extensions;

namespace ProjectManagement.Web.Api;

[ApiController]
[Authorize]
[Route("api/tasks")]
public sealed class TasksController(
    UpdateTaskCommandHandler updateTask,
    UpdateTaskStatusCommandHandler updateTaskStatus) : ControllerBase
{
    [HttpPut("{taskId:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> UpdateTask(
        Guid taskId,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        await updateTask.Handle(request.ToCommand(taskId), cancellationToken);

        return NoContent();
    }

    [HttpPatch("{taskId:guid}/status")]
    [Authorize(Roles = RoleNames.Employee)]
    public async Task<IActionResult> UpdateTaskStatus(
        Guid taskId,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        await updateTaskStatus.Handle(
            request.ToCommand(taskId, User.GetRequiredUserId()),
            cancellationToken);

        return NoContent();
    }
}

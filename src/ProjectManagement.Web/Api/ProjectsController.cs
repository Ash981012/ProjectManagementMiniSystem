using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Projects.Commands;
using ProjectManagement.Domain.Users;
using ProjectManagement.Web.Api.Mappers;
using ProjectManagement.Web.Api.Requests;
using ProjectManagement.Web.Api.Responses;

namespace ProjectManagement.Web.Api;

[ApiController]
[Authorize]
[Route("api/projects")]
public sealed class ProjectsController(
    CreateProjectCommandHandler createProject,
    CreateTaskCommandHandler createTask) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<CreatedResourceResponse>> CreateProject(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var projectId = await createProject.Handle(request.ToCommand(), cancellationToken);

        return Created($"/Projects/Details/{projectId}", new CreatedResourceResponse(projectId));
    }

    [HttpPost("{projectId:guid}/tasks")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<CreatedResourceResponse>> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var taskId = await createTask.Handle(request.ToCommand(projectId), cancellationToken);

        return Created($"/api/tasks/{taskId}", new CreatedResourceResponse(taskId));
    }
}

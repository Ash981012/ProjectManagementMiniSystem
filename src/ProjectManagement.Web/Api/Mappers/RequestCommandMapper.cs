using ProjectManagement.Application.Projects.Commands;
using ProjectManagement.Web.Api.Requests;

namespace ProjectManagement.Web.Api.Mappers;

public static class RequestCommandMapper
{
    public static CreateProjectCommand ToCommand(this CreateProjectRequest request)
    {
        return new CreateProjectCommand(request.Name, request.Description);
    }

    public static CreateTaskCommand ToCommand(this CreateTaskRequest request, Guid projectId)
    {
        return new CreateTaskCommand(projectId, request.Title, request.Description, request.AssignedEmployeeId);
    }

    public static UpdateTaskCommand ToCommand(this UpdateTaskRequest request, Guid taskId)
    {
        return new UpdateTaskCommand(taskId, request.Title, request.Description, request.AssignedEmployeeId, request.RowVersion);
    }

    public static UpdateTaskStatusCommand ToCommand(
        this UpdateTaskStatusRequest request,
        Guid taskId,
        string currentUserId)
    {
        return new UpdateTaskStatusCommand(taskId, request.Status, currentUserId, request.RowVersion);
    }
}

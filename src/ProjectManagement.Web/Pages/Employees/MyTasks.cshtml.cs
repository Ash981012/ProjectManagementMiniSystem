using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.Projects.Dtos;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Users;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Web.Pages.Employees;

[Authorize(Roles = RoleNames.Employee)]
public sealed class MyTasksModel : PageModel
{
    private readonly GetEmployeeTasksQueryHandler _employeeTasks;
    private readonly UserManager<ApplicationUser> _userManager;

    public MyTasksModel(
        GetEmployeeTasksQueryHandler employeeTasks,
        UserManager<ApplicationUser> userManager)
    {
        _employeeTasks = employeeTasks;
        _userManager = userManager;
    }

    public IReadOnlyList<TaskDto> Tasks { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            Tasks = [];
            return;
        }

        Tasks = await _employeeTasks.Handle(new GetEmployeeTasksQuery(userId), cancellationToken);
    }
}

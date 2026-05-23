using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.Projects.Dtos;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Users;

namespace ProjectManagement.Web.Pages;

[Authorize]
public sealed class IndexModel : PageModel
{
    private readonly GetDashboardQueryHandler _dashboardQuery;

    public IndexModel(GetDashboardQueryHandler dashboardQuery)
    {
        _dashboardQuery = dashboardQuery;
    }

    public DashboardDto Dashboard { get; private set; } = new(0, 0, []);

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (User.IsInRole(RoleNames.Employee) && !User.IsInRole(RoleNames.Admin))
            return RedirectToPage("/Employees/MyTasks");

        Dashboard = await _dashboardQuery.Handle(new GetDashboardQuery(), cancellationToken);
        return Page();
    }
}

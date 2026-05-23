using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.Abstractions;
using ProjectManagement.Application.Projects.Dtos;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Users;

namespace ProjectManagement.Web.Pages.Projects;

[Authorize(Roles = RoleNames.Admin)]
public sealed class DetailsModel : PageModel
{
    private readonly IEmployeeDirectory _employees;
    private readonly GetProjectDetailsQueryHandler _projectDetails;

    public DetailsModel(
        GetProjectDetailsQueryHandler projectDetails,
        IEmployeeDirectory employees)
    {
        _projectDetails = projectDetails;
        _employees = employees;
    }

    public ProjectDetailsDto Project { get; private set; } = new(Guid.Empty, string.Empty, null, 0, 0, 0, []);

    public IReadOnlyList<EmployeeDto> Employees { get; private set; } = [];

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Project = await _projectDetails.Handle(new GetProjectDetailsQuery(id), cancellationToken);
        Employees = await _employees.ListEmployeesAsync(cancellationToken);
    }
}

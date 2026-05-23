using ProjectManagement.Application.Projects.Dtos;

namespace ProjectManagement.Application.Abstractions;

public interface IEmployeeDirectory
{
    Task<EmployeeDto?> FindEmployeeAsync(string employeeId, CancellationToken cancellationToken);

    Task<IReadOnlyList<EmployeeDto>> ListEmployeesAsync(CancellationToken cancellationToken);
}

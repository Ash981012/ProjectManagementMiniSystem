# Changes Applied

- Added FluentValidation package to Application project.
- Added validators for:
  - CreateProjectCommand
  - CreateTaskCommand
  - UpdateTaskCommand
  - UpdateTaskStatusCommand
  - RegisterEmployeeCommand
- Replaced manual command validation in handlers with FluentValidation-based validation.
- Added default Admin user seeding:
  - Email: admin@pms.com
  - Password: Admin@123
- Added Swagger/OpenAPI support in Development environment.
- Optimized employee task query to fetch tasks directly by AssignedEmployeeId instead of loading all projects/tasks into memory.
- Updated README with run steps, credentials, Swagger URL, and reviewer notes.
- Removed unnecessary generated folders from packaged solution: .vs, bin, obj, .dotnet-home, .tmp.

Note: Build was not executed in this environment because the .NET SDK is not installed here.

- Aligned FluentValidation max lengths with EF Core column configuration: Project.Name 120, Task.Title 160.

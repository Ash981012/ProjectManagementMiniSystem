# Project Management Mini-System

Clean ASP.NET Core project management mini-system based on the assessment PDF.

## Stack

- .NET 10 LTS
- ASP.NET Core Razor Pages
- REST API controllers for state-changing actions
- EF Core 10 with SQL Server
- ASP.NET Core Identity with Admin and Employee roles
- FluentValidation for command validation
- Swagger/OpenAPI for API testing
- DDD-inspired Domain layer
- CQRS-style command/query handlers
- In-memory caching for dashboard statistics

## Projects

- `ProjectManagement.Domain`: entities, domain rules, lifecycle timestamps, progress calculations.
- `ProjectManagement.Application`: command handlers, query handlers, DTOs, repository abstractions, FluentValidation validators.
- `ProjectManagement.Infrastructure`: EF Core, Identity, repositories, caching, seeding, migrations.
- `ProjectManagement.Web`: Razor Pages UI and REST API endpoints.

## Run

```powershell
dotnet restore
dotnet run --project src/ProjectManagement.Web/ProjectManagement.Web.csproj
```

Open the displayed local URL.

## Authentication

The application seeds the required roles and one default Admin user on startup.

```text
Admin Email: admin@pms.com
Admin Password: Admin@123
```

Public registration from `/Account/Register` creates Employee accounts and redirects to `/Account/Login`.

## Swagger

In Development environment, Swagger is available at:

```text
/swagger
```

Use Swagger to test these API endpoints after login/authentication setup:

- `POST /api/projects`
- `POST /api/projects/{projectId}/tasks`
- `PUT /api/tasks/{taskId}`
- `PATCH /api/tasks/{taskId}/status`

## Database

The app applies the included SQL Server migration and creates required Identity roles/Admin user on startup.

Default local SQL Server connection string:

```json
"DefaultConnection": "Server=(local);Database=ProjectManagementMiniSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

If your SSMS server name is different, update `src/ProjectManagement.Web/appsettings.json`.

## Notes for reviewers

- Validation is centralized with FluentValidation validators in the Application layer.
- Employee task listing queries tasks directly by `AssignedEmployeeId` instead of loading all projects/tasks into memory.
- Domain entities still protect business invariants, so validation exists at both application and domain levels.


## Concurrency

Task updates use SQL Server `rowversion` optimistic concurrency. Update APIs require the latest `rowVersion` value returned in task DTOs; if another user changes the task first, EF Core throws a concurrency exception and the API returns HTTP 409 Conflict.

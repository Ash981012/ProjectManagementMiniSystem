using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Projects.Commands;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Application.Users.Commands;

namespace ProjectManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateProjectCommandHandler>();
        services.AddScoped<CreateTaskCommandHandler>();
        services.AddScoped<UpdateTaskCommandHandler>();
        services.AddScoped<UpdateTaskStatusCommandHandler>();
        services.AddScoped<RegisterEmployeeCommandHandler>();
        services.AddScoped<GetDashboardQueryHandler>();
        services.AddScoped<GetProjectDetailsQueryHandler>();
        services.AddScoped<GetEmployeeTasksQueryHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}

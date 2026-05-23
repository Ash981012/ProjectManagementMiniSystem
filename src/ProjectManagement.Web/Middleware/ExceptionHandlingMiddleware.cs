using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common;
using ProjectManagement.Domain.Common;
using System.Runtime.ExceptionServices;

namespace ProjectManagement.Web.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(environment);

        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogError(exception, "An exception occurred after the response had already started.");
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        var (statusCode, title, logAsError) = GetProblemDetailsMetadata(exception);

        if (logAsError)
            _logger.LogError(exception, "Unhandled exception occurred.");
        else
            _logger.LogWarning(exception, "Handled application exception occurred.");

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = IsPublicException(exception) || _environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static (int StatusCode, string Title, bool LogAsError) GetProblemDetailsMetadata(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found.", false),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Access denied.", false),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Authentication required.", false),
            DomainException => (StatusCodes.Status400BadRequest, "Business rule validation failed.", false),
            UseCaseException => (StatusCodes.Status400BadRequest, "Request validation failed.", false),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request.", false),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "The resource was modified by another operation.", false),
            _ => (StatusCodes.Status500InternalServerError, "Server error.", true)
        };
    }

    private static bool IsPublicException(Exception exception)
    {
        return exception is NotFoundException
            or ForbiddenAccessException
            or UnauthorizedAccessException
            or DomainException
            or UseCaseException
            or ArgumentException
            or DbUpdateConcurrencyException;
    }
}

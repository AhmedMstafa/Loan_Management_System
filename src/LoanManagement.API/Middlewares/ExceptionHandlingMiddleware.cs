using System.Net;
using System.Text.Json;
using FluentValidation;
using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblemAsync(context, ex);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        var (status, title, type, errors) = ex switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, "Validation Failed", "https://api.loanmanagementsystem.com/errors/validation",
                ve.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
            DomainValidationException dve => (StatusCodes.Status400BadRequest, "Domain Validation Failed", "https://api.loanmanagementsystem.com/errors/domain-validation", dve.Errors),
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found", "https://api.loanmanagementsystem.com/errors/not-found", null),
            InvalidLoanStateException => (StatusCodes.Status409Conflict, "Invalid Loan State", "https://api.loanmanagementsystem.com/errors/invalid-loan-state", null),
            IdempotencyConflictException ice => (StatusCodes.Status409Conflict, "Idempotency Conflict", "https://api.loanmanagementsystem.com/errors/conflict",
                new Dictionary<string, string[]> { ["Idempotency-Key"] = [$"Duplicate key '{ice.Key}' submitted with differing payload"] }),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "https://api.loanmanagementsystem.com/errors/internal", null)
        };

        if (context.Response.HasStarted) return;
        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = ex.Message,
            Instance = context.Request.Path
        };
        if (errors is not null) problem.Extensions["errors"] = errors;
        problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
    }
}

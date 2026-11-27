using System.Security.Cryptography;
using System.Text;
using LoanManagement.Application.Common.Interfaces;

namespace LoanManagement.API.Middlewares;

public sealed class IdempotencyMiddleware : IMiddleware
{
    private static readonly HashSet<string> MutatingMethods = ["POST", "PUT", "PATCH", "DELETE"];
    private readonly IIdempotencyService _idempotency;
    private readonly ICurrentUserService _currentUser;

    public IdempotencyMiddleware(IIdempotencyService idempotency, ICurrentUserService currentUser)
    {
        _idempotency = idempotency;
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!MutatingMethods.Contains(context.Request.Method))
        {
            await next(context);
            return;
        }

        var key = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(key))
        {
            await next(context); // enforcement can be tightened; keep compatible for now
            return;
        }

        var orgId = _currentUser.OrganizationId ?? context.Request.Headers["X-Organization-Id"].FirstOrDefault() ?? "anonymous";
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
        context.Request.Body.Position = 0;

        var requestHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{context.Request.Method}:{context.Request.Path}:{body}")));
        var cached = await _idempotency.GetAsync(orgId, key, requestHash, context.RequestAborted);
        if (cached is not null)
        {
            context.Response.StatusCode = cached.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cached.ResponsePayload);
            return;
        }

        var originalBody = context.Response.Body;
        await using var mem = new MemoryStream();
        context.Response.Body = mem;
        try
        {
            await next(context);
            mem.Position = 0;
            var responseBody = await new StreamReader(mem).ReadToEndAsync();
            mem.Position = 0;
            await mem.CopyToAsync(originalBody);
            context.Response.Body = originalBody;

            if (context.Response.StatusCode < 500)
            {
                await _idempotency.StoreAsync(orgId, key, requestHash, context.Response.StatusCode, responseBody, TimeSpan.FromHours(24), context.RequestAborted);
            }
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}

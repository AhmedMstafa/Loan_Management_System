using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoanManagement.API.Filters;

public sealed class TenantAuthorizationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var claimOrg = user.FindFirst("org_id")?.Value ?? user.FindFirst("organizationId")?.Value;
            if (!string.IsNullOrWhiteSpace(claimOrg))
            {
                context.HttpContext.Items["OrganizationId"] = claimOrg;
            }
        }

        await next();
    }
}

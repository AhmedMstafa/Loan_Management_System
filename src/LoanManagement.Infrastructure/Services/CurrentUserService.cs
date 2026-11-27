using System.Security.Claims;
using LoanManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LoanManagement.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");
    public string? OrganizationId => User?.FindFirstValue("org_id") ?? User?.FindFirstValue("organizationId");
    public string? Role => User?.FindFirstValue(ClaimTypes.Role) ?? User?.FindFirstValue("role");
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}

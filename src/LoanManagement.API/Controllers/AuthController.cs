using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LoanManagement.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Tags("Authentication")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates a valid JWT token for testing and Swagger authorization
    /// </summary>
    /// <param name="request">Optional user, organization, and roles. Leave empty to get a full Super Admin token.</param>
    /// <returns>JWT Bearer token to authorize in Swagger</returns>
    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult GenerateToken([FromBody] TokenRequest? request)
    {
        var req = request ?? new TokenRequest(null, null, null, null);
        var secret = _configuration["Jwt:Key"] ?? "DEV_ONLY_SUPER_SECRET_KEY_CHANGE_ME_1234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, req.UserId ?? "usr_admin_01"),
            new(JwtRegisteredClaimNames.Name, req.UserName ?? "Super Admin"),
            new("org_id", req.OrganizationId ?? "org_default"),
            new("organizationId", req.OrganizationId ?? "org_default")
        };

        var roles = req.Roles is { Count: > 0 }
            ? req.Roles
            : new List<string> { "Admin", "LoanOfficer", "Underwriter", "BackOffice", "Accountant", "Collector", "RiskAnalyst", "Customer", "Compliance", "Security", "Ops", "Support", "Scheduler", "System" };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new TokenResponse(tokenString, "Bearer", (int)TimeSpan.FromDays(7).TotalSeconds, roles));
    }
}

public sealed record TokenRequest(string? UserId, string? UserName, string? OrganizationId, List<string>? Roles);
public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresIn, List<string> Roles);

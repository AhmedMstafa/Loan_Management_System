using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Tags("Authentication")]
public sealed class AuthController : ControllerBase
{
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin", "LoanOfficer", "Underwriter", "BackOffice", "Accountant", "Collector",
        "RiskAnalyst", "Customer", "Compliance", "Security", "Ops", "Support", "Scheduler", "System"
    };

    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IDateTimeProvider _clock;

    public AuthController(IApplicationDbContext db, IPasswordHasher passwordHasher, IJwtTokenGenerator jwt, IDateTimeProvider clock)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _clock = clock;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new ProblemDetails { Title = "Validation Failed", Detail = "FullName, Email, and Password are required.", Status = StatusCodes.Status400BadRequest });

        if (request.Password.Length < 6)
            return BadRequest(new ProblemDetails { Title = "Validation Failed", Detail = "Password must be at least 6 characters.", Status = StatusCodes.Status400BadRequest });

        var role = NormalizeRole(request.Role);
        if (role is null)
            return BadRequest(new ProblemDetails { Title = "Validation Failed", Detail = "Invalid role.", Status = StatusCodes.Status400BadRequest });

        var orgId = string.IsNullOrWhiteSpace(request.OrganizationId) ? "org_default" : request.OrganizationId.Trim();
        var exists = await _db.Users.AnyAsync(u => u.Email == email, ct);
        if (exists)
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = "A user with this email already exists.", Status = StatusCodes.Status409Conflict });

        var user = new User(
            id: $"usr_{Guid.NewGuid():N}"[..16],
            organizationId: orgId,
            email: email,
            passwordHash: _passwordHasher.Hash(request.Password),
            fullName: request.FullName.Trim(),
            role: role,
            isActive: true);
        user.MarkCreated(_clock.UtcNow, "system");

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return StatusCode(StatusCodes.Status201Created, new RegisterResponse(user.Id, user.Email, user.FullName, user.Role, user.OrganizationId, user.IsActive));
    }

    /// <summary>
    /// Login with email and password and receive a JWT.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ProblemDetails { Title = "Validation Failed", Detail = "Email and Password are required.", Status = StatusCodes.Status400BadRequest });

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "Invalid email or password.", Status = StatusCodes.Status401Unauthorized });

        var token = _jwt.Generate(user.Id, user.FullName, user.OrganizationId, [user.Role]);
        return Ok(new AuthTokenResponse(token.AccessToken, token.TokenType, token.ExpiresInSeconds, user.Id, user.Email, user.FullName, user.OrganizationId, token.Roles.ToList()));
    }

    /// <summary>
    /// Generates a valid JWT token for testing and Swagger authorization.
    /// </summary>
    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequest? request, CancellationToken ct)
    {
        var req = request ?? new TokenRequest(null, null, null, null);
        var roles = SanitizeRoles(req.Roles);
        var orgId = string.IsNullOrWhiteSpace(req.OrganizationId) ? "org_default" : req.OrganizationId.Trim();

        // If a known seeded user email/id is provided via UserName as email, prefer real user; otherwise generate dev token.
        if (!string.IsNullOrWhiteSpace(req.UserName) && req.UserName.Contains('@'))
        {
            var email = req.UserName.Trim().ToLowerInvariant();
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email && u.IsActive, ct);
            if (user is not null)
            {
                var userToken = _jwt.Generate(user.Id, user.FullName, user.OrganizationId, [user.Role]);
                return Ok(new AuthTokenResponse(userToken.AccessToken, userToken.TokenType, userToken.ExpiresInSeconds, user.Id, user.Email, user.FullName, user.OrganizationId, userToken.Roles.ToList()));
            }
        }

        var userId = string.IsNullOrWhiteSpace(req.UserId) ? "usr_admin_01" : req.UserId.Trim();
        var userName = string.IsNullOrWhiteSpace(req.UserName) ? "Super Admin" : req.UserName.Trim();
        var token = _jwt.Generate(userId, userName, orgId, roles);
        return Ok(new AuthTokenResponse(token.AccessToken, token.TokenType, token.ExpiresInSeconds, userId, userName.Contains('@') ? userName : $"{userName.ToLowerInvariant().Replace(' ', '.')}@bank.local", userName, orgId, token.Roles.ToList()));
    }

    private static List<string> SanitizeRoles(List<string>? roles)
    {
        var cleaned = roles?
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Where(r => !string.Equals(r, "string", StringComparison.OrdinalIgnoreCase))
            .Where(r => AllowedRoles.Contains(r))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        return cleaned.Count > 0 ? cleaned : ["Admin"];
    }

    private static string? NormalizeRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role)) return null;
        var match = AllowedRoles.FirstOrDefault(r => string.Equals(r, role.Trim(), StringComparison.OrdinalIgnoreCase));
        return match;
    }
}

public sealed record RegisterRequest(string FullName, string Email, string Password, string Role, string OrganizationId);
public sealed record RegisterResponse(string Id, string Email, string FullName, string Role, string OrganizationId, bool IsActive);
public sealed record LoginRequest(string Email, string Password);
public sealed record TokenRequest(string? UserId, string? UserName, string? OrganizationId, List<string>? Roles);
public sealed record AuthTokenResponse(string AccessToken, string TokenType, int ExpiresIn, string UserId, string Email, string FullName, string OrganizationId, List<string> Roles);

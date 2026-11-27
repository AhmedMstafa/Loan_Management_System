using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoanManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LoanManagement.Infrastructure.Services;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    public JwtTokenGenerator(IConfiguration configuration) => _configuration = configuration;

    public JwtTokenResult Generate(string userId, string userName, string organizationId, IEnumerable<string> roles)
    {
        var secret = _configuration["Jwt:Key"] ?? "DEV_ONLY_SUPER_SECRET_KEY_CHANGE_ME_1234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roleList = roles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Name, userName),
            new("org_id", organizationId),
            new("organizationId", organizationId)
        };

        foreach (var role in roleList)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var expires = DateTime.UtcNow.AddDays(7);
        var token = new JwtSecurityToken(claims: claims, expires: expires, signingCredentials: creds);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtTokenResult(tokenString, "Bearer", (int)TimeSpan.FromDays(7).TotalSeconds, roleList);
    }
}

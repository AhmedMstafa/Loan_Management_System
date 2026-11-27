namespace LoanManagement.Application.Common.Interfaces;

public sealed record JwtTokenResult(string AccessToken, string TokenType, int ExpiresInSeconds, IReadOnlyList<string> Roles);

public interface IJwtTokenGenerator
{
    JwtTokenResult Generate(string userId, string userName, string organizationId, IEnumerable<string> roles);
}

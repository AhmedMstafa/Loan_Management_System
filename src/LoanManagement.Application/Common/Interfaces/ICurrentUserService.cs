namespace LoanManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? OrganizationId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}

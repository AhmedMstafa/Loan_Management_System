using LoanManagement.Domain.Enums;

namespace LoanManagement.Application.Common.Interfaces;

public sealed record CreditCheckResult(
    int CreditScore,
    RiskRating RiskRating,
    decimal DebtToIncomeRatio,
    string Recommendation,
    DateTimeOffset CheckedAt);

public interface ICreditBureauService
{
    Task<CreditCheckResult> CheckAsync(string borrowerId, string provider, bool forceRefresh, CancellationToken cancellationToken = default);
}

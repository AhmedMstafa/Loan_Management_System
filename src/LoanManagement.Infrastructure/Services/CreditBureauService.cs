using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Infrastructure.Services;

public sealed class CreditBureauService : ICreditBureauService
{
    public Task<CreditCheckResult> CheckAsync(string borrowerId, string provider, bool forceRefresh, CancellationToken cancellationToken = default)
    {
        // Mock deterministic scoring for local/dev usage.
        var seed = Math.Abs(HashCode.Combine(borrowerId, provider, forceRefresh));
        var score = 550 + (seed % 251); // 550-800
        var dti = decimal.Round(((seed % 60) / 100m), 4, MidpointRounding.ToEven); // 0.00-0.59
        var rating = score >= 720 ? RiskRating.Low : score >= 660 ? RiskRating.Medium : RiskRating.High;
        var recommendation = score >= 680 && dti <= 0.45m ? "Approve" : "Review";

        return Task.FromResult(new CreditCheckResult(
            CreditScore: score,
            RiskRating: rating,
            DebtToIncomeRatio: dti,
            Recommendation: recommendation,
            CheckedAt: DateTimeOffset.UtcNow));
    }
}

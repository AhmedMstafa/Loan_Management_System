using LoanManagement.Domain.Common;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public sealed class CreditCheck : BaseEntity<string>
{
    private CreditCheck() { }

    public CreditCheck(
        string id,
        string organizationId,
        string loanId,
        string provider,
        int creditScore,
        RiskRating riskRating,
        decimal debtToIncomeRatio,
        string recommendation,
        DateTimeOffset checkedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        LoanId = loanId;
        Provider = provider;
        CreditScore = creditScore;
        RiskRating = riskRating;
        DebtToIncomeRatio = decimal.Round(debtToIncomeRatio, 4, MidpointRounding.ToEven);
        Recommendation = recommendation;
        CheckedAt = checkedAt;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string LoanId { get; private set; } = string.Empty;
    public string Provider { get; private set; } = string.Empty;
    public int CreditScore { get; private set; }
    public RiskRating RiskRating { get; private set; }
    public decimal DebtToIncomeRatio { get; private set; }
    public string Recommendation { get; private set; } = string.Empty;
    public DateTimeOffset CheckedAt { get; private set; }
}

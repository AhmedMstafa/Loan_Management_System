using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Entities;

public sealed class LoanProduct : BaseEntity<string>, IAggregateRoot
{
    private LoanProduct() { }

    public LoanProduct(
        string id,
        string organizationId,
        string name,
        decimal baseInterestRate,
        decimal minAmount,
        decimal maxAmount,
        int minTenorMonths,
        int maxTenorMonths,
        decimal originationFeePct = 0m,
        decimal lateFeeFlat = 0m)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Product id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(organizationId)) throw new ArgumentException("Organization id is required.", nameof(organizationId));
        if (minAmount <= 0 || maxAmount < minAmount) throw new ArgumentException("Invalid loan amount limits.");
        if (minTenorMonths <= 0 || maxTenorMonths < minTenorMonths) throw new ArgumentException("Invalid tenor limits.");
        if (baseInterestRate < 0) throw new ArgumentOutOfRangeException(nameof(baseInterestRate));

        Id = id;
        OrganizationId = organizationId;
        Name = name;
        BaseInterestRate = decimal.Round(baseInterestRate, 4, MidpointRounding.ToEven);
        MinAmount = decimal.Round(minAmount, 4, MidpointRounding.ToEven);
        MaxAmount = decimal.Round(maxAmount, 4, MidpointRounding.ToEven);
        MinTenorMonths = minTenorMonths;
        MaxTenorMonths = maxTenorMonths;
        OriginationFeePct = decimal.Round(originationFeePct, 2, MidpointRounding.ToEven);
        LateFeeFlat = decimal.Round(lateFeeFlat, 4, MidpointRounding.ToEven);
        IsActive = true;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal BaseInterestRate { get; private set; }
    public decimal MinAmount { get; private set; }
    public decimal MaxAmount { get; private set; }
    public int MinTenorMonths { get; private set; }
    public int MaxTenorMonths { get; private set; }
    public decimal OriginationFeePct { get; private set; }
    public decimal LateFeeFlat { get; private set; }
    public bool IsActive { get; private set; }

    public void UpdateRates(decimal baseInterestRate, decimal minAmount, decimal maxAmount, decimal originationFeePct, decimal lateFeeFlat)
    {
        if (minAmount <= 0 || maxAmount < minAmount) throw new ArgumentException("Invalid loan amount limits.");
        if (baseInterestRate < 0) throw new ArgumentOutOfRangeException(nameof(baseInterestRate));

        BaseInterestRate = decimal.Round(baseInterestRate, 4, MidpointRounding.ToEven);
        MinAmount = decimal.Round(minAmount, 4, MidpointRounding.ToEven);
        MaxAmount = decimal.Round(maxAmount, 4, MidpointRounding.ToEven);
        OriginationFeePct = decimal.Round(originationFeePct, 2, MidpointRounding.ToEven);
        LateFeeFlat = decimal.Round(lateFeeFlat, 4, MidpointRounding.ToEven);
    }
}

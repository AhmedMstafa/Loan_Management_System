using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Products.Commands.ManageProductRates;

public sealed record ManageProductRatesCommand(string OrganizationId, string ProductId, decimal BaseInterestRate, decimal MinLoanAmount, decimal MaxLoanAmount, decimal OriginationFeePercentage, decimal LateFeeFlatRate, DateOnly EffectiveDate) : ICommand<ManageProductRatesResponse>;
public sealed record ManageProductRatesResponse(string ProductId, string Status, DateOnly EffectiveDate, DateTimeOffset UpdatedAt);

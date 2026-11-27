using LoanManagement.Application.Common.Models;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Application.Features.Loans.Commands.RunCreditCheck;

public sealed record RunCreditCheckCommand(string OrganizationId, string LoanId, string Provider, bool ForceRefresh) : ICommand<RunCreditCheckResponse>;

public sealed record RunCreditCheckResponse(string LoanId, int CreditScore, RiskRating RiskRating, decimal DebtToIncomeRatio, string Recommendation, DateTimeOffset CheckedAt);

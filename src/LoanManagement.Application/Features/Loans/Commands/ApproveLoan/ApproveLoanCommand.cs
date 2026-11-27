using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Loans.Commands.ApproveLoan;

public sealed record ApproveLoanCommand(
    string OrganizationId,
    string LoanId,
    decimal ApprovedAmount,
    decimal ApprovedInterestRate,
    int TenorMonths,
    string Notes,
    DateOnly ScheduledDisbursementDate,
    string ApprovedBy) : ICommand<ApproveLoanResponse>;

public sealed record ApproveLoanResponse(string LoanId, string Status, decimal ApprovedAmount, string ApprovedBy, DateTimeOffset ApprovedAt);

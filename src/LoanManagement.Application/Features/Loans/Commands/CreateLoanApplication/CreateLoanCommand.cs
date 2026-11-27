using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Loans.Commands.CreateLoanApplication;

public sealed record CreateLoanCommand(
    string OrganizationId,
    string BorrowerId,
    string ProductId,
    decimal RequestedAmount,
    string Currency,
    int TenorMonths,
    decimal AnnualInterestRate,
    string? Purpose,
    string? CollateralDescription) : ICommand<CreateLoanResponse>;

public sealed record CreateLoanResponse(
    string Id,
    string Status,
    string OrganizationId,
    string BorrowerId,
    decimal RequestedAmount,
    int TenorMonths,
    DateTimeOffset CreatedAt);
